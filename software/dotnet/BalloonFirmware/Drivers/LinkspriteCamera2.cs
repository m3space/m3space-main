using System;
using System.IO.Ports;
using System.Threading;
using Microsoft.SPOT;

namespace BalloonFirmware.Drivers
{
    /// <summary>
    /// New LinkSprite serial camera implementation.
    /// </summary>
    public class LinkspriteCamera2
    {
        private const int RECEIVE_BUFFER_SIZE = 256;

        private readonly byte[] COMMAND_HEADER = new byte[] { 0x56, 0x00, 0x00, 0x00 };


        private byte[] rcvBuf = new byte[RECEIVE_BUFFER_SIZE];
        private SerialPort port;

        /// <summary>
        /// A delegate for receiving image chunks.
        /// </summary>
        /// <param name="chunk">the chunk data</param>
        /// <param name="complete">image is complete</param>
        /// <param name="error">an error occurred</param>
        public delegate void ImageChunkReceived(byte[] chunk, bool complete, bool error);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="port">the serial port</param>
        public LinkspriteCamera2(SerialPort port)
        {
            this.port = port;
            port.DataBits = 8;
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            port.ReadTimeout = 250;
        }

        /// <summary>
        /// Initializes the camera.
        /// </summary>
        /// <returns>true if initialization successful, false otherwise</returns>
        public bool Initialize()
        {
            if (!port.IsOpen)
            {
                port.Open();
            }
            return true;
        }

        /// <summary>
        /// Resets the camera.
        /// </summary>
        /// <returns>true if command successful, false otherwise</returns>
        public bool Reset()
        {
            return false;
        }

        /// <summary>
        /// Sets the baudrate.
        /// </summary>
        /// <param name="baudrate">the baudrate</param>
        /// <returns>true if command successful, false otherwise</returns>
        public bool SetBaudRate(int baudrate)
        {
            if ((baudrate == 9600) || (baudrate == 19200) || (baudrate == 38400) || (baudrate == 57600) || (baudrate == 115200))
            {
            }
            return false;
        }

        /// <summary>
        /// Sends a command.
        /// </summary>
        /// <param name="command">the command ID</param>
        /// <param name="data">the command data</param>
        private void SendCommand(byte command, byte[] data)
        {
            COMMAND_HEADER[2] = command;
            COMMAND_HEADER[3] = (byte)data.Length;
            port.Write(COMMAND_HEADER, 0, COMMAND_HEADER.Length);
            port.Write(data, 0, data.Length);
            port.Flush();
            Thread.Sleep(50);
        }

        /// <summary>
        /// Receives a command response.
        /// </summary>
        /// <param name="expectedResponse">the expected response</param>
        /// <returns>true if response ok, false otherwise</returns>
        private bool ReceiveResponse(byte[] expectedResponse)
        {
            int n = port.Read(rcvBuf, 0, expectedResponse.Length);
            return (ArrayStartsWith(rcvBuf, n, expectedResponse));
        }

        /// <summary>
        /// Determines if an arrays starts with the same data as another array.
        /// </summary>
        /// <param name="array">the array to examine</param>
        /// <param name="arrayLength">the examined array length</param>
        /// <param name="contains">the expected content</param>
        /// <returns>true if array contains the data, false otherwise</returns>
        private bool ArrayStartsWith(byte[] array, int arrayLength, byte[] contains)
        {
            if (contains.Length <= arrayLength)
            {
                for (int i = 0; i < contains.Length; i++)
                {
                    if (contains[i] != rcvBuf[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }   
}
