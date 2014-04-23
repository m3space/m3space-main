using System;
using System.Drawing;
using System.Windows.Forms;

namespace TelemetryAnalyzer
{
    public partial class ImageViewer : UserControl
    {
        public delegate void ImageChangeDelegate(int index);
        public event ImageChangeDelegate ImageChanged;

        public ImageViewer()
        {
            InitializeComponent();
        }

        public Image Image { get { return pictureBox1.Image; } set { pictureBox1.Image = value; } }

        public int NumberOfFrames { set { trackBar1.Maximum = value; } }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            trackBar1.Value = Math.Max(trackBar1.Value - 1, trackBar1.Minimum);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            trackBar1.Value = Math.Min(trackBar1.Value + 1, trackBar1.Maximum);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (ImageChanged != null)
            {
                ImageChanged(trackBar1.Value);
            }
        }
    }
}
