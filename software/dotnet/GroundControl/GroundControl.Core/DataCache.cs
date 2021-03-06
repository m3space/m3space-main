﻿using System;
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
        private float peakAltitude;
        private bool burstWasDetected;

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
            peakAltitude = 0.0f;
            burstWasDetected = false;
            Locked = false;
        }

        /// <summary>
        /// Deletes all data.
        /// </summary>
        public void Clear()
        {
            telemetry.Clear();
            peakAltitude = 0.0f;
            burstWasDetected = false;
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
            if (data.GpsAltitude > peakAltitude)
            {
                peakAltitude = data.GpsAltitude;
            }
            CheckBurst(data.GpsAltitude);
            if (TelemetryAdded != null)
                TelemetryAdded(data);
        }

        /// <summary>
        /// Adds new telemetry.
        /// </summary>
        /// <param name="data">the telemetry data</param>
        /// <param name="burstDetected">true if burst detected, false otherwise</param>
        public void AddTelemetry(TelemetryData data, out bool burstDetected)
        {
            telemetry.Add(data);
            if (data.GpsAltitude > peakAltitude)
            {
                peakAltitude = data.GpsAltitude;
            }
            burstDetected = CheckBurst(data.GpsAltitude);
            if (TelemetryAdded != null)
                TelemetryAdded(data);
        }

        /// <summary>
        /// Calculate telemetry data at given datetime
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns>interpolated telemetry data</returns>
        public TelemetryData GetInterpolatedData(DateTime datetime)
        {
            if (datetime.CompareTo(telemetry.First().UtcTimestamp) < 0)
            {
                return new TelemetryData()
                {
                    UtcTimestamp = datetime,
                    Latitude = telemetry.First().Latitude,
                    Longitude = telemetry.First().Longitude,
                    GpsAltitude = telemetry.First().GpsAltitude,
                    Temperature1 = telemetry.First().Temperature1,
                    Temperature2 = telemetry.First().Temperature2,
                    Satellites = telemetry.First().Satellites,
                    Pressure = telemetry.First().Pressure,
                    PressureAltitude = telemetry.First().PressureAltitude,
                    Vin = telemetry.First().Vin,
                    DutyCycle = telemetry.First().DutyCycle,
                    GammaCount = telemetry.First().GammaCount,
                    GammaCPM = telemetry.First().GammaCPM
                };
            }
            if (datetime.CompareTo(telemetry.Last().UtcTimestamp) > 0)
            {
                return new TelemetryData()
                {
                    UtcTimestamp = datetime,
                    Latitude = telemetry.Last().Latitude,
                    Longitude = telemetry.Last().Longitude,
                    GpsAltitude = telemetry.Last().GpsAltitude,
                    Temperature1 = telemetry.Last().Temperature1,
                    Temperature2 = telemetry.Last().Temperature2,
                    Satellites = telemetry.Last().Satellites,
                    Pressure = telemetry.Last().Pressure,
                    PressureAltitude = telemetry.Last().PressureAltitude,
                    Vin = telemetry.Last().Vin,
                    DutyCycle = telemetry.Last().DutyCycle,
                    GammaCount = telemetry.Last().GammaCount,
                    GammaCPM = telemetry.Last().GammaCPM
                };
            }
            for (int i = 1; i < telemetry.Count; i++)
            {
                if (telemetry[i].UtcTimestamp.CompareTo(datetime) > 0)
                {
                    TelemetryData data = new TelemetryData();
                    float factor = (float)(datetime.Subtract(telemetry[i - 1].UtcTimestamp).TotalSeconds /
                                           telemetry[i].UtcTimestamp.Subtract(telemetry[i - 1].UtcTimestamp).TotalSeconds);

                    data.GpsAltitude = telemetry[i - 1].GpsAltitude + (telemetry[i].GpsAltitude - telemetry[i - 1].GpsAltitude) * factor;
                    data.Temperature1 = telemetry[i - 1].Temperature1 + (telemetry[i].Temperature1 - telemetry[i - 1].Temperature1) * factor;
                    data.Temperature2 = telemetry[i - 1].Temperature2 + (telemetry[i].Temperature2 - telemetry[i - 1].Temperature2) * factor;
                    data.HorizontalSpeed = telemetry[i - 1].HorizontalSpeed + (telemetry[i].HorizontalSpeed - telemetry[i - 1].HorizontalSpeed) * factor;
                    data.VerticalSpeed = telemetry[i - 1].VerticalSpeed + (telemetry[i].VerticalSpeed - telemetry[i - 1].VerticalSpeed) * factor;
                    data.Vin = telemetry[i - 1].Vin + (telemetry[i].Vin - telemetry[i - 1].Vin) * factor;
                    data.Latitude = telemetry[i - 1].Latitude + (telemetry[i].Latitude - telemetry[i - 1].Latitude) * factor;
                    data.Longitude = telemetry[i - 1].Longitude + (telemetry[i].Longitude - telemetry[i - 1].Longitude) * factor;
                    data.Heading = (short)(telemetry[i - 1].Heading + (telemetry[i].Heading - telemetry[i - 1].Heading) * factor);
                    data.Pressure = telemetry[i - 1].Pressure + (telemetry[i].Pressure - telemetry[i - 1].Pressure) * factor;
                    data.DutyCycle = (byte)(telemetry[i - 1].DutyCycle + (telemetry[i].DutyCycle - telemetry[i - 1].DutyCycle) * factor);
                    data.PressureAltitude = telemetry[i - 1].PressureAltitude + (telemetry[i].PressureAltitude - telemetry[i - 1].PressureAltitude) * factor;
                    data.IntTemperature = (int)(telemetry[i - 1].IntTemperature + (telemetry[i].IntTemperature - telemetry[i - 1].IntTemperature) * factor);
                    data.GammaCount = (int)(telemetry[i - 1].GammaCount + (telemetry[i].GammaCount - telemetry[i - 1].GammaCount) * factor);
                    data.GammaCPM = telemetry[i - 1].GammaCPM + (telemetry[i].GammaCPM - telemetry[i - 1].GammaCPM) * factor;

                    data.Satellites = (byte)(telemetry[i - 1].Satellites + (telemetry[i].Satellites - telemetry[i - 1].Satellites) * factor);
                    data.UtcTimestamp = datetime;
                    return data;
                }
            }
            return null;
        }

        private bool CheckBurst(float altitude)
        {
            bool detected = (!burstWasDetected && (altitude < peakAltitude - 75.0f));
            if (detected)
            {
                burstWasDetected = true;
            }
            return detected;
        }
    }
}
