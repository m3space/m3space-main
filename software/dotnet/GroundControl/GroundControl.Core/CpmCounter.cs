using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundControl.Core
{
    /// <summary>
    /// A CPM counter for radiation measurements.
    /// </summary>
    public class CpmCounter
    {
        private int count;
        private double cpm;
        private DateTime ageLimit;
        private LinkedList<TelemetryData> cache;

        private static readonly TimeSpan MaxAge = new TimeSpan(0, 1, 0);

        /// <summary>
        /// Gets the absolute count.
        /// </summary>
        public int Count { get { return count;  } }

        /// <summary>
        /// Gets the computed counts per minute.
        /// </summary>
        public double CPM { get { return cpm; } }

        /// <summary>
        /// Construct.
        /// </summary>
        public CpmCounter()
        {
            count = 0;
            cpm = 0.0;
            cache = new LinkedList<TelemetryData>();
        }

        /// <summary>
        /// Resets the counter.
        /// </summary>
        public void Reset()
        {
            cache.Clear();
            count = 0;
            cpm = 0.0;
        }

        /// <summary>
        /// Updates the counter with a new measurement.
        /// </summary>
        /// <param name="data">the new measurement</param>
        public void Update(TelemetryData data)
        {
            ageLimit = data.UtcTimestamp.Subtract(MaxAge);
            cache.AddLast(data);
            if (cache.Count > 1)
            {
                while (cache.First.Value.UtcTimestamp < ageLimit)
                {
                    cache.RemoveFirst();
                }
                TimeSpan ts = cache.Last.Value.UtcTimestamp.Subtract(cache.First.Value.UtcTimestamp);
                if (ts.TotalMinutes > 1.0)
                {
                    cpm = (cache.Last.Value.GammaCount - cache.First.Value.GammaCount) / ts.TotalMinutes;
                }
                else
                {
                    cpm = 0.0;
                }
            }
            else
            {
                cpm = 0.0;
            }
            this.count = data.GammaCount;
        }

        private class Measurement
        {
            public DateTime Timestamp { get; set; }
            public int Count { get; set; }

            public Measurement(DateTime timestamp, int count)
            {
                Count = count;
                Timestamp = timestamp;
            }
        }
    }
}
