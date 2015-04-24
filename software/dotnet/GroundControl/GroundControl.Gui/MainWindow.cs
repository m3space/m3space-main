using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GroundControl.Core;
using GroundControl.Gui.Properties;
using System.IO;
using SharpKml.Engine;
using SharpKml.Dom;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GroundControl.Core.WebAccess;
using NMEA;

namespace GroundControl.Gui
{
    /// <summary>
    /// The main application window.
    /// </summary>
    public partial class MainWindow : Form
    {
        const float Rad2Deg = 180.0f / (float)Math.PI;

        private LogWindow logWindow;
        private LiveImageWindow liveImageWindow;
        private TelemetryWindow telemetryWindow;
        private MapWindow mapWindow;
        private GraphWindow graphWindow;
        private PredictorWindow predictorWindow;

        private PersistenceHandler persistHandler;
        private DataTransceiver transceiver;
        private DataProtocol protocol;
        private DataCache dataCache;
        private LiveTrackerWebAccess webAccess;

        private NMEA.GPSReceiverWrapper gpsReceiver;
        private DateTime lastUploadedGpsPos;

        private DateTime startTime;
        private Timer elapsedTimer;

        private string comPortRadio;
        private string comPortGPS;
        private string dataDirectory;

        private delegate void WriteString(string str);
        private delegate void VoidDelegate();
        private delegate void PositionDelegate(double latitude, double longitude);

        private GPSReceiverWrapper.NewFixHandler gpsUploadHandler;

        /// <summary>
        /// Cosntructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            dataCache = new DataCache();

            graphWindow = new GraphWindow();
            graphWindow.MdiParent = this;
            
            logWindow = new LogWindow();
            logWindow.MdiParent = this;            

            liveImageWindow = new LiveImageWindow();
            liveImageWindow.MdiParent = this;            

            telemetryWindow = new TelemetryWindow();
            telemetryWindow.MdiParent = this;            

            mapWindow = new MapWindow();
            mapWindow.MdiParent = this;

            predictorWindow = new PredictorWindow();
            predictorWindow.MdiParent = this;
            predictorWindow.MapPosition = mapWindow.MapPosition;

            if (Settings.Default.DataDirectory.Equals(""))
            {
                Settings.Default.DataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar + "m3space";
            }
            dataDirectory = Settings.Default.DataDirectory;

            comPortGPS = Settings.Default.ComPortGPS;
            comPortRadio = Settings.Default.ComPortRadio;

            if (Settings.Default.GraphWindowVisible)
            {
                graphWindow.Show();
                graphMenuItem.Checked = true;
            }
            if (Settings.Default.LogWindowVisible)
            {
                logWindow.Show();
                eventLogMenuItem.Checked = true;
            }
            if (Settings.Default.LiveImageWindowVisible)
            {
                liveImageWindow.Show();
                liveImageMenuItem.Checked = true;
            }
            if (Settings.Default.TelemetryWindowVisible)
            {
                telemetryWindow.Show();
                telemetryMenuItem.Checked = true;
            }
            if (Settings.Default.MapWindowVisible)
            {
                mapWindow.Show();
                mapMenuItem.Checked = true;
            }
            if (Settings.Default.PredictorWindowVisible)
            {
                predictorWindow.Show();
                predictorMenuItem.Checked = true;
            }

            webAccess = new LiveTrackerWebAccess();
            persistHandler = new PersistenceHandler();
            persistHandler.DataDirectory = dataDirectory;
            transceiver = new DataTransceiver(comPortRadio);
            protocol = new DataProtocol();
            protocol.Error += HandleError;
            protocol.TelemetryReceived += HandleTelemetry;
            protocol.TelemetryReceived += persistHandler.SaveTelemetry;
            protocol.ImageComplete += HandleImage;
            protocol.ImageComplete += persistHandler.SaveImage;
            transceiver.FrameReceived += protocol.DecodeFrame;
            transceiver.Error += HandleError;
            webAccess.Error += HandleError;


            lastUploadedGpsPos = DateTime.UtcNow;
            gpsReceiver = new GPSReceiverWrapper();
            gpsReceiver.PortSettings = new SerialPortSettings(comPortGPS, BaudRate.baudRate57600, System.IO.Ports.Parity.None, DataBits.dataBits8, System.IO.Ports.StopBits.One, System.IO.Ports.Handshake.None);
            gpsReceiver.NewFix += new GPSReceiverWrapper.NewFixHandler(gpsReceiver_NewFix);
            gpsUploadHandler = new GPSReceiverWrapper.NewFixHandler(gpsReceiver_UploadGpsPosition);

