using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using GMap.NET;
using GroundControl.Core;

namespace TelemetryAnalyzer
{
    public class Mission
    {
        private string m_name;
        private string m_telemetryFile;
        private DataCache m_dataCache; // telemetry data
        private List<String> m_videoFiles; // list of all Gopro video filenames
        private List<String> m_imageFiles; // list of all Gopro image filenames
        private int m_videoOffset = 0; // time difference in seconds between telemetry launch and first video
        private int m_imageOffset = 0; // time difference in seconds between telemetry launch and first image
        private int m_numberOfVideoFrames = 0;

        [Browsable(false)]
        public int NumberOfImages { get { return m_imageFiles.Count; } }

        [Browsable(false)]
        public int NumberOfVideoFrames { get { return m_numberOfVideoFrames; } }


        private void InitVideos()
        {
            // TODO

            //m_numberOfVideoFrames = 0;
            //foreach (var videofilename in m_videoFiles)
            //{
            //    VideoFileReader inputVideo = new VideoFileReader();
            //    inputVideo.Open(videofilename);
            //    m_numberOfVideoFrames += (int)inputVideo.FrameCount;
            //    inputVideo.Close();
            //}
        }

        public static Mission FromFile(string name, string telemetryFile, string imagesPath = "", string videoPath = "")
        {
            DataCache dataCache = new DataCache();
            if (File.Exists(telemetryFile) && DataLoader.LoadTelemetryData(telemetryFile, dataCache))
            {
                return new Mission(dataCache, name, telemetryFile, imagesPath, videoPath);
            }
            return null;
        }

        private Mission(DataCache data, string name, string telemetryfile, string imagesPath, string videoPath)
        {
            m_name = name;
            m_telemetryFile = telemetryfile;
            m_dataCache = data;

            m_imageFiles = new List<string>();
            m_videoFiles = new List<string>();

            if (Directory.Exists(imagesPath))
            {
                m_imageFiles.AddRange(Directory.GetFiles(imagesPath, "G*.JPG"));
            }
            if (Directory.Exists(videoPath))
            {
                string[] mainVideo = Directory.GetFiles(videoPath, "GOPR*.MP4");
                if (mainVideo.Length == 1)
                {
                    m_videoFiles.Add(mainVideo[0]);
                    string[] subvideos = Directory.GetFiles(videoPath, "G*.MP4");
                    Array.Sort(subvideos);
                    foreach (var subvideo in subvideos)
                    {
                        if (!m_videoFiles.Contains(subvideo))
                        {
                            m_videoFiles.Add(subvideo);
                        }
                    }
                    InitVideos();
                }
            }
        }

        public static double CalcDistance(float latitude1, float longitude1, float latitude2, float longitude2, float alt1 = 0, float alt2 = 0)
        {
            double e = (Math.PI * latitude1 / 180);
            double f = (Math.PI * longitude1 / 180);
            double g = (Math.PI * latitude2 / 180);
            double h = (Math.PI * longitude2 / 180);
            double i = (Math.Cos(e) * Math.Cos(g) * Math.Cos(f) * Math.Cos(h) +
                        Math.Cos(e) * Math.Sin(f) * Math.Cos(g) * Math.Sin(h) +
                        Math.Sin(e) * Math.Sin(g));
            double j = (Math.Acos(i));
            double k = Double.IsNaN(j) ? 0 : (6371000 * j);

            return Math.Sqrt(k * k + (alt1 - alt2) * (alt1 - alt2));
        }

        public RectLatLng GetBounds()
        {
            var flight = Flight;
            double minLat = flight.Min(x => x.Latitude);
            double minLng = flight.Min(x => x.Longitude);
            double maxLat = flight.Max(x => x.Latitude);
            double maxLng = flight.Max(x => x.Longitude);
            return new RectLatLng(maxLat, minLng, maxLng - minLng, maxLat - minLat);
        }

        public TelemetryData GetLaunch()
        {
            var tmp = m_dataCache.Telemetry.Select(x => x.VerticalSpeed);
            int burstIndex = m_dataCache.Telemetry.IndexOf(GetBurst());
            return m_dataCache.Telemetry.GetRange(0, burstIndex).Reverse<TelemetryData>().First(x => x.VerticalSpeed <= 0);
        }

        public TelemetryData GetBurst()
        {
            return m_dataCache.Telemetry.Aggregate((agg, next) => next.GpsAltitude > agg.GpsAltitude ? next : agg);
        }

        public TelemetryData GetLanding()
        {
            int burstIndex = m_dataCache.Telemetry.IndexOf(GetBurst());
            return m_dataCache.Telemetry.Skip(burstIndex + 1).First(x => x.VerticalSpeed >= 0);
        }

