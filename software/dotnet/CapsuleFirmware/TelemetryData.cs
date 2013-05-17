using System;
using M3Space.Capsule.Drivers;

namespace M3Space.Capsule
{
    public struct TelemetryData
    {
        public DateTime UtcTimestamp;
        public GpsPoint GpsData;
        public short IntTemperature;
        public ushort Temperature1Raw;
        public ushort Temperature2Raw;
        public ushort Pressure;
        public ushort PressureAltitude;
        public ushort VinRaw;
        public byte DutyCycle;
    }
}