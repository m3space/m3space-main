using System;
using System.Drawing;
using System.Windows.Forms;
using GroundControl.Core;

namespace VideoPostProcess
{
    public partial class TelemetryOverlay : UserControl
    {
        public string MissionName
        {
            get { return lblMissionName.Text; }
            set
            {
                lblMissionName.Text = value;
            }
        }

        public TelemetryOverlay()
        {
            InitializeComponent();
        }

        public void DrawOverlay(Bitmap videoFrame, TelemetryData data, TimeSpan missionTime)
        {
            // Track
            lblSpeedH.Text = String.Format("{0:0.0}km/h", data.HorizontalSpeed * 3.6);
            lblSpeedV.Text = String.Format("{0:0.0}m/s", data.VerticalSpeed);
            lblAlt.Text = String.Format("{0:0}m", data.GpsAltitude);
            lblHead.Text = String.Format("{0:000}°", data.Heading);

            // Temperature
            lblTin.Text = String.Format("{0:#.0}°C", data.Temperature2);
            lblTout.Text = String.Format("{0:#.0}°C", data.Temperature1);

            // Capsule Data
            lblBat.Text = String.Format("{0:0.##}V", data.Vin);
            lblDuty.Text = String.Format("{0}%", data.DutyCycle);
            lblSat.Text = String.Format("{0}", data.Satellites);
            lblLat.Text = String.Format("{0:0.00000} {1}", data.Latitude, data.Latitude >= 0.0f ? 'N' : 'S');
            lblLong.Text = String.Format("{0:0.00000} {1}", data.Longitude, data.Latitude >= 0.0f ? 'E' : 'W');

            // Time
            lblDate.Text = String.Format("{0:dd.MM.yyyy}", data.UtcTimestamp.ToLocalTime());
            lblTime.Text = String.Format("{0:HH:mm:ss}", data.UtcTimestamp.ToLocalTime());
            lblMissionTime.Text = String.Format("{0}{1:00}:{2:00}:{3:00}", (missionTime < TimeSpan.Zero) ? "-" : "", Math.Abs(missionTime.Hours), Math.Abs(missionTime.Minutes), Math.Abs(missionTime.Seconds));
            
            // draw overlay
            this.DrawToBitmap(videoFrame, new Rectangle(0, 0, this.Width, this.Height));
        }
    }
}
