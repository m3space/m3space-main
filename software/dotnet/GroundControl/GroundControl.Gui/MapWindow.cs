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
        //private GMapControl map;
        private GMapOverlay balloonOverlay;
        private GMapOverlay predictionOverlay;
        private GMapOverlay groundControlOverlay;
        private GMapOverlay routeOverlay;

        private GMapMarkerImage balloonMarker;
        private GMapRoute balloonCourse;

        private GMapMarkerImage groundControlMarker;
        private GMapMarkerImage burstMarker;

        private FlightRadar24 flightRadar24;

        public PointLatLng MapPosition
        {
            get { return map.Position; }
        }

        public MapWindow()
        {
            InitializeComponent();

            map.DragButton = MouseButtons.Right;
            map.Manager.Mode = AccessMode.ServerAndCache;
            map.MapProvider = GMapProviders.GoogleMap;
            map.Position = new PointLatLng(47.558119, 7.587800);

            balloonOverlay = new GMapOverlay("Balloon");
            predictionOverlay = new GMapOverlay("Prediction");
            groundControlOverlay = new GMapOverlay("GroundControl");
            routeOverlay = new GMapOverlay("Route");

            map.Overlays.Add(balloonOverlay);
            map.Overlays.Add(predictionOverlay);
            map.Overlays.Add(groundControlOverlay);
            map.Overlays.Add(routeOverlay);

            balloonCourse = new GMapRoute(new List<PointLatLng>(), "BalloonCourse");
            balloonCourse.Stroke = new Pen(Color.Blue, 2.0f);
            balloonMarker = new GMapMarkerImage(map.Position, Properties.Resources.Ascending, new Point(-17, -43));
            balloonOverlay.Markers.Add(balloonMarker);
            balloonOverlay.Routes.Add(balloonCourse);

            groundControlMarker = new GMapMarkerImage(map.Position, Properties.Resources.Receiver, new Point(-10, -27));
            groundControlOverlay.Markers.Add(groundControlMarker);

            burstMarker = null;

            mapTypeDropDown.SelectedIndex = 0;

            flightRadar24 = new FlightRadar24(map);

            //this.Controls.Add(map);
        }

        public void AddTelemetryPoint(TelemetryData data, bool burst)
        {
            PointLatLng mapPoint = new PointLatLng(data.Latitude, data.Longitude);
            balloonCourse.Points.Add(mapPoint);
            balloonMarker.Position = mapPoint;
            map.Position = mapPoint;

            // detect burst
            if (burst && (burstMarker == null))
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
            if (burstMarker != null)
            {
                balloonOverlay.Markers.Remove(burstMarker);
                burstMarker = null;
            }
            balloonMarker.MarkerImage = Properties.Resources.Ascending;
            balloonMarker.Offset = new Point(-17, -43);
            map.ReloadMap();
        }

        public void LoadFromCache(DataCache dataCache)
        {
            PointLatLng mapPoint = PointLatLng.Empty;

            foreach (TelemetryData data in dataCache.Telemetry)
            {
                mapPoint = new PointLatLng(data.Latitude, data.Longitude);
                balloonCourse.Points.Add(mapPoint);
            }

            if (dataCache.Size > 0)
            {
                int burstIdx = Utils.FindBurstIndex(dataCache.Telemetry);
                if (burstIdx >= 0)
                {
                    PointLatLng burstPoint = new PointLatLng(dataCache.Telemetry[burstIdx].Latitude, dataCache.Telemetry[burstIdx].Longitude);
                    burstMarker = new GMapMarkerImage(burstPoint, Properties.Resources.Burst);
                    balloonOverlay.Markers.Add(burstMarker);
                    balloonMarker.MarkerImage = Properties.Resources.Descending;
                    balloonMarker.Offset = new Point(-10, -25);
                }                
                balloonMarker.Position = mapPoint;
                map.Position = mapPoint;
            }
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
                case "Route":
                    routeOverlay.IsVisibile = layer.Checked;
                    break;
            }
            map.Refresh();
        }

        private void alongPredictionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (predictionOverlay.Routes.Count == 0)
            {
                MessageBox.Show("Load prediction file before calculating route!", "No prediction file", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            PointLatLng start = predictionOverlay.Markers.First().Position;
            PointLatLng end = predictionOverlay.Markers.Last().Position;
            MapRoute route = GMapProviders.GoogleMap.GetRoute(start, end, false, false, 10);
            routeOverlay.Routes.Clear();
            routeOverlay.Routes.Add(new GMapRoute(route.Points, "Route"));
        }

        private void toBalloonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PointLatLng start = groundControlMarker.Position;
            PointLatLng end = balloonMarker.Position;
            MapRoute route = GMapProviders.GoogleMap.GetRoute(start, end, false, false, 10);
            routeOverlay.Routes.Clear();
            routeOverlay.Routes.Add(new GMapRoute(route.Points, "Route"));

            //GDirections s;
            //var result = GMapProviders.GoogleMap.GetDirections(out s, start, end, false, false, false, false, true);
            //if (result == DirectionsStatusCode.OK)
            //{
            //    Console.WriteLine(s.Summary + ", " + s.Copyrights);
            //    Console.WriteLine(s.StartAddress + " -> " + s.EndAddress);
            //    Console.WriteLine(s.Distance);
            //    Console.WriteLine(s.Duration);

            //    foreach (var step in s.Steps)
            //    {
            //        Console.WriteLine(step);
            //    }
            //}
        }
        public event GMap.NET.PositionChanged MapPositionChanged;
        private void map_OnPositionChanged(PointLatLng point)
        {
            if (MapPositionChanged != null)
            {
                MapPositionChanged.Invoke(point);
            }
        }

        internal void EnableFlightRadar24(bool enable)
        {
            if (enable)
            {
                flightRadar24.Start();
            }
            else
            {
                flightRadar24.Stop();
            }
        }
    }
}
