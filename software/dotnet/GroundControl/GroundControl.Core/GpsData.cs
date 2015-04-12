using System;

namespace GroundControl.Core
{
    /// <summary>
    /// A GPS data object.
    /// </summary>
    public class GpsData
    {
        /// <summary>
        /// The UTC timestamp.
        /// </summary>
        public DateTime UtcTimestamp { get; set; }

        /// <summary>
        /// GPS latitude in decimal degrees.
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// GPS longitude in decimal degrees.
        /// </summary>
        public float Longitude { get; set; }
    }
}