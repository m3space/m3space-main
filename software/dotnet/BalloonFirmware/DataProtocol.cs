using System;
using System.Threading;

namespace BalloonFirmware
{
    public class DataProtocol
    {
        private const int BufferSize = 1000;
        private const byte Sync = 0x7E;
        private const byte Esc = 0x7D;
        private const byte TransmitTelemetry = 0x01;
        private const byte BeginImage = 0x02;
        private const byte ImageData = 0x03;
        private const byte EndImage = 0x04;

        private byte[] buf;
        private int bufPos;

        public DataProtocol()
        {
            buf = new byte[BufferSize];
        }

        public byte[] GetTelemetry(TelemetryData data)
        {
            Monitor.Enter(buf);
            bufPos = 0;
            buf[bufPos++] = Sync;
            buf[bufPos++] = TransmitTelemetry;
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes((ushort)34));
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes(data.UtcTimestamp.Ticks));
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes(data.GpsData.Latitude));
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes(data.GpsData.Longitude));
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes(data.GpsData.Altitude));
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes(data.GpsData.Heading));
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes(data.GpsData.Speed));
            bufPos += WriteEscapedBytes(buf, bufPos, new byte[] { data.GpsData.Satellites });
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes(data.IntTemperatureRaw));
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes(data.ExtTemperatureRaw));
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes(data.PressureRaw));
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes(data.VinRaw));
            bufPos += WriteEscapedBytes(buf, bufPos, new byte[] { data.DutyCycle });
            buf[bufPos++] = Sync;
            byte[] txData = new byte[bufPos];
            Array.Copy(buf, 0, txData, 0, bufPos);
            Monitor.Exit(buf);
            return txData;
        }

        public byte[] GetBeginImage(DateTime utcTs)
        {
            Monitor.Enter(buf);
            bufPos = 0;
            buf[bufPos++] = Sync;
            buf[bufPos++] = BeginImage;
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes((ushort)8));
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes(utcTs.Ticks));
            buf[bufPos++] = Sync;
            byte[] txData = new byte[bufPos];
            Array.Copy(buf, 0, txData, 0, bufPos);
            Monitor.Exit(buf);
            return txData;
        }

        public byte[] GetEndImage()
        {
            Monitor.Enter(buf);
            bufPos = 0;
            buf[bufPos++] = Sync;
            buf[bufPos++] = EndImage;
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes((ushort)0));
            buf[bufPos++] = Sync;
            byte[] txData = new byte[bufPos];
            Array.Copy(buf, 0, txData, 0, bufPos);
            Monitor.Exit(buf);
            return txData;
        }

        public byte[] GetImageData(byte[] data, int length)
        {
            Monitor.Enter(buf);
            bufPos = 0;
            buf[bufPos++] = Sync;
            buf[bufPos++] = ImageData;
            bufPos += WriteEscapedBytes(buf, bufPos, BitConverter.GetBytes((ushort)length));
            bufPos += WriteEscapedBytes(buf, bufPos, data, length);
            buf[bufPos++] = Sync;
            byte[] txData = new byte[bufPos];
            Array.Copy(buf, 0, txData, 0, bufPos);
            Monitor.Exit(buf);
            return txData;
        }

        private int WriteEscapedBytes(byte[] buf, int offset, byte[] data, int length)
        {
            int count = 0;
            for (int i = 0; i < length; i++)
            {
                if ((data[i] == Sync) || (data[i] == Esc))
                {
                    buf[offset++] = Esc;
                    buf[offset++] = (byte)(data[i] ^ 0x20);
                    count += 2;
                }
                else
                {
                    buf[offset++] = data[i];
                    count++;
                }
            }
            return count;
        }

        private int WriteEscapedBytes(byte[] buf, int offset, byte[] data)
        {
            return WriteEscapedBytes(buf, offset, data, data.Length);
        }
    }
}
