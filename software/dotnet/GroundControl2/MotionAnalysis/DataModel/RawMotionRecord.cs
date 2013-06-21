using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M3Space.MotionAnalysis.DataModel
{
    /// <summary>
    /// Raw motion data record as captured by the measurement hardware.
    /// </summary>
    public class RawMotionRecord
    {
        /// <summary>
        /// The absolute timestamp in UTC.
        /// </summary>
        public DateTime UtcTimestamp { get; set; }

        /// <summary>
        /// The acceleration along the X-axis.
        /// </summary>
        public int Ax { get; set; }

        /// <summary>
        /// The acceleration along the Y-axis.
        /// </summary>
        public int Ay { get; set; }

        /// <summary>
        /// The acceleration along the Z-axis.
        /// </summary>
        public int Az { get; set; }

        /// <summary>
        /// The gyro rotation around the X-axis.
        /// </summary>
        public int Rx { get; set; }

        /// <summary>
        /// The gyro rotation around the Y-axis.
        /// </summary>
        public int Ry { get; set; }

        /// <summary>
        /// The gyro rotation around the Z-axis.
        /// </summary>
        public int Rz { get; set; }
    }
}