        public List<TelemetryData> Flight
        {
            get
            {
                int launchIndex = m_dataCache.Telemetry.IndexOf(GetLaunch());
                int landingIndex = m_dataCache.Telemetry.IndexOf(GetLanding());
                return m_dataCache.Telemetry.Skip(launchIndex).Take(landingIndex - launchIndex).ToList();
            }
        }

        [DisplayName("Mission Name")]
        public string Name
        {
            get { return m_name; }
        }

        [DisplayName("Launch Date")]
        public DateTime StartDate
        {
            get { return GetLaunch().UtcTimestamp.ToLocalTime(); }
        }

        [DisplayName("Flight Time [hh:mm:ss]")]
        public TimeSpan FlightTime
        {
            get { return (GetLanding().UtcTimestamp - GetLaunch().UtcTimestamp); }
        }

        [DisplayName("Time to burst [hh:mm:ss]")]
        public TimeSpan TimeToBurst
        {
            get { return (GetBurst().UtcTimestamp - GetLaunch().UtcTimestamp); }
        }

        [DisplayName("Flight Distance [km]")]
        public double FlightDistance
        {
            get
            {
                int launchIndex = m_dataCache.Telemetry.IndexOf(GetLaunch());
                int landingIndex = m_dataCache.Telemetry.IndexOf(GetLanding());
                double distance = 0;
                for (int i = launchIndex; i < landingIndex - 1; i++)
                {
                    TelemetryData a = m_dataCache.Telemetry[i];
                    TelemetryData b = m_dataCache.Telemetry[i + 1];
                    distance += CalcDistance(a.Latitude, a.Longitude, b.Latitude, b.Longitude, a.GpsAltitude, b.GpsAltitude);
                }
                return distance / 1000;
            }
        }

        [DisplayName("Launch - Landing Distance [km]")]
        public double LaunchLandingDistance
        {
            get
            {
                TelemetryData a = GetLaunch();
                TelemetryData b = GetLanding();
                double distance = CalcDistance(a.Latitude, a.Longitude, b.Latitude, b.Longitude, a.GpsAltitude, b.GpsAltitude);
                return distance / 1000;
            }
        }

        [DisplayName("Maximum Altitude [m]")]
        public float MaxAltitude
        {
            get { return GetBurst().GpsAltitude; }
        }

        [DisplayName("Maximum Horizontal Speed [km/h]")]
        public double MaxHSpeed
        {
            get { return m_dataCache.Telemetry.Max(x => x.HorizontalSpeed) * 3.6; }
        }

        [DisplayName("Maximum Vertical Speed [km/h]")]
        public double MaxVSpeed
        {
            get { return Math.Abs(m_dataCache.Telemetry.Min(x => x.VerticalSpeed) * 3.6); }
        }

        [DisplayName("Average Ascent Rate [m/s]")]
        public double AvgAscentRate
        {
            get
            {
                TelemetryData launch = GetLaunch();
                TelemetryData burst = GetBurst();
                return (burst.GpsAltitude - launch.GpsAltitude) / (burst.UtcTimestamp - launch.UtcTimestamp).TotalSeconds;
            }
        }

        [DisplayName("Average Descent Rate [m/s]")]
        public double AvgDescentRate
        {
            get
            {
                TelemetryData landing = GetLanding();
                TelemetryData burst = GetBurst();
                return Math.Abs((burst.GpsAltitude - landing.GpsAltitude) / (burst.UtcTimestamp - landing.UtcTimestamp).TotalSeconds);
            }
        }

        [DisplayName("Coldest Temperature [°C]")]
        public double ColdestTemperature
        {
            get { return Math.Min(m_dataCache.Telemetry.Min(t => t.Temperature1), m_dataCache.Telemetry.Min(t => t.Temperature2)); }
        }

        [DisplayName("Burst Temperature [°C]")]
        public double BurstTemperature
        {
            get
            {
                TelemetryData burst = GetBurst();
                return Math.Min(burst.Temperature1, burst.Temperature2);
            }
        }

        [DisplayName("Battery at Landing [V]")]
        public double BatteryAtLanding
        {
            get { return GetLanding().Vin; }
        }



        public Image GetImage(DateTime dateTime)
        {
            if (m_imageFiles.Count > 0)
            {
                DateTime shiftedDate = dateTime.AddSeconds(m_imageOffset);
                var closestImageFile = m_imageFiles.TakeWhile(x => File.GetCreationTime(x).Ticks <= shiftedDate.Ticks).LastOrDefault();
                if (closestImageFile != null)
                {
                    return ImageFast.FromFile(closestImageFile);
                }
            }
            return null;

        }

        public Image GetImageAtIndex(int index)
        {
            if (index < 0 || index >= m_imageFiles.Count)
            {
                return null;
            }
            return ImageFast.FromFile(m_imageFiles[index]);
        }

        public Image GetVideoFrameAtIndex(int index)
        {
            // TODO:
            return null;
        }

        public Image GetVideoFrame(DateTime date)
        {
            //TODO:
            return null;
        }
    }
}
