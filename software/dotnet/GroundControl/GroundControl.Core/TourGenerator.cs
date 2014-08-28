using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpKml.Base;
using SharpKml.Engine;
using SharpKml.Dom;
using SharpKml.Dom.GX;

namespace GroundControl.Core
{
    /// <summary>
    /// Creates Google Earth tours.
    /// </summary>
    public class TourGenerator
    {
        /// <summary>
        /// Construct.
        /// </summary>
        public TourGenerator()
        {
        }

        /// <summary>
        /// Creates a Google Earth tour from telemetry data.
        /// </summary>
        /// <param name="telemetry">the telemetry data</param>
        /// <param name="minTimeDelta">the minimal time difference between two tour points</param>
        /// <returns>a KML object</returns>
        public KmlFile createTour(List<TelemetryData> telemetry, double minTimeDelta)
        {
            // choose data points
            List<TelemetryData> dataPoints = new List<TelemetryData>();
            if (telemetry.Count > 0)
            {
                long minTicks = (long)(minTimeDelta * 10000000);
                dataPoints.Add(telemetry[0]);
                TelemetryData lastP = telemetry[0];
                for (int i = 1; i < telemetry.Count; i++)
                {
                    if (telemetry[i].UtcTimestamp.Ticks - lastP.UtcTimestamp.Ticks >= minTicks)
                    {
                        dataPoints.Add(telemetry[i]);
                        lastP = telemetry[i];
                    }
                }
            }

            // build KML structure
            Tour tour = new Tour();
            tour.Name = "Balloon View";
            tour.Playlist = buildPlaylist(dataPoints);
            
            LineStyle lstyle = new LineStyle();
            lstyle.Color = new Color32(255, 0, 127, 255);
            lstyle.ColorMode = ColorMode.Normal;
            lstyle.Width = 2.0;
            Style style = new Style();
            style.Id = "orangeline";
            style.Line = lstyle;

            Placemark flight = buildFlightPath(dataPoints);

            Document document = new Document();
            document.Name = "M3 Space Mission";
            document.Open = true;
            document.AddStyle(style);
            document.AddFeature(tour);            
            document.AddFeature(flight);
            Kml root = new Kml();
            root.Feature = document;
            
            return KmlFile.Create(root, false);
        }

        private Playlist buildPlaylist(List<TelemetryData> dataPoints)
        {
            Playlist playList = new Playlist();
            if (dataPoints.Count > 0)
            {
                // first point
                Camera cam = new Camera();
                cam.AltitudeMode = SharpKml.Dom.AltitudeMode.Absolute;
                cam.Altitude = dataPoints[0].GpsAltitude;
                cam.Heading = dataPoints[0].Heading;
                cam.Latitude = dataPoints[0].Latitude;
                cam.Longitude = dataPoints[0].Longitude;
                cam.Tilt = 0.0;
                FlyTo fp = new FlyTo();
                fp.Duration = 5.0;
                fp.Mode = FlyToMode.Bounce;
                fp.View = cam;
                playList.AddTourPrimitive(fp);

                for (int i = 1; i < dataPoints.Count; i++)
                {
                    Camera c = new Camera();
                    c.AltitudeMode = SharpKml.Dom.AltitudeMode.Absolute;
                    c.Altitude = dataPoints[i].GpsAltitude;
                    c.Heading = dataPoints[i].Heading;
                    c.Latitude = dataPoints[i].Latitude;
                    c.Longitude = dataPoints[i].Longitude;
                    c.Tilt = 60.0;
                    FlyTo p = new FlyTo();
                    p.Duration = (dataPoints[i].UtcTimestamp - dataPoints[i - 1].UtcTimestamp).TotalSeconds;
                    p.Mode = FlyToMode.Smooth;
                    p.View = c;
                    playList.AddTourPrimitive(p);
                }
            }
            return playList;
        }

        private Placemark buildFlightPath(List<TelemetryData> dataPoints)
        {
            CoordinateCollection coords = new CoordinateCollection();
            for (int i = 0; i < dataPoints.Count; i++)
            {
                coords.Add(new Vector(dataPoints[i].Latitude, dataPoints[i].Longitude, dataPoints[i].GpsAltitude));
            }
            Placemark path = new Placemark();
            path.Name = "Flight path";
            LineString line = new LineString();
            line.Extrude = false;
            line.Tessellate = true;
            line.AltitudeMode = SharpKml.Dom.AltitudeMode.Absolute;
            line.Coordinates = coords;

            path.Geometry = line;
            // TODO add style (throws exception for unknown reason)
            //path.StyleUrl = new Uri("#orangeline", UriKind.Relative);
            return path;
        }
    }
}
