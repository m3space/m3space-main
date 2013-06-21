using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M3Space.MotionAnalysis.DataModel
{
    /// <summary>
    /// Processed motion data record.
    /// </summary>
    public class MotionRecord
    {
        /// <summary>
        /// The raw acceleration along the X-axis.
        /// Measured in g.
        /// </summary>
        public float Ax { get; set; }

        /// <summary>
        /// The raw acceleration along the Y-axis.
        /// Measured in g.
        /// </summary>
        public float Ay { get; set; }

        /// <summary>
        /// The raw acceleration along the Z-axis.
        /// Measured in g.
        /// </summary>
        public float Az { get; set; }

        /// <summary>
        /// The gravity along the X-axis.
        /// Measured in g.
        /// </summary>
        public float Gx { get; set; }

        /// <summary>
        /// The gravity along the Y-axis.
        /// Measured in g.
        /// </summary>
        public float Gy { get; set; }

        /// <summary>
        /// The gravity along the Z-axis.
        /// Measured in g.
        /// </summary>
        public float Gz { get; set; }

        /// <summary>
        /// The gyro rotation around the X-axis.
        /// Measured in degrees/sec.
        /// </summary>
        public float Rx { get; set; }

        /// <summary>
        /// The gyro rotation around the Y-axis.
        /// Measured in degrees/sec.
        /// </summary>
        public float Ry { get; set; }

        /// <summary>
        /// The gyro rotation around the Z-axis.
        /// Measured in degrees/sec.
        /// </summary>
        public float Rz { get; set; }
    }
}
