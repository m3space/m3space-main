using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GroundControl.Core;
using TelemetryAnalyzer.Properties;

namespace TelemetryAnalyzer
{
    public partial class Form1 : Form
    {
        private MissionManager m_missionManager;
        private ImportMissionDialog m_importDialog = new ImportMissionDialog();

        public Form1()
        {
            InitializeComponent();
            SetCellFormat();

            m_missionManager = new MissionManager(m_map);

            //videoSourcePlayer1.VideoSource = new AForge.Video.FFMPEG.VideoFileSource(@"W:\Mission 4\Gopro Videos\GOPR2235.MP4");
            //videoSourcePlayer1.Start();
            OverviewDataTable.DataSource = m_missionManager;

            MapTypeDropDown.Items.AddRange(GMapProviders.List.ToArray());
            MapTypeDropDown.SelectedItem = GMapProviders.BingSatelliteMap;

            m_map.DragButton = MouseButtons.Right;
            m_map.Manager.Mode = AccessMode.ServerAndCache;
            m_map.SetZoomToFitRect(new RectLatLng(47.5, 7, 1, 1));
        }

        private void SetCellFormat()
        {
            startDateDataGridViewTextBoxColumn.Frozen = true;
            startDateDataGridViewTextBoxColumn.DefaultCellStyle.Format = "dd.MM.yyyy hh:mm:ss";
            flightTimeDataGridViewTextBoxColumn.DefaultCellStyle.Format = @"hh\:mm\:ss";
            timeToBurstDataGridViewTextBoxColumn.DefaultCellStyle.Format = @"hh\:mm\:ss";
            flightDistanceDataGridViewTextBoxColumn.DefaultCellStyle.Format = "N0";
            launchLandingDistanceDataGridViewTextBoxColumn.DefaultCellStyle.Format = "N0";
            maxAltitudeDataGridViewTextBoxColumn.DefaultCellStyle.Format = "N0";
            maxHSpeedDataGridViewTextBoxColumn.DefaultCellStyle.Format = "N0";
            maxVSpeedDataGridViewTextBoxColumn.DefaultCellStyle.Format = "N0";
            avgAscentRateDataGridViewTextBoxColumn.DefaultCellStyle.Format = "N2";
            avgDescentRateDataGridViewTextBoxColumn.DefaultCellStyle.Format = "N2";
            coldestTemperatureDataGridViewTextBoxColumn.DefaultCellStyle.Format = "N1";
            burstTemperatureDataGridViewTextBoxColumn.DefaultCellStyle.Format = "N1";
            batteryAtLandingDataGridViewTextBoxColumn.DefaultCellStyle.Format = "N2";
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (m_importDialog.ShowDialog() == DialogResult.OK)
            {
                Mission m = Mission.FromFile(m_importDialog.MissionName, m_importDialog.TelemetryFile, m_importDialog.ImagesPath, m_importDialog.VideoPath);
                if (m != null)
                {
                    m_missionManager.Add(m);
                }
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            splitMissions.SplitterDistance = Settings.Default.SplitterMissionsPosition;
            splitMap.SplitterDistance = Settings.Default.SplitterMapPosition;
            splitData.SplitterDistance = Settings.Default.SplitterDataPosition;
            splitGraph.SplitterDistance = Settings.Default.SplitterGraphPosition;
            splitVideo.SplitterDistance = Settings.Default.SplitterVideoPosition;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.SplitterMissionsPosition = splitMissions.SplitterDistance;
            Settings.Default.SplitterMapPosition = splitMap.SplitterDistance;
            Settings.Default.SplitterDataPosition = splitData.SplitterDistance;
            Settings.Default.SplitterGraphPosition = splitGraph.SplitterDistance;
            Settings.Default.SplitterVideoPosition = splitVideo.SplitterDistance;
            Settings.Default.Save();
        }

        private void MapTypeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_map.MapProvider = MapTypeDropDown.SelectedItem as GMapProvider;
        }

        private void OverviewDataTable_SelectionChanged(object sender, EventArgs e)
        {
            Mission m = (OverviewDataTable.DataSource as BindingSource).Current as Mission;
            TelemetryDataTable.DataSource = m.Flight;
            imageViewer1.NumberOfFrames = m.NumberOfImages;
            imageViewer2.NumberOfFrames = m.NumberOfVideoFrames;
            m_missionManager.SelectMission(m);
        }

        private void TelemetryDataTable_SelectionChanged(object sender, EventArgs e)
        {
            if (TelemetryDataTable.SelectedColumns.Count > 0)
            {
                var data = TelemetryDataTable.SelectedColumns[0];

            }

            if (TelemetryDataTable.CurrentRow != null)
            {
                TelemetryData data = (TelemetryDataTable.CurrentRow.DataBoundItem as TelemetryData);
                m_missionManager.SetCurrentMarker(data);

                if (chkboxSyncMode.Checked)
                {
                    Image image = m_missionManager.GetImage(data.UtcTimestamp);
                    if (image != null)
                    {
                        imageViewer1.Image = image;
                    }
                    Image videoFrame = m_missionManager.GetVideoFrame(data.UtcTimestamp);
                    if (videoFrame != null)
                    {
                        imageViewer2.Image = videoFrame;
                    }
                }
            }
        }

        private void imageViewer1_ImageChanged(int index)
        {
            Image image = m_missionManager.ImageAtIndex(index);
            if (image != null)
            {
                imageViewer1.Image = image;
            }
        }

        private void imageViewer2_ImageChanged(int index)
        {
            Image image = m_missionManager.VideoFrameAtIndex(index);
            if (image != null)
            {
                imageViewer2.Image = image;
            }
        }

        private void splitGraph_Panel2_Paint(object sender, PaintEventArgs e)
        {
            //TODO: draw selected graph
        }
    }
}
