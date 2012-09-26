using System;

namespace BalloonFirmware
{
    public struct TelemetryData
    {
        public DateTime UtcTimestamp;
        public GpsPoint GpsData;
        public ushort IntTemperatureRaw;
        public ushort ExtTemperatureRaw;
        public ushort PressureRaw;
        public ushort VinRaw;
        public byte DutyCycle;
    }
}