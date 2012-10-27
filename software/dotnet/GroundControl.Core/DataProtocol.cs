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
                                OnError(String.Format("Unable to decode telemetry. Data size is {0}, expected 39.", payloadSize));
                            }
                            break;

                        // beginning of image
                        case BeginImage:
                            long ticks = BitConverter.ToInt64(payload, 0);
                            int length = BitConverter.ToUInt16(payload, 8);
                            if (!imageDecoder.IsIdle)
                            {
                                OnError("Begin image, but last image was not fully decoded.");
                                OnImageComplete(imageDecoder.CurrentUtcTimestamp, imageDecoder.ImageData);
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
                            if (imageDecoder.InsertChunk(imgOffset, payload, 2, chunkLength))
                            {
                                if (imageDecoder.IsImageComplete)
                                {
                                    OnImageComplete(imageDecoder.CurrentUtcTimestamp, imageDecoder.ImageData);
                                    imageDecoder.EndImage();
                                }
                            }
                            else
                            {
                                OnError("Cannot insert image data, begin image first.");
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
        private void OnImageComplete(DateTime utcTimestamp, byte[] data)
        {
            if (data != null)
            {
                if (ImageComplete != null)
                {
                    ImageComplete(utcTimestamp, data);
                }
            }
            else
            {
                OnError("Cannot fetch image data from decoder.");
            }
        }
    }
}
