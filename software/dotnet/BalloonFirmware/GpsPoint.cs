using System;

namespace BalloonFirmware
{
    public struct GpsPoint
    {
        public DateTime UtcTimestamp;   // 
        public float Latitude;          // [°]
        public float Longitude;         // [°]
        public float Speed;             // [m/s]
        public ushort Heading;          // [°]
        public ushort Altitude;         // [m]
        public byte Satellites;         // [#]
    }
}
