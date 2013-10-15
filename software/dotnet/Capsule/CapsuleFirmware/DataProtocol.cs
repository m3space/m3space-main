using System;
using System.Threading;
using M3Space.Capsule.Util;

namespace M3Space.Capsule
{
    public class DataProtocol
    {
        private const byte StartPacket = 0x7D;
        private const byte EndPacket = 0x7E;
        private const byte Esc = 0x7F;
        private const byte EscMask = 0x20;

        private const byte TransmitTelemetry = 0x01;
        private const byte BeginImage = 0x02;
        private const byte ImageData = 0x03;


        public byte[] GetTelemetry(TelemetryData data)
        {
            byte[] packet = new byte[45];
            packet[0] = TransmitTelemetry;
            Array.Copy(BitConverter.GetBytes((ushort)42), 0, packet, 1, 2);
            Array.Copy(BitConverter.GetBytes(data.UtcTimestamp.Ticks), 0, packet, 3, 8);
            Array.Copy(BitConverter.GetBytes(data.GpsData.Latitude), 0, packet, 11, 4);
            Array.Copy(BitConverter.GetBytes(data.GpsData.Longitude), 0, packet, 15, 4);
            Array.Copy(BitConverter.GetBytes((ushort)data.GpsData.Altitude), 0, packet, 19, 2);
            Array.Copy(BitConverter.GetBytes((ushort)data.GpsData.Heading), 0, packet, 21, 2);
            Array.Copy(BitConverter.GetBytes(data.GpsData.HorizontalSpeed), 0, packet, 23, 4);
            Array.Copy(BitConverter.GetBytes(data.GpsData.VerticalSpeed), 0, packet, 27, 4);
            packet[31] = data.GpsData.Satellites;
            Array.Copy(BitConverter.GetBytes(data.IntTemperature), 0, packet, 32, 2);
            Array.Copy(BitConverter.GetBytes(data.Temperature1Raw), 0, packet, 34, 2);
            Array.Copy(BitConverter.GetBytes(data.Temperature2Raw), 0, packet, 36, 2);
            Array.Copy(BitConverter.GetBytes(data.Pressure), 0, packet, 38, 2);
            Array.Copy(BitConverter.GetBytes(data.PressureAltitude), 0, packet, 40, 2);
            Array.Copy(BitConverter.GetBytes(data.VinRaw), 0, packet, 42, 2);
            packet[44] = data.DutyCycle;
            return packet;
        }

        public byte[] GetBeginImage(DateTime utcTs, int length)
        {
            byte[] packet = new byte[13];
            packet[0] = BeginImage;
            Array.Copy(BitConverter.GetBytes((ushort)10), 0, packet, 1, 2);
            Array.Copy(BitConverter.GetBytes(utcTs.Ticks), 0, packet, 3, 8);
            Array.Copy(BitConverter.GetBytes((ushort)length), 0, packet, 11, 2);
            return packet;
        }

        public byte[] GetImageData(int imgOffset, byte[] data, int dataLength)
        {
            byte[] packet = new byte[dataLength + 5];
            packet[0] = ImageData;
            Array.Copy(BitConverter.GetBytes((ushort)dataLength + 2), 0, packet, 1, 2);
            Array.Copy(BitConverter.GetBytes(imgOffset), 0, packet, 3, 2);
            Array.Copy(data, 0, packet, 5, dataLength);
            return packet;
        }

        public static int PreparePacket(byte[] output, byte[] input)
        {
            output[0] = StartPacket;
            int count = 1;
            for (int i = 0; i < input.Length; i++)
            {
                if ((input[i] == StartPacket) || (input[i] == EndPacket) || (input[i] == Esc))
                {
                    output[count++] = Esc;
                    output[count++] = (byte)(input[i] ^ EscMask);
                }
                else
                {
                    output[count++] = input[i];
                }
            }
            output[count++] = EndPacket;
            return count;
        }

    }
}
