using System;
using Microsoft.SPOT;
using System.IO.Ports;
using System.Threading;

namespace BalloonFirmware.Drivers
{
    /// <summary>
    /// Sure Electronics Barometer Module with MS5561 Sensor.
    /// </summary>
    public class Barometer
    {
        private SerialPort port;

        private static readonly byte[] COMMAND_START = new byte[] { 0x24, 0x73, 0x75, 0x72, 0x65, 0x20 };   // $sure<SP>
        private static readonly byte[] COMMAND_END = new byte[] { 0x0D, 0x0A };                             // <CR><LF>
        private static readonly byte[] GETPRESSURE = new byte[] { 0x70 };                                   // p
        private static readonly byte[] GETTEMPERATURE = new byte[] { 0x74, 0x2D, 0x63 };                    // t-c
        private static readonly byte[] GETALTITUDE = new byte[] { 0x68 };                                   // h

        private byte[] readBuffer;
        private const int readBufferSize = 32;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="port">the serial port</param>
        public Barometer(SerialPort port)
        {
            readBuffer = new byte[readBufferSize];

            this.port = port;

            this.port.BaudRate = 9600;
            this.port.DataBits = 8;
            this.port.StopBits = StopBits.One;
            this.port.Parity = Parity.None;
            this.port.ReadTimeout = 250;
        }

        /// <summary>
        /// Initializes the port.
        /// </summary>
        public void Initialize()
        {
            if (!port.IsOpen)
            {
                port.Open();
            }
        }

        /// <summary>
        /// Gets the air pressure in bar.
        /// </summary>
        /// <returns>the air pressure</returns>
        public int GetPressure()
        {
            FlushInput();
            port.Write(COMMAND_START, 0, 6);
            port.Write(GETPRESSURE, 0, 1);
            port.Write(COMMAND_END, 0, 2);
            port.Flush();
            while (port.BytesToRead < 24)
            {
                Thread.Sleep(100);
            }
            int n = port.Read(readBuffer, 0, 24);
            if (n >= 24)
            {
                // returns integer part only. float parsing not available.
                string str = new String(System.Text.Encoding.UTF8.GetChars(readBuffer)).Substring(13, 4);
                return int.Parse(str);
            }

            return int.MinValue;
        }

        /// <summary>
        /// Gets the temperature in celsius.
        /// </summary>
        /// <returns>the temperature</returns>
        public int GetTemperature()
        {
            FlushInput();
            port.Write(COMMAND_START, 0, 6);
            port.Write(GETTEMPERATURE, 0, 3);
            port.Write(COMMAND_END, 0, 2);
            port.Flush();
            while (port.BytesToRead < 29)
            {
                Thread.Sleep(100);
            }
            int n = port.Read(readBuffer, 0, 29);
            if (n >= 29)
            {
                // returns integer part only. float parsing not available.
                string str = new String(System.Text.Encoding.UTF8.GetChars(readBuffer)).Substring(15, 4);
                return int.Parse(str);
            }

            return int.MinValue;
        }

        /// <summary>
        /// Gets the current altitude in meters above sea level.
        /// </summary>
        /// <returns>the altitude</returns>
        public int GetAltitude()
        {
            FlushInput();
            port.Write(COMMAND_START, 0, 6);
            port.Write(GETALTITUDE, 0, 1);
            port.Write(COMMAND_END, 0, 2);
            port.Flush();
            while (port.BytesToRead < 19)
            {
                Thread.Sleep(100);
            }
            int n = port.Read(readBuffer, 0, 19);
            if (n >= 19)
            {
                string str = new String(System.Text.Encoding.UTF8.GetChars(readBuffer)).Substring(7, 5);
                return int.Parse(str);
             }

            return int.MinValue;
        }

        /// <summary>
        /// Discards remaining serial input.
        /// </summary>
        private void FlushInput()
        {
            int n;
            do
            {
                n = port.Read(readBuffer, 0, readBufferSize);
            }
            while (n != 0);
        }
    }
}
