using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundControl.Core
{
    /// <summary>
    /// Data transceiver protocol implementation.
    /// </summary>
    public class DataProtocol
    {
        /// <summary>
        /// The frame header size.
        /// </summary>
        public const int HeaderSize = 3;

        /// <summary>
        /// The sync byte definition.
        /// </summary>
        public const byte Sync = 0x7E;

        /// <summary>
        /// The escape byte definition.
        /// </summary>
        public const byte Esc = 0x7D;

        private const byte TransmitTelemetry = 0x01;
        private const byte BeginImage = 0x02;
        private const byte ImageData = 0x03;
        private const byte EndImage = 0x04;

        private TelemetryDecoder telemetryDecoder;
        private ImageDecoder imageDecoder;

        /// <summary>
        /// This event is fired in case of an error.
        /// </summary>
        public event ErrorHandler Error;

        /// <summary>
        /// This event is fired when new telemetry data is received.
        /// </summary>
        public event TelemetryHandler TelemetryReceived;

        /// <summary>
        /// This event is fired when an image is complete.
        /// </summary>
        public event ImageCompleteHandler ImageComplete;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataProtocol()
        {
            telemetryDecoder = new TelemetryDecoder();
            imageDecoder = new ImageDecoder();
        }

        /// <summary>
        /// Decodes a data frame.
        /// </summary>
        /// <param name="frame">the frame data</param>
        public void DecodeFrame(byte[] frame)
        {
            if (frame.Length >= HeaderSize)
            {
                int payloadSize = BitConverter.ToUInt16(frame, 1);
                if (frame.Length >= HeaderSize + payloadSize)
                {
                    byte[] payload = new byte[payloadSize];
                    Array.Copy(frame, HeaderSize, payload, 0, payloadSize);
                    switch (frame[0])
                    {
                        // telemetry received
                        case TransmitTelemetry:
                            TelemetryData telemetry = telemetryDecoder.DecodeRawTelemetry(payload);
                            if (telemetry != null)
                            {
                                OnTelemetryReceived(telemetry);
                            }
                            else
                            {
                                OnError(String.Format("Unable to decode telemetry. Data size is {0}, expected {1}.", payloadSize, TelemetryDecoder.TelemetryPayloadSize));
                            }
                            break;

                        // beginning of image
                        case BeginImage:
                            long ticks = BitConverter.ToInt64(payload, 0);
                            int length = BitConverter.ToUInt16(payload, 8);
                            if (!imageDecoder.IsIdle)
                            {
                                OnError("Begin image, but last image was not fully decoded.");
                                OnImageComplete(imageDecoder.CurrentUtcTimestamp, imageDecoder.ImageData, false);
                                imageDecoder.EndImage();                                
                            }
                            if (!imageDecoder.BeginImage(new DateTime(ticks), length))
                            {
                                OnError("Cannot begin image, decoder is not idle.");
                            }
                            break;

                        // image data
                        case ImageData:
                            int imgOffset = BitConverter.ToUInt16(payload, 0);
                            int chunkLength = payload.Length - 2;

                            int result = imageDecoder.InsertChunk(imgOffset, payload, 2, chunkLength);
                            switch (result)
                            {
                                case ImageDecoder.OK:
                                    if (imageDecoder.IsImageComplete)
                                    {
                                        OnImageComplete(imageDecoder.CurrentUtcTimestamp, imageDecoder.ImageData, true);
                                        imageDecoder.EndImage();
                                    }
                                    break;

                                case ImageDecoder.UnexpectedEnd:
                                    OnError("Unexpected end of image.");
                                    OnImageComplete(imageDecoder.CurrentUtcTimestamp, imageDecoder.ImageData, false);
                                    imageDecoder.EndImage();
                                    break;

                                default:
                                    OnError(String.Format("Cannot insert image data (offset {0}, length {1})", imgOffset, chunkLength));
                                    break;
                            }
                            break;

                        // unknown frame
                        default:
                            OnError(String.Format("Invalid frame ID {0}.", frame[0]));
                            break;
                    }
                }
                else
                {
                    OnError("Frame payload incomplete.");
                }
            }
            else
            {
                OnError("Frame header incomplete.");
            }
        }

        /// <summary>
        /// Handles an error.
        /// </summary>
        /// <param name="message">the error message</param>
        private void OnError(string message)
        {
            if (Error != null)
                Error(message);
        }

        /// <summary>
        /// Handles new telemetry data.
        /// </summary>
        /// <param name="telemetry">the telemetry data</param>
        private void OnTelemetryReceived(TelemetryData telemetry)
        {
            if (TelemetryReceived != null)
                TelemetryReceived(telemetry);
        }

        /// <summary>
        /// Handles a completed image.
        /// </summary>
        /// <param name="utcTimestamp">the image timestamp (UTC)</param>
        /// <param name="data">the image data</param>
        /// <param name="ok">true if ok, false if with errors</param>
        private void OnImageComplete(DateTime utcTimestamp, byte[] data, bool ok)
        {
            if (data != null)
            {
                if (ImageComplete != null)
                {
                    ImageComplete(utcTimestamp, data, ok);
                }
            }
            else
            {
                OnError("Cannot fetch image data from decoder.");
            }
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
                }
                else
                {
                    output[count++] = input[i];
                }
            }
            output[count++] = Sync;
            return count;
        }

        public static byte[] GetTelemetry(TelemetryData data)
        {
            byte[] packet = new byte[45];
            packet[0] = TransmitTelemetry;
            Array.Copy(BitConverter.GetBytes((ushort)42), 0, packet, 1, 2);
            Array.Copy(BitConverter.GetBytes(data.UtcTimestamp.Ticks), 0, packet, 3, 8);
            Array.Copy(BitConverter.GetBytes(data.Latitude), 0, packet, 11, 4);
            Array.Copy(BitConverter.GetBytes(data.Longitude), 0, packet, 15, 4);
            Array.Copy(BitConverter.GetBytes((ushort)data.GpsAltitude), 0, packet, 19, 2);
            Array.Copy(BitConverter.GetBytes((ushort)data.Heading), 0, packet, 21, 2);
            Array.Copy(BitConverter.GetBytes(data.HorizontalSpeed), 0, packet, 23, 4);
            Array.Copy(BitConverter.GetBytes(data.VerticalSpeed), 0, packet, 27, 4);
            packet[31] = data.Satellites;
            Array.Copy(BitConverter.GetBytes(data.IntTemperature), 0, packet, 32, 2);
            Array.Copy(BitConverter.GetBytes(data.Temperature1Raw), 0, packet, 34, 2);
            Array.Copy(BitConverter.GetBytes(data.Temperature2Raw), 0, packet, 36, 2);
            Array.Copy(BitConverter.GetBytes(data.Pressure), 0, packet, 38, 2);
            Array.Copy(BitConverter.GetBytes(data.PressureAltitude), 0, packet, 40, 2);
            Array.Copy(BitConverter.GetBytes(data.VinRaw), 0, packet, 42, 2);
            packet[44] = data.DutyCycle;
            return packet;
        }
    }
}
