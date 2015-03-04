using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace GroundControl.Gui
{
    public struct FlightRadar24Data
    {
        public string       Name;
        public PointLatLng  Position;
        public int          Heading;
        public int          Altitude;
        public int          Speed;
        public int          Id;
        public string       AircraftType;
        public string       AircraftReg;
    }

    public class FlightRadar24
    {
        private readonly List<FlightRadar24Data> m_flightList;
        private BackgroundWorker m_captureThread;
        private GMapOverlay m_overlay;
        private GMapControl m_map;
        private int m_updateRate;

        public FlightRadar24(GMapControl map, int updateRate = 5)
        {
            m_map = map;
            m_updateRate = updateRate;
            m_overlay = new GMapOverlay();
            m_captureThread = new BackgroundWorker() { WorkerSupportsCancellation = true };
            m_flightList = new List<FlightRadar24Data>();
            m_captureThread.DoWork += new DoWorkEventHandler(FlightDoWork);
            m_map.Overlays.Add(m_overlay);
        }

        public void Start()
        {
            if (!m_captureThread.IsBusy)
            {
                m_captureThread.RunWorkerAsync();
            }
        }

        public void Stop()
        {
            if (m_captureThread.IsBusy)
            {
                m_captureThread.CancelAsync();
            }
        }

        private void FlightDoWork(object sender, DoWorkEventArgs e)
        {
            while (!m_captureThread.CancellationPending)
            {
                int startTime = Environment.TickCount;
                try
                {
                    lock (m_flightList)
                    {
                        GetFlightRadarData();
                    }
                    NewFlightData();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("flight_DoWork: " + ex.ToString());
                }
                int endTime = Environment.TickCount;
                int timeToWait = m_updateRate * 1000 - (endTime - startTime);
                if (timeToWait > 0)
                {
                    Thread.Sleep(timeToWait);
                }
            }
            lock (m_flightList)
            {
                m_flightList.Clear();
                m_overlay.Markers.Clear();
            }
            NewFlightData();
        }

        private void GetFlightRadarData()
        {
            //Debug.WriteLine("GetFlightRadarData()");
            m_flightList.Clear();
            string responseString = string.Empty;
            RectLatLng bounds = m_map.ViewArea;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                String.Format("http://krk.data.fr24.com/zones/fcgi/feed.js?bounds={0},{1},{2},{3}&maxage=900", bounds.Top, bounds.Bottom, bounds.Left, bounds.Right));
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader read = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        responseString = read.ReadToEnd();
                    }
                }
                response.Close();
            }
            responseString = responseString.Replace("\n", string.Empty);
            // remove stuff before first flight object
            int begin = responseString.IndexOf("\"version\":4");
            if (begin >= 0)
            {
                responseString = responseString.Substring(begin + 12);
            }
            // split at end of json arrays
            var items = responseString.Split(']');
            foreach (var it in items)
            {
                if (it.Length > 2)
                {
                    try
                    {
                        var par = it.Substring(2).Replace(":", ",").Replace("\"", string.Empty).Replace("[", string.Empty).Split(',');
                        if (par.Length == 19)
                        {
                            FlightRadar24Data fd = new FlightRadar24Data()
                            {
                                Name = par[1],
                                Heading = int.Parse(par[4]),
                                Altitude = (int)(int.Parse(par[5]) * 0.3048),
                                Speed = (int)(int.Parse(par[6]) * 1.852),
                                Position = new PointLatLng(double.Parse(par[2], CultureInfo.InvariantCulture), double.Parse(par[3], CultureInfo.InvariantCulture)),
                                Id = Convert.ToInt32(par[0], 16),
                                AircraftType = par[9],
                                AircraftReg = par[10]
                            };
                            m_flightList.Add(fd);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("GetFlightRadarData: " + ex.ToString());
                    }
                }
            }
        }

        private void NewFlightData()
        {
            m_map.Invoke(new MethodInvoker(delegate
            {
                m_map.HoldInvalidation = true;
                lock (m_flightList)
                {
                    // add new flightmarker or update position of all flights in visible map area
                    foreach (var flight in m_flightList.Where(x => m_map.ViewArea.Contains(x.Position)))
                    {
                        GMapMarkerFlightRadar24 marker = m_overlay.Markers.FirstOrDefault(m => m.Tag.Equals(flight.Id)) as GMapMarkerFlightRadar24;
                        if (marker == null)
                        {
                            marker = new GMapMarkerFlightRadar24(flight.Position) { Tag = flight.Id };
                            m_overlay.Markers.Add(marker);
                        }
                        marker.FlightRadarData = flight;
                    }
                    // remove all other flightmarkers
                    var flightsToRemove = m_flightList.Where(x => !m_map.ViewArea.Contains(x.Position)).ToList();
                    foreach (var flight in flightsToRemove)
                    {
                        GMapMarkerFlightRadar24 marker = m_overlay.Markers.FirstOrDefault(m => m.Tag.Equals(flight.Id)) as GMapMarkerFlightRadar24;
                        if (marker != null)
                        {
                            m_overlay.Markers.Remove(marker);
                        }
                    }
                }
                m_map.Refresh();
            }));
        }
    }
}
