using System;
using System.Drawing;
using System.Windows.Forms;

namespace VideoPostProcess
{
    public partial class MainWindow : Form
    {
        private VideoProcessor m_videoProcessor;

        public MainWindow()
        {
            InitializeComponent();
            m_videoProcessor = new VideoProcessor();
            m_videoProcessor.UpdateProgress += new VideoProcessor.UpdateProgressEventHandler(UpdateProgress);
            propertyGrid1.SelectedObject = m_videoProcessor;
            this.panel1.Controls.Add(m_videoProcessor.MapOverlay);
        }

        private void UpdateProgress(object sender, UpdateProgressEventArgs e)
        {
            Invoke(new MethodInvoker(delegate
            {
                lblVideo.Text = String.Format("{0} / {1}", e.Frame.VideoId, m_videoProcessor.NrOfVideos);
                lblFrame.Text = String.Format("{0} / {1}", e.Frame.FrameId, m_videoProcessor.NrOfFrames);
                lblTimeElapsed.Text = String.Format("{0:00}:{1:00}:{2:00}", e.TimeElapsed.Hours, e.TimeElapsed.Minutes, e.TimeElapsed.Seconds);
                lblTimeRemaining.Text = String.Format("{0:00}:{1:00}:{2:00}", e.TimeRemaining.Hours, e.TimeRemaining.Minutes, e.TimeRemaining.Seconds);
                progressBar1.Value = e.Frame.FrameId;
                if (e.Frame.Image != null)
                    pboxVideo.Image = e.Frame.Image;
                else if (pboxVideo.Image != null)
                    Graphics.FromImage(pboxVideo.Image).Clear(pboxVideo.BackColor);
            }));
        }

        private void Cancel()
        {
            m_videoProcessor.Cancel();
            btnCancel.Enabled = false;
            btnStart.Enabled = true;
            btnSelectFolder.Enabled = true;
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                m_videoProcessor.Init(openFileDialog1.FileNames);
                progressBar1.Maximum = (int)m_videoProcessor.NrOfFrames;
                propertyGrid1.Refresh();
                if (m_videoProcessor.Ready)
                {
                    btnStart.Enabled = true;
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnCancel.Enabled = true;
            btnSelectFolder.Enabled = false;
            btnStart.Enabled = false;
            m_videoProcessor.Start();
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cancel();
        }
    }
}