            webAccess.Url = Settings.Default.WebAccessUrl;
            if (Settings.Default.WebAccessEnabled)
            {
                protocol.TelemetryReceived += webAccess.UploadTelemetry;
                protocol.ImageComplete += webAccess.UploadLiveImage;
                gpsReceiver.NewFix += gpsUploadHandler;
            }

            blogMessageMenuItem.Enabled = Settings.Default.WebAccessEnabled;

            mapWindow.MapPositionChanged += new PositionChanged(predictorWindow.MapPositionChanged);
            predictorWindow.NewPrediction += new EventHandler(predictorWindow_NewPrediction);


            elapsedTimer = new Timer();
            elapsedTimer.Interval = 1000;
            elapsedTimer.Tick += ElapsedTick;

            RestoreSettings();
        }

        void predictorWindow_NewPrediction(object sender, EventArgs e)
        {
            LoadPrediction(predictorWindow.Filename);
        }

        private void gpsReceiver_NewFix(DateTime fixTime, GeographicDimension latitude, GeographicDimension longitude)
        {
            mapWindow.Invoke(new PositionDelegate(mapWindow.UpdateGroundPosition), new object[] { latitude.Angle, longitude.Angle });
            predictorWindow.GroundControlPositionChanged(new PointLatLng(latitude.Angle, longitude.Angle));
        }

        private void gpsReceiver_UploadGpsPosition(DateTime fixTime, GeographicDimension latitude, GeographicDimension longitude)
        {
            if (webAccess.IsRunning && (fixTime - lastUploadedGpsPos).TotalSeconds > 30)
            {
                GpsData gpsPos = new GpsData();
                gpsPos.UtcTimestamp = fixTime;
                gpsPos.Latitude = (float)latitude.Angle;
                gpsPos.Longitude = (float)longitude.Angle;
                try
                {
                    webAccess.UploadGpsPosition(gpsPos);
                }
                catch (Exception e)
                {
                    logWindow.WriteLine("GPS upload error: " + e.Message);
                }
                lastUploadedGpsPos = fixTime;
            }
        }

        /// <summary>
        /// Handles received telemetry.
        /// </summary>
        /// <param name="data">the telemetry data</param>
        private void HandleTelemetry(TelemetryData data)
        {
            bool burst = false;
            dataCache.AddTelemetry(data, out burst);

            string str = DataFormat.TelemetryDisplayString(data);           
            logWindow.Invoke(new WriteString(logWindow.WriteLine), new object[] { str });
            if (burst)
            {
                string bstr = "[Info] Balloon burst detected";
                logWindow.Invoke(new WriteString(logWindow.WriteLine), new object[] { bstr });
            }

            telemetryWindow.Invoke(new TelemetryHandler(telemetryWindow.DisplayTelemetry), new object[] { data });
            mapWindow.Invoke(new TelemetryBurstHandler(mapWindow.AddTelemetryPoint), new object[] { data, burst });
            graphWindow.Invoke(new TelemetryHandler(graphWindow.AddTelemetry), new object[] { data });
            predictorWindow.Invoke(new TelemetryHandler(predictorWindow.CapsulePositionChanged), new object[] { data });
        }

        /// <summary>
        /// Handles received images.
        /// </summary>
        /// <param name="utcTs">the UTC timestamp</param>
        /// <param name="data">the image data</param>
        /// <param name="ok">true if ok, false if with errors</param>
        private void HandleImage(DateTime utcTs, byte[] data, bool ok)
        {
            string str = String.Format("[Image] {0:yyyy/MM/dd HH:mm:ss} Size: {1} bytes {2}", utcTs.ToLocalTime(), data.Length, (ok) ? "OK" : "with errors");
            logWindow.Invoke(new WriteString(logWindow.WriteLine), new object[] { str });
            liveImageWindow.Invoke(new ImageCompleteHandler(liveImageWindow.UpdateImage), new object[] { utcTs, data, ok });
        }

        /// <summary>
        /// Handles error messages.
        /// </summary>
        /// <param name="message"></param>
        private void HandleError(string message)
        {
            string str = "[Error] " + message;
            logWindow.Invoke(new WriteString(logWindow.WriteLine), new object[] { str });
        }

