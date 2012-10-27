using System;

namespace BalloonFirmware
{
    public struct TelemetryData
    {
        public DateTime UtcTimestamp;
        public GpsPoint GpsData;
        public ushort IntTemperatureRaw;
        public ushort ExtTemperatureRaw;
        public ushort Pressure;
        public ushort PressureAltitude;
        public ushort VinRaw;
        public byte DutyCycle;
    }
}