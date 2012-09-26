using System;
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
        private GMapControl map;
        private GMapOverlay balloonOverlay;
        private GMapOverlay predictionOverlay;

        private GMapMarker balloonMarker;
        private GMapRoute balloonCourse;

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

            map.Overlays.Add(balloonOverlay);
            map.Overlays.Add(predictionOverlay);

            balloonCourse = new GMapRoute(new List<PointLatLng>(), "BalloonCourse");
            balloonCourse.Stroke = new Pen(Color.DarkBlue, 2.0f);

            balloonMarker = new GMapMarkerGoogleRed(map.Position);
            balloonOverlay.Markers.Add(balloonMarker);
            balloonOverlay.Routes.Add(balloonCourse);

            mapTypeDropDown.SelectedIndex = 0;

            this.Controls.Add(map);
        }

        public void AddTelemetryPoint(TelemetryData data)
        {
            PointLatLng mapPoint = new PointLatLng(data.Latitude, data.Longitude);
            balloonCourse.Points.Add(mapPoint);
            balloonMarker.Position = mapPoint;
            map.Position = mapPoint;
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
            route.Stroke = new Pen(Color.Black, 2.0f);
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
    }
}
