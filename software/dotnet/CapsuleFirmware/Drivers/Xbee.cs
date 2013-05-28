using System;
using Microsoft.SPOT;
using System.IO.Ports;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Threading;
using System.Text;
using M3Space.Capsule.Util;

namespace M3Space.Capsule.Drivers
{
    /// <summary>
    /// An XBee 868 Pro driver.
    /// version 1.00
    /// </summary>
    public class Xbee
    {
        const int RECEIVE_BUFFER_SIZE = 32;

        private SerialPort port;
        private byte[] rcvBuf;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="port">the serial port</param>
        public Xbee(SerialPort port)
        {
            this.port = port;
            rcvBuf = new byte[RECEIVE_BUFFER_SIZE];
        }

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public void Initialize()
        {
            if (!port.IsOpen)
            {
                port.Open();
            }
            port.DiscardInBuffer();
        }

        /// <summary>
        /// Resets the XBee module.
        /// Uses FEZ Panda digital I/O.
        /// </summary>
        public void Reset()
        {
            TristatePort port = new TristatePort((Cpu.Pin)FEZ_Pin.Digital.Di7, false, false, Port.ResistorMode.PullUp);
            port.Active = true;  // set port as output
            port.Write(false);   // reset the xBee module 
            Thread.Sleep(1);     // wait (at least 100us)
            port.Active = false; // set port as input
        }

        /// <summary>
        /// Sets the transmission power level.
        /// </summary>
        /// <param name="level">0=1mW, 1=25mW, 2=100mW, 3=150mW, 4=300mW</param>
        public bool SetTransmitPower(byte level)
        {               
            if (EnterAtMode())
            {
                // now we are in at-command mode

                // set TX power (0=1mW, 1=23mW, 2=100mW, 3=158mW, 4=316mW)
                port.Write(Encoding.UTF8.GetBytes("ATPL" + level + "\r"), 0, 6);
                Thread.Sleep(200);

#if DEBUG
                Debug.Print("TX Power Level = " + level);
#endif
                ExitAtMode();

                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the duty cycle of the transmitter.
        /// </summary>
        /// <returns>the duty cycle in percent</returns>
        public uint GetDutyCycle()
        {
            uint dc = Byte.MaxValue;
            if (EnterAtMode())
            {
                //get DutyCycle counter (0 to 0x64) 0x64 means 100% dutycycle is reached
                port.Write(Encoding.UTF8.GetBytes("ATDC\r"), 0, 5);
                Thread.Sleep(200);
                int n = port.Read(rcvBuf, 0, RECEIVE_BUFFER_SIZE);
                if (n > 1)
                {
                    string readString = new String(Encoding.UTF8.GetChars(rcvBuf), 0 , n).TrimEnd('\r');    // subtract \r
                    dc = BitConverter.Hex2Dec(readString);
                }
#if DEBUG
                Debug.Print("DutyCycle = " + dc + "%");
#endif
                ExitAtMode();
            }
            return dc;
        }

        /// <summary>
        /// Transmits data.
        /// </summary>
        /// <param name="buf">the data buffer</param>
        /// <param name="offset">the buffer offset index</param>
        /// <param name="length">the number of bytes to send</param>
        public void Send(byte[] buf, int offset, int length)
        {
            port.Write(buf, offset, length);
            port.Flush();
        }

        /// <summary>
        /// Enters AT command mode.
        /// </summary>
        /// <returns>true if in AT mode, false otherwise</returns>
        public bool EnterAtMode()
        {
            Thread.Sleep(150);
            port.Write(Encoding.UTF8.GetBytes("+++"), 0, 3);
            Thread.Sleep(200);

            int n = port.Read(rcvBuf, 0, RECEIVE_BUFFER_SIZE);
            if (n >= 3)
            {
                string readString = new String(System.Text.Encoding.UTF8.GetChars(rcvBuf), 0, n).TrimEnd('\r');    // subtract \r
#if DEBUG
            Debug.Print("Enter ATmode = " + readString);
#endif
                return (readString.Equals("OK"));
            }

            return false;
        }

        /// <summary>
        /// Exits AT command mode.
        /// </summary>
        /// <returns></returns>
        public bool ExitAtMode()
        {
            port.DiscardInBuffer();
            port.Write(Encoding.UTF8.GetBytes("ATCN\r"), 0, 5);
            Thread.Sleep(200);

            int n = port.Read(rcvBuf, 0, RECEIVE_BUFFER_SIZE);
            if (n > 0)
            {
                string readString = new String(Encoding.UTF8.GetChars(rcvBuf), 0, n).TrimEnd('\r');    // subtract \r
#if DEBUG
                Debug.Print("Exit ATmode = " + readString + "\n");
#endif
                return true;
            }
            return false;
        }
    }
}
