using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundControl.Core
{
    /// <summary>
    /// Utility functions.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Finds the telemetry data index where the balloon bursted.
        /// Requires at least 3 elements to detect burst.
        /// </summary>
        /// <param name="telemetry">the telemetry data</param>
        /// <returns>-1 if not bursted, otherwise the burst index</returns>
        public static int FindBurstIndex(List<TelemetryData> telemetry)
        {
            int idx = -1;
            if (telemetry.Count > 2)
            {
                int i = 2;
                TelemetryData prev2 = telemetry[0];
                TelemetryData prev = telemetry[1];
                TelemetryData cur = telemetry[2];
                while (i < telemetry.Count)
                {
                    if ((prev2.VerticalSpeed >= 0.0f) &&
                        (prev.VerticalSpeed < 0.0f) &&
                        (cur.VerticalSpeed < 0.0f))
                    {
                        // last data point where balloon was going up
                        return i - 2;
                    }
                    prev2 = prev;
                    prev = cur;
                    cur = telemetry[i];
                    i++;
                }
            }
            return idx;
        }
    }
}
