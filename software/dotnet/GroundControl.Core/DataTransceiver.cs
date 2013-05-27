using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace GroundControl.Core
{
    /// <summary>
    /// The serial space balloon data transceiver.
    /// </summary>
    public class DataTransceiver
    {
        private const int RECEIVE_BUFFER_SIZE = 1000;
        private const int FRAME_BUFFER_SIZE = 2000;

        private const int WAIT_BEGIN = 0;
        private const int WAIT_END = 2;

        private SerialPort serialPort;
        private Thread rcvThread;
        private bool doRun;
        private byte[] rcvBuf;
        private byte[] sndBuf;
        private byte[] frameBuf;
        private int framePos;
        private int receiverState;
        private byte lastByte;

        /// <summary>
        /// Checks if the transceiver thread is running.
        /// </summary>
        public bool IsRunning { get { return (rcvThread != null); } }

        /// <summary>
        /// A frame handler delegate.
        /// </summary>
        /// <param name="frame">the frame data</param>
        public delegate void FrameReceivedHandler(byte[] frame);

        /// <summary>
        /// This event is fired when a frame has been received.
        /// </summary>
        public event FrameReceivedHandler FrameReceived;

        /// <summary>
        /// This event is fired in case of an error.
        /// </summary>
        public event ErrorHandler Error;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="port">the serial port name</param>
        public DataTransceiver(string port)
        {
            serialPort = new SerialPort(port, 38400, Parity.None, 8, StopBits.One);
            doRun = false;
            rcvBuf = new byte[RECEIVE_BUFFER_SIZE];
            frameBuf = new byte[FRAME_BUFFER_SIZE];
            sndBuf = new byte[FRAME_BUFFER_SIZE];
            framePos = 0;
        }

        /// <summary>
        /// Sets the port name.
        /// Only allowed when the transceiver is not started.
        /// </summary>
        /// <param name="port">the port name</param>
        /// <returns>true if set successfully, false if transceiver already started</returns>
        public bool SetPort(string port)
        {
            if (rcvThread == null)
            {
                serialPort.PortName = port;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Starts the transceiver.
        /// </summary>
        public void Start()
        {
            if (rcvThread == null)
            {
                doRun = true;
                rcvThread = new Thread(new ThreadStart(ReceiveExec));
                rcvThread.Start();
            }
        }

        /// <summary>
        /// Stops the transceiver.
        /// </summary>
        public void Stop()
        {
            if (rcvThread != null)
            {
                doRun = false;
                rcvThread.Join(10000);
                if (rcvThread.IsAlive)
                {
                    try
                    {
                        rcvThread.Abort();
                    }
                    catch (Exception)
                    {
                        OnError("Transceiver thread aborted.");
                    }
                }
                rcvThread = null;
            }
        }

        /// <summary>
        /// The receiver thread method.
        /// </summary>
        private void ReceiveExec()
        {
            try
            {
                if (!serialPort.IsOpen)
                    serialPort.Open();

                receiverState = WAIT_BEGIN;
                lastByte = 0;
                framePos = 0;

                while (doRun)
                {
                    int count = serialPort.Read(rcvBuf, 0, RECEIVE_BUFFER_SIZE);
                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                            ReceiveByte(rcvBuf[i]);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception e)
            {
                OnError("Transceiver error: " + e.Message);
            }
            finally
            {
                serialPort.Close();
            }
        }

        /// <summary>
        /// Processes a byte received from the serial port.
        /// </summary>
        /// <param name="b"></param>
        private void ReceiveByte(byte b)
        {
            switch (receiverState)
            {
                case WAIT_BEGIN:
                    if ((b == DataProtocol.Sync) && (lastByte == DataProtocol.Sync))
                    {
                        receiverState = WAIT_END;
                    }
                    break;

                case WAIT_END:
                    if (b == DataProtocol.Sync)
                    {
                        byte[] frame = new byte[framePos];
                        Array.Copy(frameBuf, frame, framePos);
                        OnFrameReceived(frame);
                        receiverState = WAIT_BEGIN;
                        framePos = 0;
                    }
                    else if (b != DataProtocol.Esc)
                    {
                        if (framePos < FRAME_BUFFER_SIZE)
                        {
                            if (lastByte != DataProtocol.Esc)
                            {
                                frameBuf[framePos++] = b;
                            }
                            else
                            {
                                frameBuf[framePos++] = (byte)(b ^ 0x20);
                            }
                        }
                        else
                        {
                            OnError("Receiving frame buffer too small.");
                            receiverState = WAIT_BEGIN;
                            framePos = 0;
                        }
                    }
                    break;

                default:
                    OnError("Invalid receiver state.");
                    receiverState = WAIT_BEGIN;
                    break;
            }

            lastByte = b;
        }

        /// <summary>
        /// Handles a completely received frame.
        /// </summary>
        /// <param name="frame">the frame data</param>
        private void OnFrameReceived(byte[] frame)
        {
            if (FrameReceived != null)
                FrameReceived(frame);
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
        /// Sends a frame through the serial port.
        /// Data is escaped before transmitting.
        /// </summary>
        /// <param name="data">the frame data</param>
        /// <returns>true if successfully sent, false otherwise</returns>
        public bool SendFrame(byte[] data)
        {
            if (serialPort.IsOpen)
            {
                int sndPos = 0;
                sndBuf[sndPos++] = DataProtocol.Sync;
                for (int i = 0; i < data.Length; i++)
                {
                    if ((data[i] == DataProtocol.Sync) || (data[i] == DataProtocol.Esc))
                    {
                        sndBuf[sndPos++] = DataProtocol.Esc;
                        sndBuf[sndPos++] = (byte)(data[i] ^ 0x20);
                    }
                    else
                    {
                        sndBuf[sndPos++] = data[i];
                    }
                }
                sndBuf[sndPos++] = DataProtocol.Sync;
                serialPort.Write(sndBuf, 0, sndPos);
                return true;
            }
            return false;
        }
    }
}
