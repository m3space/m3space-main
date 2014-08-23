using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GroundControl.Gui
{
    public partial class ExportTourDialog : Form
    {
        public String FileName { get { return fileNameTxt.Text; } }

        public double MinTimeDifference
        {
            get
            {
                double t = 10.0;
                Double.TryParse(minDeltaTxt.Text, out t);
                return t;
            }
        }

        public double Speed
        {
            get
            {
                double s = 10.0;
                Double.TryParse(speedTxt.Text, out s);
                return s;
            }
        }


        public ExportTourDialog()
        {
            InitializeComponent();
        }

        private void selectBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "KML files (*.kml)|*.kml";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                fileNameTxt.Text = dialog.FileName;
            }
        }
    }
}
