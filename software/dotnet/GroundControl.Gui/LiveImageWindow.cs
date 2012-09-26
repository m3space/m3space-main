using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GroundControl.Gui
{
    public partial class LiveImageWindow : Form
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public LiveImageWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Updates the display with a new live image.
        /// </summary>
        /// <param name="utcTs">the UTC timestamp of the image</param>
        /// <param name="imgData">the image data</param>
        public void UpdateImage(DateTime utcTs, byte[] imgData)
        {
            dateLbl.Text = String.Format("{0:dd.MM.yyyy HH:mm:ss}", utcTs.ToLocalTime());

            MemoryStream stream = new MemoryStream(imgData);

            try
            {
                Image img = Image.FromStream(stream);
                imageBox.Image = img;
            }
            catch (Exception)
            {
                imageBox.Image = null;
            }
        }
    }
}
