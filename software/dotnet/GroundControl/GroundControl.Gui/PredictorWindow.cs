using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using GMap.NET;
using GroundControl.Gui.Properties;
using GroundControl.Core;

namespace GroundControl.Gui
{
    public partial class PredictorWindow : Form
    {
        private const string PREDICTOR_URL = "http://predict.habhub.org";
        private static readonly DateTime UNIX_TIMESTAMP_BASE = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private string      m_filename;
        private PointLatLng m_mapPosition;
        private PointLatLng m_groundControlPosition;
        //private PointLatLng m_capsulePosition;
        private TelemetryData m_telemetry;
        private int         m_launchIndex = 0;
        public event EventHandler NewPrediction;
        
        public string Filename
        {
            get { return m_filename; }
        }

        public PointLatLng MapPosition
        {
            get { return m_mapPosition; }
            set { m_mapPosition = value; }
        }

        public PredictorWindow()
        {
            InitializeComponent();
            cboxPosition.SelectedIndex = 0;
        }

        private void btnRunPrediction_Click(object sender, EventArgs e)
        {
            Thread workerThread = new Thread(new ThreadStart(CheckState));
            workerThread.Start();
        }

        private void CheckState()
        {
            Thread predictorThread = new Thread(new ThreadStart(RunPrediction));
            EnableButton(false);
            predictorThread.Start();

            if (!predictorThread.Join(60000))
            {
                predictorThread.Abort();
                RefreshProgress("timeout");
            }
            EnableButton(true);
        }

        private void RunPrediction()
        {
            bool success = true;
            string error = "";
            try
            {
                RefreshProgress("starting...");
                double latitude = 0;
                double longitude = 0;
                int altitude = (int)numLaunchAltitude.Value;
                DateTime date = new DateTime(datepickerLaunchDate.Value.Year, datepickerLaunchDate.Value.Month, datepickerLaunchDate.Value.Day,
                    datepickerLaunchTime.Value.Hour, datepickerLaunchTime.Value.Minute, datepickerLaunchTime.Value.Second).ToUniversalTime();

                switch (m_launchIndex)
                {
                    case 0://"Center of Map"
                        latitude = m_mapPosition.Lat;
                        longitude = m_mapPosition.Lng;
                        break;
                    case 1://"Ballon"
                        latitude = m_telemetry.Latitude;
                        longitude = m_telemetry.Longitude;
                        altitude = (int)m_telemetry.GpsAltitude;
                        date = m_telemetry.UtcTimestamp;
                        break;
                    case 2://"GroundControl"
                        latitude = m_groundControlPosition.Lat;
                        longitude = m_groundControlPosition.Lng;
                        break;
                }

                string postData = "launchsite=Other&" +
                    "lat=" + latitude + "&" +
                    "lon=" + longitude + "&" +
                    "initial_alt=" + altitude + "&" +
                    "hour=" + date.Hour + "&" +
                    "min=" + date.Minute + "&" +
                    "second=" + date.Second + "&" +
                    "day=" + date.Day + "&" +
                    "month=" + date.Month + "&" +
                    "year=" + date.Year + "&" +
                    "ascent=" + numAscentRate.Value + "&" +
                    "burst=" + numBurstAltitude.Value + "&" +
                    "drag=" + numDescentRate.Value + "&" +
                    "submit=Run+Prediction";


                // Requesting a uuid from the predictor website
                WebRequest request = WebRequest.Create(PREDICTOR_URL + "/ajax.php?action=submitForm");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(Encoding.UTF8.GetBytes(postData), 0, postData.Length);
                }

                // Get the uuid in the response.
                string uuid = "";
                using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    // Sample answer:
                    // {"valid":"true","uuid":"226b6e55ba2169a2bc3d9c53fd208b250e9ab2a9","timestamp":1375099200}
                    var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(reader.ReadToEnd());
                    if (dict["valid"].ToString().Equals("false"))
                    {
                        success = false;
                        error = dict["error"].ToString();
                    }
                    else
                    {
                        uuid = dict["uuid"].ToString();
                    }
                }

                if (success)
                {
                    // Polling for prediction calculation progress
                    bool running = false;
                    bool complete = false;
                    int i = 1;
                    do
                    {
                        Thread.Sleep(500);
                        string reqString = PREDICTOR_URL + "/preds/" + uuid + "/progress.json?_=" + (long)((DateTime.UtcNow - UNIX_TIMESTAMP_BASE)).TotalMilliseconds;
                        request = WebRequest.Create(reqString);
                        request.Method = "GET";
                        using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                        {
                            // Sample answer:
                            //"warnings": false,
                            //"pred_output": [],
                            //"dataset": "2013072700",
                            //"pred_running": true, 
                            //"run_time": "1370172040", 
                            //"error": "",                             
                            //"pred_complete": false, 

                            var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(reader.ReadToEnd());
                            running = Convert.ToBoolean(dict["pred_running"]);
                            complete = Convert.ToBoolean(dict["pred_complete"]);
                            error = Convert.ToString(dict["error"]);
                            if (complete)
                            {
                                running = false;
                            }
                            if (error.Equals(""))
                            {
                                RefreshProgress("running... " + i++);
                            }
                            else
                            {
                                RefreshProgress("Error: " + error);
                                break;
                            }
                        }
                    }
                    while (running);

                    if (!complete)
                    {
                        RefreshProgress("Prediction not finished!");
                        return;
                    }

                    // prediction was successful. Requesting the KML file
                    RefreshProgress("getting kml...");
                    request = WebRequest.Create(PREDICTOR_URL + "/kml.php?uuid=" + uuid);
                    request.Method = "GET";
                    m_filename = Settings.Default.DataDirectory + "\\Predict_" + DateTime.Now.ToShortDateString() + "_" +
                                 datepickerLaunchDate.Value.ToShortDateString() + "_" +
                                 datepickerLaunchTime.Value.ToString("HH.mm") + "_" +
                                 numBurstAltitude.Value + "_" + numAscentRate.Value + "_" + numDescentRate.Value + ".kml";

                    // save kml to file
                    using (var fileStream = File.Create(m_filename))
                    {
                        request.GetResponse().GetResponseStream().CopyTo(fileStream);
                    }
                    
                    if (NewPrediction != null)
                    {
                        NewPrediction.Invoke(this, new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                success = false;
                error = ex.Message;
            }
            RefreshProgress(success ? "Finished" : error);
        }

        private void RefreshProgress(string text)
        {
            Invoke(new MethodInvoker(delegate
            {
                lblTimeRemaining.Text = text;
            }));
        }

        private void EnableButton(bool enable)
        {
            Invoke(new MethodInvoker(delegate
            {
                btnRunPrediction.Enabled = enable;
            }));
        }

        public void MapPositionChanged(PointLatLng point)
        {
            m_mapPosition = point;
        }

        public void GroundControlPositionChanged(PointLatLng point)
        {
            m_groundControlPosition = point;
        }

        public void CapsulePositionChanged(TelemetryData telemetry)
        {
            m_telemetry = telemetry;
        }

        private void cboxPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_launchIndex = cboxPosition.SelectedIndex;
            numLaunchAltitude.Enabled = m_launchIndex != 1;
            datepickerLaunchDate.Enabled = m_launchIndex != 1;
            datepickerLaunchTime.Enabled = m_launchIndex != 1;
        }
    }
}
