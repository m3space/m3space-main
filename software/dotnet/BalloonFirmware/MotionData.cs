using System;
using Microsoft.SPOT;

namespace BalloonFirmware
{
    public struct MotionData
    {
        public DateTime UtcTimestamp;
        public short Ax;
        public short Ay;
        public short Az;
        public short Gx;
        public short Gy;
        public short Gz;
    }
}