        /// <summary>
        /// Displays an error dialog.
        /// </summary>
        /// <param name="e">the error</param>
        private void DisplayExecption(Exception e)
        {
            DialogResult result = MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Updates the elapsed time clock.
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="e">the event arguments</param>
        private void ElapsedTick(object sender, EventArgs e)
        {
            System.TimeSpan elapsed = (DateTime.Now - startTime);
            elapsedLbl.Text = String.Format("{0:00}:{1:00}:{2:00}", elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
        }

        /// <summary>
        /// Restores window settings.
        /// </summary>
        private void RestoreSettings()
        {
            this.Location = Settings.Default.MainWindowLocation;
            this.Size = Settings.Default.MainWindowSize;
            if (Settings.Default.MainWindowMaximized)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;
            telemetryWindow.Location = Settings.Default.TelemetryWindowLocation;
            mapWindow.Location = Settings.Default.MapWindowLocation;
            mapWindow.Size = Settings.Default.MapWindowSize;
            liveImageWindow.Location = Settings.Default.LiveImageWindowLocation;
            liveImageWindow.Size = Settings.Default.LiveImageWindowSize;
            logWindow.Location = Settings.Default.LogWindowLocation;
            logWindow.Size = Settings.Default.LogWindowSize;
            graphWindow.Location = Settings.Default.GraphWindowLocation;
            graphWindow.Size = Settings.Default.GraphWindowSize;
            predictorWindow.Location = Settings.Default.PredictorWindowLocation;
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.MainWindowLocation = this.Location;
            Settings.Default.MainWindowSize = this.Size;
            Settings.Default.MainWindowMaximized = (this.WindowState == FormWindowState.Maximized);
            Settings.Default.TelemetryWindowLocation = telemetryWindow.Location;
            Settings.Default.MapWindowLocation = mapWindow.Location;
            Settings.Default.MapWindowSize = mapWindow.Size;
            Settings.Default.LiveImageWindowLocation = liveImageWindow.Location;
            Settings.Default.LiveImageWindowSize = liveImageWindow.Size;
            Settings.Default.LogWindowLocation = logWindow.Location;
            Settings.Default.LogWindowSize = logWindow.Size;
            Settings.Default.GraphWindowLocation = graphWindow.Location;
            Settings.Default.GraphWindowSize = graphWindow.Size;
            Settings.Default.PredictorWindowLocation = predictorWindow.Location;
            Settings.Default.MapWindowVisible = mapWindow.Visible;
            Settings.Default.LogWindowVisible = logWindow.Visible;
            Settings.Default.LiveImageWindowVisible = liveImageWindow.Visible;
            Settings.Default.TelemetryWindowVisible = telemetryWindow.Visible;
            Settings.Default.PredictorWindowVisible = predictorWindow.Visible;
            Settings.Default.GraphWindowVisible = graphWindow.Visible;
            Settings.Default.Save();            
            transceiver.Stop();
            webAccess.Stop();
        }

        private void eventLogMenuItem_Click(object sender, EventArgs e)
        {
            if (logWindow.Visible)
            {
                logWindow.Hide();
                eventLogMenuItem.CheckState = CheckState.Unchecked;
            }
            else
            {
                logWindow.Show();
                eventLogMenuItem.CheckState = CheckState.Checked;
            }
        }        

        private void liveImageMenuItem_Click(object sender, EventArgs e)
        {
            if (liveImageWindow.Visible)
            {
                liveImageWindow.Hide();
                liveImageMenuItem.CheckState = CheckState.Unchecked;
            }
            else
            {
                liveImageWindow.Show();
                liveImageMenuItem.CheckState = CheckState.Checked;
            }
        }

        private void mapMenuItem_Click(object sender, EventArgs e)
        {
            if (mapWindow.Visible)
            {
                mapWindow.Hide();
                mapMenuItem.CheckState = CheckState.Unchecked;
            }
            else
            {
                mapWindow.Show();
                mapMenuItem.CheckState = CheckState.Checked;
            }
        }

        private void telemetryMenuItem_Click(object sender, EventArgs e)
        {
            if (telemetryWindow.Visible)
            {
                telemetryWindow.Hide();
                telemetryMenuItem.CheckState = CheckState.Unchecked;
            }
            else
            {
                telemetryWindow.Show();
                telemetryMenuItem.CheckState = CheckState.Checked;
            }
        }

        private void graphMenuItem_Click(object sender, EventArgs e)
        {
            if (graphWindow.Visible)
            {
                graphWindow.Hide();
                graphMenuItem.CheckState = CheckState.Unchecked;
            }
            else
            {
                graphWindow.Show();
                graphMenuItem.CheckState = CheckState.Checked;
            }
        }

        private void predictorMenuItem_Click(object sender, EventArgs e)
        {
            if (predictorWindow.Visible)
            {
                predictorWindow.Hide();
                predictorMenuItem.CheckState = CheckState.Unchecked;
            }
            else
            {
                predictorWindow.Show();
                predictorMenuItem.CheckState = CheckState.Checked;
            }
        }

        private void startCaptureMenuItem_Click(object sender, EventArgs e)
        {
            if (dataCache.Locked)
            {
                MessageBox.Show("Data cache is locked because old data was loaded. Clear data first.", "Data cache locked", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                // if data exists, continue capture in same file.
                if (dataCache.Size == 0)
                {
                    persistHandler.CreateTelemetryFile(DateTime.UtcNow);
                    filenameLbl.Text = persistHandler.TelemetryFileName;
                }
                if (Settings.Default.WebAccessEnabled)
                {
                    webAccess.Start();
                }
                transceiver.Start();
                startTime = DateTime.Now;
                stopCaptureMenuItem.Enabled = true;
                startCaptureMenuItem.Enabled = false;
                loadDataMenuItem.Enabled = false;
                loadPredictedMenuItem.Enabled = true;
                clearDataMenuItem.Enabled = false;
                preferencesMenuItem.Enabled = false;
                transceiverStateLbl.Text = "Started";
                elapsedTimer.Start();
                logWindow.WriteLine("Transceiver started on " + comPortRadio + ".");
            }
            catch (Exception ex)
            {
                DisplayExecption(ex);                
                transceiver.Stop();
                webAccess.Stop();
            }
        }

        private void stopCaptureMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                transceiver.Stop();
                webAccess.Stop();
                elapsedTimer.Stop();
                stopCaptureMenuItem.Enabled = false;
                startCaptureMenuItem.Enabled = true;
                loadDataMenuItem.Enabled = true;
                loadPredictedMenuItem.Enabled = true;
                clearDataMenuItem.Enabled = true;
                preferencesMenuItem.Enabled = true;
                transceiverStateLbl.Text = "Stopped";
                logWindow.WriteLine("Transceiver stopped.");
            }
            catch (Exception ex)
            {
                DisplayExecption(ex);
            }
        }

        private void clearDataMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Clear ALL the data?", "Clear data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                protocol.Reset();
                dataCache.Clear();
                telemetryWindow.Clear();
                mapWindow.Clear();
                graphWindow.Clear();
                filenameLbl.Text = "no file";
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void preferencesMenuItem_Click(object sender, EventArgs e)
        {
            PreferencesDialog dialog = new PreferencesDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                dataDirectory = dialog.DataDirectory;
                persistHandler.DataDirectory = dataDirectory;
                Settings.Default.DataDirectory = dataDirectory;
                
                comPortRadio = dialog.ComPortRadio;
                transceiver.SetPort(comPortRadio);
                Settings.Default.ComPortRadio = comPortRadio;

                comPortGPS = dialog.ComPortGPS;
                gpsReceiver.PortSettings.PortName = comPortGPS;
                Settings.Default.ComPortGPS = comPortGPS;

                Settings.Default.WebAccessUrl = dialog.WebAccessUrl;
                webAccess.Url = dialog.WebAccessUrl;
                if (!Settings.Default.WebAccessEnabled && dialog.WebAccessEnabled)
                {
                    protocol.TelemetryReceived += webAccess.UploadTelemetry;
                    protocol.ImageComplete += webAccess.UploadLiveImage;
                    gpsReceiver.NewFix += gpsUploadHandler;
                }
                else if (Settings.Default.WebAccessEnabled && !dialog.WebAccessEnabled)
                {
                    protocol.TelemetryReceived -= webAccess.UploadTelemetry;
                    protocol.ImageComplete -= webAccess.UploadLiveImage;
                    gpsReceiver.NewFix -= gpsUploadHandler;
                }
                Settings.Default.WebAccessEnabled = dialog.WebAccessEnabled;

                blogMessageMenuItem.Enabled = dialog.WebAccessEnabled;

                Settings.Default.Save();
            }
        }

