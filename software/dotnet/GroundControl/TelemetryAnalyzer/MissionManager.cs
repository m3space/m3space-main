using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GroundControl.Core;

namespace TelemetryAnalyzer
{
    public class MissionManager : BindingSource
    {
        private GMapControl m_map;
        private GMapOverlay m_mapOverlay;
        private GMarkerGoogle m_currentPosMarker;
        private Mission m_selectedMission;

        public IList MissionList
        {
            get { return base.List; }
        }

        public MissionManager(GMapControl map = null)
        {
            DataSource = typeof(Mission);
            m_map = map;
            m_currentPosMarker = new GMarkerGoogle(new PointLatLng(0, 0), GMarkerGoogleType.yellow_pushpin);
            m_mapOverlay = new GMapOverlay();
            m_mapOverlay.Markers.Add(m_currentPosMarker);
            m_map.Overlays.Add(m_mapOverlay);
        }

        public void Add(Mission m)
        {
            base.Add(m);
            AddRoute(m);
            m_selectedMission = m;
        }

        private void AddRoute(Mission m)
        {
            // add route
            GMapRoute route = new GMapRoute(new List<PointLatLng>(), m.StartDate.ToString()) { Stroke = new Pen(Color.Red, 3) };
            route.Points.AddRange(m.Flight.Select(x => new PointLatLng(x.Latitude, x.Longitude)).ToList());
            m_mapOverlay.Routes.Add(route);

            // add markers
            m_mapOverlay.Markers.Add(new GMapMarkerImage(new PointLatLng(m.GetLaunch().Latitude, m.GetLaunch().Longitude), Properties.Resources.Ascending, new Point(-17, -43)));
            m_mapOverlay.Markers.Add(new GMapMarkerImage(new PointLatLng(m.GetBurst().Latitude, m.GetBurst().Longitude), Properties.Resources.Burst));
            m_mapOverlay.Markers.Add(new GMapMarkerImage(new PointLatLng(m.GetLanding().Latitude, m.GetLanding().Longitude), Properties.Resources.Descending, new Point(-10, -25)));

            m_map.SetZoomToFitRect(m.GetBounds());
        }

        public void SetCurrentMarker(TelemetryData data)
        {
            m_currentPosMarker.Position = new PointLatLng(data.Latitude, data.Longitude);
        }

        public void SelectMission(Mission m)
        {
            m_selectedMission = m;
            foreach (var item in m_mapOverlay.Routes)
            {
                item.Stroke.Width = 3;
                item.Stroke.Color = Color.Red;
            }
            int index = base.List.IndexOf(m);
            if (m_mapOverlay.Routes.Count > index)
            {
                m_mapOverlay.Routes[index].Stroke.Width = 5;
                m_mapOverlay.Routes[index].Stroke.Color = Color.Blue;
                m_map.Refresh();
            }
        }

        internal Image ImageAtIndex(int index)
        {
            if (m_selectedMission != null)
            {
                return m_selectedMission.GetImageAtIndex(index);
            }
            return null;
        }

        public Image GetImage(DateTime date)
        {
            if (m_selectedMission != null)
            {
                return m_selectedMission.GetImage(date);
            }
            return null;
        }

        internal Image VideoFrameAtIndex(int index)
        {
            if (m_selectedMission != null)
            {
                return m_selectedMission.GetVideoFrameAtIndex(index);
            }
            return null;
        }

        internal Image GetVideoFrame(DateTime date)
        {
            if (m_selectedMission != null)
            {
                return m_selectedMission.GetVideoFrame(date);
            }
            return null;
        }
    }
}
