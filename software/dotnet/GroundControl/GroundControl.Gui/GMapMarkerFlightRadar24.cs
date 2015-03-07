using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using GMap.NET;

namespace GroundControl.Gui
{
    public class GMapMarkerFlightRadar24 : GMap.NET.WindowsForms.GMapMarker
    {
        private const int FontSize = 8;

        private FlightRadar24Data m_flightRadarData;

        public FlightRadar24Data FlightRadarData
        {
            get
            {
                return m_flightRadarData;
            }
            set
            {
                m_flightRadarData = value;
                Position = m_flightRadarData.Position;
            }
        }

        public GMapMarkerFlightRadar24(PointLatLng p) : base(p)
        {
        }

        private Bitmap RotateImage(Bitmap b, float angle)
        {
            using (Graphics graphics = Graphics.FromImage(b))
            {
                Bitmap org = (Bitmap)b.Clone();
                graphics.Clear(Color.Transparent);
                graphics.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
                graphics.RotateTransform(angle);
                graphics.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
                graphics.DrawImage(org, new Point(0, 0));
            }
            return b;
        }

        public override void OnRender(Graphics g)
        {
            // draw an icon and aircraft info
            Bitmap aircraftIcon = RotateImage(Properties.Resources.Airplane, m_flightRadarData.Heading);
            String text = string.Format("{0}, {1}\n{2}m, {3}km/h", m_flightRadarData.AircraftReg, m_flightRadarData.AircraftType, m_flightRadarData.Altitude, m_flightRadarData.Speed);

            PointF textPos = new PointF(LocalPosition.X + 5, LocalPosition.Y + 15);
            PointF linePos = new PointF(LocalPosition.X + 0, LocalPosition.Y + 5);
            PointF iconPos = new PointF(LocalPosition.X - aircraftIcon.Width / 2, LocalPosition.Y - aircraftIcon.Height / 2);

            GraphicsPath p = new GraphicsPath();
            p.AddString(text, FontFamily.GenericSansSerif, (int)FontStyle.Bold, FontSize * g.DpiY / 72 , textPos, new StringFormat());
            p.AddLine(linePos, textPos);
            g.DrawPath(new Pen(Color.Yellow, 1.0f), p);
            g.FillPath(new SolidBrush(Color.Black), p);
            g.DrawImage(aircraftIcon, iconPos);
        }
    }
}
