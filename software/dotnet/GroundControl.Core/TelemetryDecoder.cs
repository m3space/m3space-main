using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundControl.Core
{
    /// <summary>
    /// Computes telemetry data from raw data.
    /// </summary>
    public class TelemetryDecoder
    {
        private const int YEAR_OFFSET = 1600;  // because different DateTime origin in the microframework (year 1601 instead of 0001)
                                               // http://netmf.codeplex.com/workitem/1003
        private int TelemetryPayloadSize = 40;

        private float Deg2Rad = (float)Math.PI / 180.0f;
        private float TempOffset     = 24.0f;  // Werte empirisch ermittelt. Verglichen mit
        private float TempGain       = 0.102f; // Thermometer bei -30°C und +20°C (Temp[°C] = Offset - Raw * Gain)
        private float BatteryGain    = 1/120f; // Battery[V] = Raw * Gain

        /// <summary>
        /// Calculates real telemetry data from binary telemetry packet.
        /// </summary>
        /// <param name="rawData">the raw telemetry data (packet payload)</param>
        /// <returns>a TelemetryData object</returns>
        public TelemetryData DecodeRawTelemetry(byte[] rawData)
        {
            if (rawData.Length < TelemetryPayloadSize)
                return null;

            TelemetryData data = new TelemetryData();

            data.UtcTimestamp = new DateTime(BitConverter.ToInt64(rawData, 0)).AddYears(YEAR_OFFSET);
            data.Latitude = BitConverter.ToSingle(rawData, 8);
            data.Longitude = BitConverter.ToSingle(rawData, 12);
            data.GpsAltitude = BitConverter.ToUInt16(rawData, 16);
            data.Heading = Deg2Rad * BitConverter.ToUInt16(rawData, 18);
            data.HorizontalSpeed = BitConverter.ToSingle(rawData, 20);
            data.VerticalSpeed = BitConverter.ToSingle(rawData, 24);
            data.Satellites = rawData[28];

            data.IntTemperatureRaw = BitConverter.ToUInt16(rawData, 29);
            data.ExtTemperatureRaw = BitConverter.ToUInt16(rawData, 31);

            data.Pressure = BitConverter.ToUInt16(rawData, 33);
            data.PressureAltitude = BitConverter.ToUInt16(rawData, 35);

            data.VinRaw = BitConverter.ToUInt16(rawData, 37);
            data.DutyCycle = rawData[39];

            data.Vin = data.VinRaw * BatteryGain;

            data.IntTemperature = TempOffset - data.IntTemperatureRaw * TempGain;
            data.ExtTemperature = TempOffset - data.ExtTemperatureRaw * TempGain;

            return data;
        }
    }
}
