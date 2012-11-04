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

        private PersistenceHandler persistHandler;
        private DataTransceiver transceiver;
        private DataProtocol protocol;
        private DataCache dataCache;
        private WebAccess webAccess;

        private NMEA.GPSReceiverWrapper gpsReceiver;

        private DateTime startTime;
        private Timer elapsedTimer;

        private string comPortRadio;
        private string comPortGPS;
        private string dataDirectory;

        private delegate void WriteString(string str);
        private delegate void VoidDelegate();
        private delegate void PositionDelegate(double latitude, double longitude);

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

            if (Settings.Default.DataDirectory.Equals(""))
            {
                Settings.Default.DataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar + "m3space";
            }
            dataDirectory = Settings.Default.DataDirectory;

            comPortGPS = Settings.Default.ComPortGPS;
            comPortRadio = Settings.Default.ComPortRadio;

            graphWindow.Show();
            logWindow.Show();
            liveImageWindow.Show();
            telemetryWindow.Show();
            mapWindow.Show();

            webAccess = new WebAccess();
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

            gpsReceiver = new GPSReceiverWrapper();
            gpsReceiver.PortSettings = new SerialPortSettings(comPortGPS, BaudRate.baudRate57600, System.IO.Ports.Parity.None, DataBits.dataBits8, System.IO.Ports.StopBits.One, System.IO.Ports.Handshake.None);
            gpsReceiver.NewFix += new GPSReceiverWrapper.NewFixHandler(gpsReceiver_NewFix);

            webAccess.Url = Settings.Default.WebAccessUrl;
            if (Settings.Default.WebAccessEnabled)
            {
                protocol.TelemetryReceived += webAccess.UploadTelemetry;
                protocol.ImageComplete += webAccess.UploadLiveImage;
            }

            elapsedTimer = new Timer();
            elapsedTimer.Interval = 1000;
            elapsedTimer.Tick += ElapsedTick;

            RestoreSettings();
        }

        private void gpsReceiver_NewFix(DateTime fixTime, GeographicDimension latitude, GeographicDimension longitude)
        {
            mapWindow.Invoke(new PositionDelegate(mapWindow.UpdateGroundPosition), new object[] { latitude.Angle, longitude.Angle });
        }

        /// <summary>
        /// Handles received telemetry.
        /// </summary>
        /// <param name="data">the telemetry data</param>
        private void HandleTelemetry(TelemetryData data)
        {
            dataCache.AddTelemetry(data);

            // convert GPS position to decimal minutes
            float latAbs = Math.Abs(data.Latitude);
            int latDegs = (int)latAbs;
            float latDecMins = (latAbs - latDegs) * 60;
            char latOri = (data.Latitude >= 0.0f) ? 'N' : 'S';

            float lngAbs = Math.Abs(data.Longitude);
            int lngDegs = (int)lngAbs;
            float lngDecMins = (lngAbs - lngDegs) * 60;
            char lngOri = (data.Latitude >= 0.0f) ? 'E' : 'W';

            string str = String.Format("[Telemetry] {0:dd.MM.yyyy HH:mm:ss} Loc:{1}°{2:0.###}'{3} {4}°{5:0.###}'{6} Alt:{7:0.#}m PAlt:{8:0.#}m Head:{9:0.#}° HSpd:{10:0.#}m/s VSpd:{11:0.#}m/s Sat:{12} Int:{13:0.#}°C Ext:{14:0.#}°C P:{15:0.####}bar Vin:{16:0.##}V Duty:{17}%",
                data.UtcTimestamp.ToLocalTime(),
                latDegs,
                latDecMins,
                latOri,
                lngDegs,
                lngDecMins,
                lngOri,
                data.GpsAltitude,
                data.PressureAltitude,
                data.Heading * Rad2Deg,
                data.HorizontalSpeed,
                data.VerticalSpeed,
                data.Satellites,
                data.IntTemperature,
                data.ExtTemperature,
                data.Pressure,
                data.Vin,
                data.DutyCycle);
            logWindow.Invoke(new WriteString(logWindow.WriteLine), new object[] { str });
            telemetryWindow.Invoke(new TelemetryHandler(telemetryWindow.DisplayTelemetry), new object[] { data });
            mapWindow.Invoke(new TelemetryHandler(mapWindow.AddTelemetryPoint), new object[] { data });
            graphWindow.Invoke(new TelemetryHandler(graphWindow.AddTelemetry), new object[] { data });
        }

        /// <summary>
        /// Handles received images.
        /// </summary>
        /// <param name="utcTs">the UTC timestamp</param>
        /// <param name="data">the image data</param>
        private void HandleImage(DateTime utcTs, byte[] data)
        {
            string str = String.Format("[Image] {0:yyyy/MM/dd HH:mm:ss} Size: {1} bytes", utcTs.ToLocalTime(), data.Length);
            logWindow.Invoke(new WriteString(logWindow.WriteLine), new object[] { str });
            liveImageWindow.Invoke(new ImageCompleteHandler(liveImageWindow.UpdateImage), new object[] { utcTs, data });
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
            Settings.Default.Save();
            transceiver.Stop();
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

        private void startCaptureMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // if data exists, continue capture in same file.
                if (dataCache.Size == 0)
                {
                    persistHandler.CreateTelemetryFile(DateTime.UtcNow);
                    filenameLbl.Text = persistHandler.TelemetryFileName;
                }
                transceiver.Start();
                startTime = DateTime.Now;
                stopCaptureMenuItem.Enabled = true;
                startCaptureMenuItem.Enabled = false;
                loadDataMenuItem.Enabled = false;
                loadPredictedMenuItem.Enabled = false;
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
            }
        }

        private void stopCaptureMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                transceiver.Stop();
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
                dataCache.Clear();
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

                Settings.Default.WebAccessUrl = dialog.WebAccessUrl;
                webAccess.Url = dialog.WebAccessUrl;
                if (!Settings.Default.WebAccessEnabled && dialog.WebAccessEnabled)
                {
                    protocol.TelemetryReceived += webAccess.UploadTelemetry;
                    protocol.ImageComplete += webAccess.UploadLiveImage;
                }
                else if (Settings.Default.WebAccessEnabled && !dialog.WebAccessEnabled)
                {
                    protocol.TelemetryReceived -= webAccess.UploadTelemetry;
                    protocol.ImageComplete -= webAccess.UploadLiveImage;
                }
                Settings.Default.WebAccessEnabled = dialog.WebAccessEnabled;

                comPortGPS = dialog.ComPortGPS;
                gpsReceiver.PortSettings.PortName = comPortGPS;
                Settings.Default.ComPortGPS = comPortGPS;

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
                // TODO find a way to load CUSF landing predictor files
                KmlFile kml = KmlFile.Load(dialog.FileName);

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
                        foreach (var point in placemark.Flatten().OfType<SharpKml.Dom.Point>())
                        {
                            markers.Add(new GMapMarkerImage(new PointLatLng(point.Coordinate.Latitude, point.Coordinate.Longitude), Properties.Resources.Ascending));
                        }
                    }
                    else if (placemark.Name.Equals("Balloon Burst"))
                    {
                        foreach (var point in placemark.Flatten().OfType<SharpKml.Dom.Point>())
                        {
                            markers.Add(new GMapMarkerImage(new PointLatLng(point.Coordinate.Latitude, point.Coordinate.Longitude), Properties.Resources.Descending));
                        }                        
                    }
                    else if (placemark.Name.Equals("Predicted Balloon Landing"))
                    {
                        foreach (var point in placemark.Flatten().OfType<SharpKml.Dom.Point>())
                        {
                            markers.Add(new GMapMarkerImage(new PointLatLng(point.Coordinate.Latitude, point.Coordinate.Longitude), Properties.Resources.Descending));
                        }
                    }
                }

                if ((coords.Count > 0) && (markers.Count > 0))
                {
                    mapWindow.LoadPredictedCourse(coords, markers);
                }
            }
        }

        private void connectGPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gpsReceiver.IsOpen)
            {
                gpsReceiver.Close();
            }
            gpsReceiver.Open();
        }
    }
}