        private void loadDataMenuItem_Click(object sender, EventArgs e)
        {
            if (dataCache.Size > 0)
            {
                DialogResult res = MessageBox.Show("Existing data will be deleted. Do you want to continue?", "Clear data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    dataCache.Clear();
                    mapWindow.Clear();
                    graphWindow.Clear();
                }
                else
                {
                    return;
                }
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSV files (*.csv)|*.csv";
            dialog.InitialDirectory = Settings.Default.DataDirectory + Path.PathSeparator + "telemetry";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (DataLoader.LoadTelemetryData(dialog.FileName, dataCache))
                {
                    persistHandler.CreateTelemetryFile(dialog.FileName);
                    mapWindow.LoadFromCache(dataCache);
                    if (dataCache.Size > 0)
                        telemetryWindow.DisplayTelemetry(dataCache.Telemetry.Last());
                    filenameLbl.Text = dialog.FileName;
                    graphWindow.LoadFromCache(dataCache);
                }
                else
                {
                    MessageBox.Show("Failed to load data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void loadPredictedMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "KML files (*.kml)|*.kml";
            dialog.InitialDirectory = Settings.Default.DataDirectory + Path.PathSeparator + "telemetry";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                LoadPrediction(dialog.FileName);
            }
        }

        private void connectGPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gpsReceiver.IsOpen)
            {
                gpsReceiver.Close();
                connectGPSToolStripMenuItem.Text = "Connect GPS";
                logWindow.WriteLine("GPS disconnected.");
            }
            try
            {
                gpsReceiver.Open();
                connectGPSToolStripMenuItem.Text = "Disconnect GPS";
                logWindow.WriteLine("GPS connected.");
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to open GPS serial port.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void blogMessageMenuItem_Click(object sender, EventArgs e)
        {
            BlogMessageDialog dialog = new BlogMessageDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                webAccess.PostBlog(DateTime.UtcNow, dialog.Message);
            }
        }

        private void LoadPrediction(string filename)
        {
            TextReader reader = File.OpenText(filename);
            KmlFile kml = KmlFile.Load(reader);
            reader.Close();

            List<PointLatLng> coords = new List<PointLatLng>();
            List<GMapMarker> markers = new List<GMapMarker>();
            foreach (var placemark in kml.Root.Flatten().OfType<SharpKml.Dom.Placemark>())
            {
                if (placemark.Name.Equals("Flight path"))
                {
                    foreach (var lstr in placemark.Flatten().OfType<LineString>())
                    {
                        foreach (var c in lstr.Coordinates)
                        {
                            coords.Add(new PointLatLng(c.Latitude, c.Longitude));
                        }
                    }
                }
                else if (placemark.Name.Equals("Balloon Launch"))
                {
                    SharpKml.Dom.Point point = placemark.Geometry as SharpKml.Dom.Point;
                    markers.Add(new GMapMarkerImage(new PointLatLng(point.Coordinate.Latitude, point.Coordinate.Longitude), Properties.Resources.Ascending, new System.Drawing.Point(-17, -43)) { ToolTipMode = MarkerTooltipMode.OnMouseOver, ToolTipText = placemark.Description.Text });
                }
                else if (placemark.Name.Equals("Balloon Burst"))
                {
                    SharpKml.Dom.Point point = placemark.Geometry as SharpKml.Dom.Point;
                    markers.Add(new GMapMarkerImage(new PointLatLng(point.Coordinate.Latitude, point.Coordinate.Longitude), Properties.Resources.Burst) { ToolTipMode = MarkerTooltipMode.OnMouseOver, ToolTipText = placemark.Description.Text });
                }
                else if (placemark.Name.Equals("Predicted Balloon Landing"))
                {
                    SharpKml.Dom.Point point = placemark.Geometry as SharpKml.Dom.Point;
                    markers.Add(new GMapMarkerImage(new PointLatLng(point.Coordinate.Latitude, point.Coordinate.Longitude), Properties.Resources.Descending, new System.Drawing.Point(-10, -27)) { ToolTipMode = MarkerTooltipMode.OnMouseOver, ToolTipText = placemark.Description.Text });
                }
            }

            if ((coords.Count > 0) && (markers.Count > 0))
            {
                mapWindow.LoadPredictedCourse(coords, markers);
            }
        }

        private void flightradar24ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapWindow.EnableFlightRadar24(flightradar24MenuItem.Checked);
        }

        private void exportGoogleEarthTourMenuItem_Click(object sender, EventArgs e)
        {
            if (dataCache.Telemetry.Count > 0)
            {
                ExportTourDialog dialog = new ExportTourDialog();
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    TourGenerator generator = new TourGenerator();
                    try
                    {
                        KmlFile kml = generator.createTour(dataCache.Telemetry, dialog.MinTimeDifference);
                        FileStream stream = File.OpenWrite(dialog.FileName);
                        kml.Save(stream);
                        stream.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to create tour:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("No flight data available.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
