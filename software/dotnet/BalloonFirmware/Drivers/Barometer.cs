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
        private static readonly byte PRESSURE_SIZE = 26;    // number of bytes of the pressure packet (including \r\n)
        private static readonly byte TEMPERATURE_SIZE = 31; // number of bytes of the temperature packet (including \r\n)
        private static readonly byte ALTITUDE_SIZE = 21;    // number of bytes of the altitude packet (including \r\n)
        private static readonly byte READ_WAIT_TIME = 50;  // time in ms to wait between checking port for incoming data
        private static readonly byte READ_WAIT_CYCLE = 10;   // max cycles waiting for incoming data

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
        public ushort GetPressure()
        {
            FlushInput();
            port.Write(COMMAND_START, 0, 6);
            port.Write(GETPRESSURE, 0, 1);
            port.Write(COMMAND_END, 0, 2);
            port.Flush();
            byte i = 0;
            do
            {
                if (i == READ_WAIT_CYCLE)
                    return ushort.MaxValue;
                Thread.Sleep(READ_WAIT_TIME);
                i++;
            }
            while (port.BytesToRead < PRESSURE_SIZE);
            //Debug.Print("Pres: " + i);
            int n = port.Read(readBuffer, 0, PRESSURE_SIZE);
            if (n >= PRESSURE_SIZE)
            {
                // returns integer part only. float parsing not available.
                string str = new String(System.Text.Encoding.UTF8.GetChars(readBuffer)).Substring(13, 4);
                try
                {
                    return ushort.Parse(str);
                }
                catch (Exception)
                {
#if DEBUG
                    Debug.Print("Parse Pressure value failed.");
#endif
                    return ushort.MaxValue;
                }
            }

            return ushort.MaxValue;
        }

        /// <summary>
        /// Gets the temperature in celsius.
        /// </summary>
        /// <returns>the temperature</returns>
        public short GetTemperature()
        {
            FlushInput();
            port.Write(COMMAND_START, 0, 6);
            port.Write(GETTEMPERATURE, 0, 3);
            port.Write(COMMAND_END, 0, 2);
            port.Flush();
            byte i = 0;
            do
            {
                if (i == READ_WAIT_CYCLE)
                    return short.MinValue;
                Thread.Sleep(READ_WAIT_TIME);
                i++;
            }
            while (port.BytesToRead < TEMPERATURE_SIZE);
            //Debug.Print("Temp: " + i);
            int n = port.Read(readBuffer, 0, TEMPERATURE_SIZE);
            if (n >= TEMPERATURE_SIZE)
            {
                // returns integer part only. float parsing not available.
                string str = new String(System.Text.Encoding.UTF8.GetChars(readBuffer)).Substring(15, 4);
                try
                {
                    return short.Parse(str);
                }
                catch (Exception)
                {
#if DEBUG
                    Debug.Print("Parse Temperature value failed.");
#endif
                    return short.MinValue;
                } 
            }

            return short.MinValue;
        }

        /// <summary>
        /// Gets the current altitude in meters above sea level.
        /// </summary>
        /// <returns>the altitude</returns>
        public ushort GetAltitude()
        {
            FlushInput();
            port.Write(COMMAND_START, 0, 6);
            port.Write(GETALTITUDE, 0, 1);
            port.Write(COMMAND_END, 0, 2);
            port.Flush();
            byte i = 0;
            do
            {
                if (i == READ_WAIT_CYCLE)
                    return ushort.MaxValue;
                Thread.Sleep(READ_WAIT_TIME);
                i++;
            }
            while (port.BytesToRead < ALTITUDE_SIZE);
            //Debug.Print("Alti: " + i);
            int n = port.Read(readBuffer, 0, ALTITUDE_SIZE);
            if (n >= ALTITUDE_SIZE)
            {
                string str = new String(System.Text.Encoding.UTF8.GetChars(readBuffer)).Substring(7, 5);
                try
                {
                    return ushort.Parse(str);
                }
                catch (Exception)
                {
#if DEBUG
                    Debug.Print("Parse Altitude value failed.");
#endif
                    return ushort.MaxValue;
                }
            }

            return ushort.MaxValue;
        }

        /// <summary>
        /// Discards remaining serial input.
        /// </summary>
        private void FlushInput()
        {
            while (port.BytesToRead > 0)
            {
                port.Read(readBuffer, 0, readBufferSize);
            }
        }
    }
}
