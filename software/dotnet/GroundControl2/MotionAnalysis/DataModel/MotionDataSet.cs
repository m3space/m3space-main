using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M3Space.MotionAnalysis.DataModel
{
    /// <summary>
    /// A set of motion data records.
    /// </summary>
    public class MotionDataSet
    {
        /// <summary>
        /// The starting timestamp (UTC).
        /// </summary>
        public DateTime UtcStart { get; set; }

        /// <summary>
        /// The constant time interval between two data records.
        /// Measured in milliseconds.
        /// </summary>
        public long Interval { get; set; }

        /// <summary>
        /// The data records.
        /// </summary>
        public List<MotionRecord> Records { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MotionDataSet()
        {
            Records = new List<MotionRecord>();
        }
    }
}
