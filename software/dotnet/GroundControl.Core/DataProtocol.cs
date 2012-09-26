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
                                OnError(String.Format("Unable to decode telemetry. Data size is {0}, expected 34.", payloadSize));
                            }
                            break;

                        // beginning of image
                        case BeginImage:
                            long ticks = BitConverter.ToInt64(payload, 0);
                            if (!imageDecoder.IsIdle)
                            {
                                imageDecoder.EndImage();
                                OnError("Begin image, but last image was not fully decoded.");
                            }
                            if (!imageDecoder.BeginImage(new DateTime(ticks)))
                            {
                                OnError("Cannot begin image, decoder is not idle.");
                            }
                            break;

                        // image data
                        case ImageData:
                            if (!imageDecoder.AppendData(payload))
                            {
                                OnError("Cannot append image data, begin image first.");
                            }
                            break;

                        // end image
                        case EndImage:
                            DateTime utcTs = imageDecoder.CurrentUtcTimestamp;
                            byte[] imgData = imageDecoder.ImageData;
                            if (imgData != null)
                            {
                                OnImageComplete(utcTs, imgData);
                            }
                            else
                            {
                                OnError("Cannot fetch image data from decoder.");
                            }
                            imageDecoder.EndImage();
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
        private void OnImageComplete(DateTime utcTimestamp, byte[] data)
        {
            if (ImageComplete != null)
            {
                ImageComplete(utcTimestamp, data);
            }
        }
    }
}
