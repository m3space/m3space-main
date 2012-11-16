﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET.WindowsForms;
using GMap.NET.MapProviders;
using GMap.NET;
using GroundControl.Core;
using GMap.NET.WindowsForms.Markers;

namespace GroundControl.Gui
{
    public partial class MapWindow : Form
    {
        /// <summary>
        /// Minimal descending speed to detect burst (m/s).
        /// </summary>
        const float BurstSpeed = -15.0f;

        private GMapControl map;
        private GMapOverlay balloonOverlay;
        private GMapOverlay predictionOverlay;
        private GMapOverlay groundControlOverlay;

        private GMapMarkerImage balloonMarker;
        private GMapRoute balloonCourse;

        private GMapMarkerImage groundControlMarker;
        private GMapMarkerImage burstMarker;

        public MapWindow()
        {
            InitializeComponent();

            map = new GMapControl();
            map.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            map.Dock = DockStyle.Fill;
            map.CanDragMap = true;
            map.DragButton = MouseButtons.Right;
            map.Manager.Mode = AccessMode.ServerAndCache;
            map.CanDragMap = true;
            map.MarkersEnabled = true;
            map.PolygonsEnabled = true;
            map.MinZoom = 0;
            map.MaxZoom = 24;
            map.MapProvider = GMapProviders.GoogleMap;

            map.Zoom = 15;            
            map.Position = new PointLatLng(47.558119, 7.587800);

            balloonOverlay = new GMapOverlay(map, "Balloon");
            predictionOverlay = new GMapOverlay(map, "Prediction");
            groundControlOverlay = new GMapOverlay(map, "GroundControl");
            

            map.Overlays.Add(balloonOverlay);
            map.Overlays.Add(predictionOverlay);
            map.Overlays.Add(groundControlOverlay);
            

            balloonCourse = new GMapRoute(new List<PointLatLng>(), "BalloonCourse");
            balloonCourse.Stroke = new Pen(Color.Blue, 2.0f);

            balloonMarker = new GMapMarkerImage(map.Position, Properties.Resources.Ascending, new Point(-17, -43));
            balloonOverlay.Markers.Add(balloonMarker);
            balloonOverlay.Routes.Add(balloonCourse);

            groundControlMarker = new GMapMarkerImage(map.Position, Properties.Resources.Receiver, new Point(-10, -27));
            groundControlOverlay.Markers.Add(groundControlMarker);

            burstMarker = null;

            mapTypeDropDown.SelectedIndex = 0;

            this.Controls.Add(map);
        }

        public void AddTelemetryPoint(TelemetryData data)
        {
            PointLatLng mapPoint = new PointLatLng(data.Latitude, data.Longitude);
            balloonCourse.Points.Add(mapPoint);
            balloonMarker.Position = mapPoint;
            map.Position = mapPoint;

            // detect burst
            if ((burstMarker == null) && (data.VerticalSpeed < BurstSpeed))
            {
                burstMarker = new GMapMarkerImage(mapPoint, Properties.Resources.Burst);
                balloonOverlay.Markers.Add(burstMarker);
                balloonMarker.MarkerImage = Properties.Resources.Descending;
                balloonMarker.Offset = new Point(-10, -25);
            }
        }

        public void UpdateGroundPosition(double latitude, double longitude)
        {
            groundControlMarker.Position = new PointLatLng(latitude, longitude);
        }

        public void Clear()
        {
            balloonCourse.Points.Clear();
            predictionOverlay.Routes.Clear();
            predictionOverlay.Markers.Clear();
            map.ReloadMap();
        }

        public void LoadFromCache(DataCache dataCache)
        {
            PointLatLng mapPoint = PointLatLng.Zero;

            foreach (TelemetryData data in dataCache.Telemetry)
            {
                mapPoint = new PointLatLng(data.Latitude, data.Longitude);
                balloonCourse.Points.Add(mapPoint);
                balloonMarker.Position = mapPoint;
            }            
            
            if (dataCache.Size > 0)
                map.Position = mapPoint;
        }

        public void LoadPredictedCourse(List<PointLatLng> points, List<GMapMarker> markers)
        {
            predictionOverlay.Routes.Clear();
            predictionOverlay.Markers.Clear();
            GMapRoute route = new GMapRoute(points, "PredictedCourse");
            route.Stroke = new Pen(Color.Fuchsia, 2.0f);
            predictionOverlay.Routes.Add(route);
            foreach (GMapMarker marker in markers)
                predictionOverlay.Markers.Add(marker);
        }

        private void mapTypeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (mapTypeDropDown.SelectedIndex)
            {
                case 0:
                    map.MapProvider = GMapProviders.GoogleMap;
                    break;

                case 1:
                    map.MapProvider = GMapProviders.GoogleHybridMap;
                    break;

                case 2:
                    map.MapProvider = GMapProviders.GoogleTerrainMap;
                    break;

                default:
                    map.MapProvider = GMapProviders.GoogleMap;
                    break;
            }
        }

        private void layerToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem layer = (System.Windows.Forms.ToolStripMenuItem)sender;

            switch (layer.Text)
            {
                case "Prediction":
                    predictionOverlay.IsVisibile = layer.Checked;
                    break;
                case "Balloon":
                    balloonOverlay.IsVisibile = layer.Checked;
                    break;
                case "GroundControl":
                    groundControlOverlay.IsVisibile = layer.Checked;
                    break;
            }
            map.Refresh();
        }
    }
}
