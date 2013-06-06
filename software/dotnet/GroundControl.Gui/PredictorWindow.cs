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

namespace GroundControl.Gui
{
    public partial class PredictorWindow : Form
    {
        private const string PREDICTOR_URL = "http://habhub.org/predict";
        private string      m_filename;
        private PointLatLng m_mapPosition;
        private PointLatLng m_groundControlPosition;
        private PointLatLng m_capsulePosition;
        Thread m_workerThread;
        int m_launchIndex = 0;
        public event EventHandler NewPrediction;
        
        public string Filename
        {
            get { return m_filename; }
        }

        public PredictorWindow()
        {
            InitializeComponent();
            cboxPosition.SelectedIndex = 0;
        }

        private void btnRunPrediction_Click(object sender, EventArgs e)
        {
            m_workerThread = new Thread(new ThreadStart(CheckState));
            m_workerThread.Start();
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
            try
            {
                RefreshProgress("starting...");
                double latitude = 0;
                double longitude = 0;

                switch (m_launchIndex)
                {
                    case 0://"Center of Map"
                        latitude = m_mapPosition.Lat;
                        longitude = m_mapPosition.Lng;
                        break;
                    case 1://"Ballon"
                        latitude = m_capsulePosition.Lat;
                        longitude = m_capsulePosition.Lng;
                        break;
                    case 2://"GroundControl"
                        latitude = m_groundControlPosition.Lat;
                        longitude = m_groundControlPosition.Lng;
                        break;
                }

                string postData = "launchsite=Other&" +
                    "lat=" + latitude + "&" +
                    "lon=" + longitude + "&" +
                    "initial_alt=" + numLaunchAltitude.Value + "&" +
                    "hour=" + datepickerLaunchTime.Value.Hour + "&" +
                    "min=" + datepickerLaunchTime.Value.Minute + "&" +
                    "second=" + datepickerLaunchTime.Value.Second + "&" +
                    "day=" + datepickerLaunchDate.Value.Day + "&" +
                    "month=" + datepickerLaunchDate.Value.Month + "&" +
                    "year=" + datepickerLaunchDate.Value.Year + "&" +
                    "ascent=" + numAscentRate.Value + "&" +
                    "burst=" + numBurstAltitude.Value + "&" +
                    "drag=8&" +
                    "software=gfs&" +
                    "delta_lat=3&" +
                    "delta_lon=3&" +
                    "delta_time=5&" +
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
                    // {"valid":"true","uuid":"98a901ec92ce344a65a54b8bc176ce087a723492","timestamp":1370692680}
                    var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(reader.ReadToEnd());
                    if (dict["valid"].ToString().Equals("false"))
                    {
                        success = false;
                    }
                    uuid = dict["uuid"].ToString();
                }
                if (success)
                {
                    // Polling for prediction calculation progress
                    bool finished = false;
                    do
                    {
                        Thread.Sleep(500);
                        string reqString = PREDICTOR_URL + "/preds/" + uuid + "/progress.json?_=" + (long)((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalMilliseconds;
                        request = WebRequest.Create(reqString);
                        request.Method = "GET";
                        using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                        {
                            // Sample answer:
                            //"pred_running": false, 
                            //"error": "", 
                            //"run_time": "1370172040", 
                            //"gfs_timestamp": "", 
                            //"warnings": false, 
                            //"pred_complete": false, 
                            //"pred_output": [], 
                            //"gfs_percent": 0, 
                            //"gfs_complete": false, 
                            //"gfs_timeremaining": "Please wait..."

                            var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(reader.ReadToEnd());
                            finished = Convert.ToBoolean(dict["gfs_complete"]);
                            RefreshProgress(Convert.ToInt32(dict["gfs_percent"]) + "%");
                        }
                    }
                    while (!finished);

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
            }
            RefreshProgress(success? "Finished" : "Failed");
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

        public void CapsulePositionChanged(PointLatLng point, float altitude)
        {
            m_capsulePosition = point;
            numLaunchAltitude.Value = (int)altitude;
        }

        private void cboxPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_launchIndex = cboxPosition.SelectedIndex;
        }
    }
}
