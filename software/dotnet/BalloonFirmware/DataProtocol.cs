using System;
using System.Threading;

namespace BalloonFirmware
{
    public class DataProtocol
    {
        private const byte Sync = 0x7E;
        private const byte Esc = 0x7D;
        private const byte TransmitTelemetry = 0x01;
        private const byte BeginImage = 0x02;
        private const byte ImageData = 0x03;
        private const byte EndImage = 0x04;


        public byte[] GetTelemetry(TelemetryData data)
        {
            byte[] packet = new byte[37];
            packet[0] = TransmitTelemetry;
            Array.Copy(BitConverter.GetBytes((ushort)34), 0, packet, 1, 2);
            Array.Copy(BitConverter.GetBytes(data.UtcTimestamp.Ticks), 0, packet, 3, 8);
            Array.Copy(BitConverter.GetBytes(data.GpsData.Latitude), 0, packet, 11, 4);
            Array.Copy(BitConverter.GetBytes(data.GpsData.Longitude), 0, packet, 15, 4);
            Array.Copy(BitConverter.GetBytes(data.GpsData.Altitude), 0, packet, 19, 2);
            Array.Copy(BitConverter.GetBytes(data.GpsData.Heading), 0, packet, 21, 2);
            Array.Copy(BitConverter.GetBytes(data.GpsData.Speed), 0, packet, 23, 4);
            packet[27] = data.GpsData.Satellites;
            Array.Copy(BitConverter.GetBytes(data.IntTemperatureRaw), 0, packet, 28, 2);
            Array.Copy(BitConverter.GetBytes(data.ExtTemperatureRaw), 0, packet, 30, 2);
            Array.Copy(BitConverter.GetBytes(data.PressureRaw), 0, packet, 32, 2);
            Array.Copy(BitConverter.GetBytes(data.VinRaw), 0, packet, 34, 2);
            packet[36] = data.DutyCycle;
            return packet;
        }

        public byte[] GetBeginImage(DateTime utcTs)
        {
            byte[] packet = new byte[11];
            packet[0] = BeginImage;
            Array.Copy(BitConverter.GetBytes((ushort)8), 0, packet, 1, 2);
            Array.Copy(BitConverter.GetBytes(utcTs.Ticks), 0, packet, 3, 8);
            return packet;
        }

        public byte[] GetEndImage()
        {
            byte[] packet = new byte[3];
            packet[0] = EndImage;
            Array.Copy(BitConverter.GetBytes((ushort)0), 0, packet, 1, 2);
            return packet;
        }

        public byte[] GetImageData(byte[] data, int length)
        {
            byte[] packet = new byte[length + 3];
            packet[0] = ImageData;
            Array.Copy(BitConverter.GetBytes((ushort)length), 0, packet, 1, 2);
            Array.Copy(data, 0, packet, 3, length);
            return packet;
        }

        public static int PreparePacket(byte[] output, byte[] input)
        {
            output[0] = Sync;
            int count = 1;
            for (int i = 0; i < input.Length; i++)
            {
                if ((input[i] == Sync) || (input[i] == Esc))
                {
                    output[count++] = Esc;
                    output[count++] = (byte)(input[i] ^ 0x20);
                    count += 2;
                }
                else
                {
                    output[count++] = input[i];
                    count++;
                }
            }
            output[count++] = Sync;
            return count;
        }

    }
}
