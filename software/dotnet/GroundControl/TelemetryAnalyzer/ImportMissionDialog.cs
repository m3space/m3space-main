using System;
using System.Windows.Forms;

namespace TelemetryAnalyzer
{
    public partial class ImportMissionDialog : Form
    {
        public string TelemetryFile
        {
            get { return tboxTelemetryFile.Text; }
        }

        public string ImagesPath
        {
            get { return tboxImagePath.Text; }
        }

        public string VideoPath
        {
            get { return tboxImagePath.Text; }
        }

        public string MissionName
        {
            get { return tboxMissionName.Text; }
        }

        public ImportMissionDialog()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnSelectTelemetryFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tboxTelemetryFile.Text = openFileDialog1.FileName;
            }
        }

        private void btnSelectImagePath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tboxImagePath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnSelectVideoPath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tboxVideoPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
