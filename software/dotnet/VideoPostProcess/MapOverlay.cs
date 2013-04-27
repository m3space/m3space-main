using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;

namespace VideoPostProcess
{
    public partial class MapOverlay : UserControl
    {
        public MapOverlay()
        {
            InitializeComponent();
            gMapControl1.MapProvider  = GMapProviders.GoogleSatelliteMap;
            gMapControl1.Manager.Mode = AccessMode.ServerAndCache;
            gMapControl1.Position = new PointLatLng(47.558119, 7.587800);
            gMapControl1.ScaleMode = ScaleModes.Fractional;
        }

        public void DrawOverlay(Bitmap videoFrame, float latitude, float longitude)
        {
            Invoke(new MethodInvoker(delegate
            {
                PointLatLng pointLatLng = new PointLatLng(latitude, longitude);
                gMapControl1.Position = pointLatLng;
                this.DrawToBitmap(videoFrame, new Rectangle(0, 680, this.Width, this.Height));
            }));
        }

        public double Zoom 
        {
            get { return gMapControl1.Zoom; }
            set { gMapControl1.Zoom = value; } 
        }
    }
}
