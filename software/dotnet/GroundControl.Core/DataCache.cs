using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundControl.Core
{
    /// <summary>
    /// A memory cache for telemetry data.
    /// </summary>
    public class DataCache
    {
        private readonly List<TelemetryData> telemetry;

        /// <summary>
        /// Telemetry delegate.
        /// </summary>
        /// <param name="data">the telemetry data</param>
        public delegate void TelemetryModified(TelemetryData data);

        /// <summary>
        /// Clear delegate.
        /// </summary>
        public delegate void ClearedDelegate();

        /// <summary>
        /// This event is fired when new telemetry is added.
        /// </summary>
        public event TelemetryModified TelemetryAdded;

        /// <summary>
        /// This event is fired when the data cache is cleared.
        /// </summary>
        public event ClearedDelegate Cleared;

        /// <summary>
        /// Gets the number of telemetry entries.
        /// </summary>
        public int Size { get { return telemetry.Count; } }

        /// <summary>
        /// Sets the data cache read-only or not.
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Gets the telemetry data.
        /// </summary>
        public List<TelemetryData> Telemetry { get { return telemetry; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataCache()
        {
            telemetry = new List<TelemetryData>();
            Locked = false;
        }

        /// <summary>
        /// Deletes all data.
        /// </summary>
        public void Clear()
        {
            telemetry.Clear();
            Locked = false;

            if (Cleared != null)
                Cleared();
        }

        /// <summary>
        /// Adds new telemetry.
        /// </summary>
        /// <param name="data">the telemetry data</param>
        public void AddTelemetry(TelemetryData data)
        {
            telemetry.Add(data);

            if (TelemetryAdded != null)
                TelemetryAdded(data);
        }
    }
}
