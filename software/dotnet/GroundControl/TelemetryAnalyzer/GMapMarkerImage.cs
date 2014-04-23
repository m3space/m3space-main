using System.Drawing;
using GMap.NET;

namespace TelemetryAnalyzer
{
    public class GMapMarkerImage : GMap.NET.WindowsForms.GMapMarker
    {
        private Image img;
        
        /// <summary>
        /// The image to display as a marker.
        /// </summary>
        public Image MarkerImage
        {
            get
            {
                return img;
            }
            set
            {
                img = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="p">The position of the marker</param>
        /// <param name="image">the marker image</param>
        /// <param name="offset">the position offset</param>
        public GMapMarkerImage(PointLatLng p, Image image, Point offset)
            : base(p)
        {
            img = image;
            Size = img.Size;
            Offset = offset;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="p">The position of the marker</param>
        /// <param name="image">the marker image</param>
        public GMapMarkerImage(PointLatLng p, Image image)
            : this(p, image, new Point(-image.Size.Width / 2, -image.Size.Height / 2))
        {
        }

        public override void OnRender(Graphics g)
        {
            g.DrawImage(img, LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
        }
    }
}
