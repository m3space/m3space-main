using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.IO;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.IO;
using BalloonFirmware.Drivers;


namespace BalloonFirmware
{
    public class SpaceBalloon
    {
        private const int TELEMETRY_TX_INTERVAL = 5;
        private const int IMAGE_TX_INTERVAL = 5;
        private const int IMAGE_CHUNK_SIZE = 60;

        private BoundedBuffer txQueue;
        private byte[] txBuffer;
        private DataProtocol dataProtocol;

        private PersistentStorage sdStorage;
        private string telemetryFileName;

        private AnalogIn tempSensorInt;
        private AnalogIn tempSensorExt;
        private AnalogIn vInSensor;

        private TelemetryData cachedTelemetry;
        private GpsPoint cachedGpsPoint;

        private SerialPort gpsPort;
        private SerialPort xBeePort;
        private SerialPort cameraPort;

        private byte xBeeDutyCycle;
        private DateTime lastXBeeResetCheck;

        private string sdRootDirectory;
        private string currentImageFilename;
        private DateTime currentImageTimestamp;
        private DateTime lastSentImage;
        
        private GpsReader gps;

        private object gpsLock = new object();
        private object dutyCycleLock = new object();
        private AutoResetEvent waitTimeSync;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SpaceBalloon()
        {
            txQueue = new BoundedBuffer();
            txBuffer = new byte[256];
            dataProtocol = new DataProtocol();
            sdStorage = new PersistentStorage("SD");
            tempSensorInt = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An4);
            tempSensorExt = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An5);
            vInSensor = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An2);

            gpsPort = new SerialPort("COM3", 38400, Parity.None, 8, StopBits.One);
            xBeePort = new SerialPort("COM1", 38400, Parity.None, 8, StopBits.One);
            cameraPort = new SerialPort("COM4", 38400, Parity.None, 8, StopBits.One);

            currentImageTimestamp = DateTime.Now;
            lastSentImage = DateTime.Now;

            lastXBeeResetCheck = DateTime.Now;
            xBeeDutyCycle = 0;

            gps = new GpsReader();
            gps.GpsDataReceived += SetTimeFromGps;

            waitTimeSync = new AutoResetEvent(false);
        }


        /// <summary>
        /// Initializes the balloon software.
        /// </summary>
        public void Initialize()
        {
            sdStorage.MountFileSystem();
            sdRootDirectory = VolumeInfo.GetVolumes()[0].RootDirectory;
            telemetryFileName = sdRootDirectory + @"\telemetry.csv";
            
            Thread gpsThread = new Thread(new ThreadStart(StartGpsThread));
            Thread transmitThread = new Thread(new ThreadStart(StartTransmitThread));
            Thread telemetryThread = new Thread(new ThreadStart(StartTelemetryThread));
            Thread cameraThread = new Thread(new ThreadStart(StartCameraThread));

            gpsThread.Start();

            // wait until system time is synchronized.
            waitTimeSync.WaitOne();

            transmitThread.Start();
            telemetryThread.Start();
            cameraThread.Start();            
        }


        /// <summary>
        /// Starts the GPS receiver thread.
        /// </summary>
        private void StartGpsThread()
        {            
            gpsPort.Open();
            string strData = "";
            byte[] gpsBuf = new byte[100];

            while (true)
            {
                int count = gpsPort.Read(gpsBuf, 0, 100);
                if (count > 0)
                {
                    try
                    {
                        strData += new String(System.Text.Encoding.UTF8.GetChars(gpsBuf), 0, count);
                    }
                    catch (Exception)
                    {
                    }
                    string temp = "";
                    while (strData != temp)
                    {
                        temp = strData;
                        strData = gps.GetNmeaString(strData);
                    }
                }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Processes GPS data.
        /// </summary>
        /// <param name="gpsPoint">the GPS data</param>
        private void GpsDataHandler(GpsPoint gpsPoint)
        {
            Monitor.Enter(gpsLock);
            // compute vertical speed
            gpsPoint.VerticalSpeed = (gpsPoint.Altitude - cachedGpsPoint.Altitude) / ((gpsPoint.UtcTimestamp.Ticks - cachedGpsPoint.UtcTimestamp.Ticks) * 1e-7f);             
            this.cachedGpsPoint = gpsPoint;
            Monitor.Exit(gpsLock);
        }

        /// <summary>
        /// Sets the system time according to GPS data.
        /// Only executed once.
        /// </summary>
        /// <param name="gpsPoint">the GPS data</param>
        private void SetTimeFromGps(GpsPoint gpsPoint)
        {
            gps.GpsDataReceived -= SetTimeFromGps;          // deactivate handler
            Utility.SetLocalTime(gpsPoint.UtcTimestamp);    // set system time to UTC, therefore DateTime.Now returns UTC.
#if DEBUG
            Debug.Print("Time synchronized to " + gpsPoint.UtcTimestamp.ToString("yyyyMMdd_HHmmss"));
#endif
            waitTimeSync.Set();
            gps.GpsDataReceived += GpsDataHandler;
        }

        /// <summary>
        /// Starts the XBee transmitter thread.
        /// </summary>
        private void StartTransmitThread()
        {
            xBeePort.Open();

            SetXbeeTransmitPower();

            while (true)
            {
                try
                {
                    // get packet from queue
                    byte[] packet = (byte[])txQueue.Take();

                    #if DEBUG
                    Debug.Print("Sending packet ID " + packet[0]);
                    #endif

                    // escape packet
                    int len = DataProtocol.PreparePacket(txBuffer, packet);

                    // send escaped packet
                    xBeePort.Write(txBuffer, 0, len);
                    xBeePort.Flush();

                    Thread.Sleep(100);

                    // read dutycycle value every 2 minutes and reset xBee if value >= 40%
                    if ((DateTime.Now - lastXBeeResetCheck).Minutes >= 2)
                    {
                        GetDutyCycleAndReset(xBeePort, 40);
                        lastXBeeResetCheck = DateTime.Now;
                    }
                }
                catch (Exception e)
                {
                    //Debug.Print(e.Message);
                }
            }
        }

        /// <summary>
        /// Starts the sensor telemetry thread.
        /// </summary>
        private void StartTelemetryThread()
        {
            int count = TELEMETRY_TX_INTERVAL;
            while (true)
            {
                // use system time, not time of last GPS position
                cachedTelemetry.UtcTimestamp = DateTime.Now;
                // read sensor values
                cachedTelemetry.IntTemperatureRaw = (ushort)tempSensorInt.Read();
                cachedTelemetry.ExtTemperatureRaw = (ushort)tempSensorExt.Read();

                cachedTelemetry.Pressure = 0;           // TODO read pressure
                cachedTelemetry.PressureAltitude = 0;   // TODO read altitude

                cachedTelemetry.VinRaw = (ushort)vInSensor.Read();
                Monitor.Enter(gpsLock);
                cachedTelemetry.GpsData = cachedGpsPoint;
                Monitor.Exit(gpsLock);
                Monitor.Enter(dutyCycleLock);
                cachedTelemetry.DutyCycle = xBeeDutyCycle;
                Monitor.Exit(dutyCycleLock);
                StoreTelemetry(cachedTelemetry);
                count--;
                if (count == 0)
                {
                    // transmit data
                    txQueue.Add(dataProtocol.GetTelemetry(cachedTelemetry));
                    count = TELEMETRY_TX_INTERVAL;
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Stores telemetry data on SD card.
        /// </summary>
        /// <param name="data">the telemetry data to save</param>
        private void StoreTelemetry(TelemetryData data)
        {
            FileStream fileHandle = new FileStream(telemetryFileName, FileMode.OpenOrCreate);

            string text = data.UtcTimestamp.ToString("dd.MM.yyyy HH:mm:ss") + ';' +
                data.GpsData.Latitude.ToString("F5") + ';' +
                data.GpsData.Longitude.ToString("F5") + ';' +
                data.GpsData.Altitude.ToString() + ';' +
                data.GpsData.HorizontalSpeed.ToString("F2") + ';' +
                data.GpsData.VerticalSpeed.ToString("F2") + ';' +
                data.GpsData.Heading.ToString() + ';' +
                data.GpsData.Satellites.ToString() + ';' +
                data.IntTemperatureRaw.ToString() + ';' +
                data.ExtTemperatureRaw.ToString() + ';' +
                data.Pressure.ToString() + ';' +
                data.PressureAltitude.ToString() + ';' +
                data.VinRaw.ToString() + ';' +
                data.DutyCycle.ToString() +
                "\r\n";

            byte[] writeData = Encoding.UTF8.GetBytes(text);

            fileHandle.Position = fileHandle.Length;
            fileHandle.Write(writeData, 0, writeData.Length);
            fileHandle.Close();
        }

        /// <summary>
        /// Starts the serial camera thread.
        /// </summary>
        private void StartCameraThread()
        {            
            LinkspriteCamera camera = new LinkspriteCamera(cameraPort);

            //camera.SetBaudRate(LinkspriteCamera.Baudrate.Baud_115200);
            //camera.SetPictureSize(LinkspriteCamera.SET_SIZE_640x480);

            while (true)
            {
                camera.Reset();
                //camera.SetPictureSize(LinkspriteCamera.SET_SIZE_640x480);
                camera.GetPicture(CameraDataHandler);
                camera.Stop();

                Thread.Sleep(60000);
            }
        }

        /// <summary>
        /// Processes image data received from camera.
        /// </summary>
        /// <param name="data">the data</param>
        private void CameraDataHandler(byte[] data)
        {
            try
            {
                // the begin of a new jpg image
                if ((data.Length >= 2) && (data[0] == 0xFF) && (data[1] == 0xD8))
                {
#if DEBUG
                    Debug.Print("Begin of JPG");
#endif
                    currentImageTimestamp = DateTime.Now;
                    currentImageFilename = sdRootDirectory + @"\" + currentImageTimestamp.ToString("yyyyMMdd_HHmmss") + ".jpg";
                }

                StoreImageChunk(data);

                // the end of a jpg image
                if ((data.Length >= 2) && (data[data.Length - 2] == 0xFF) && (data[data.Length - 1] == 0xD9))
                {
#if DEBUG
                    Debug.Print("End of JPG");
#endif
                    if ((currentImageTimestamp - lastSentImage).Minutes >= IMAGE_TX_INTERVAL)
                    {
                        new Thread(new ThreadStart(StartTransmitImageThread)).Start();
                    }
                }

            }
            catch (Exception e)
            {
                //Debug.Print(e.Message);
            }
        }

        /// <summary>
        /// Starts the transmission of a camera image.
        /// </summary>
        private void StartTransmitImageThread()
        {
            FileStream fileHandle = new FileStream(currentImageFilename, FileMode.Open);
            lastSentImage = currentImageTimestamp;

            txQueue.Add(dataProtocol.GetBeginImage(lastSentImage, (int)fileHandle.Length));
            byte[] chunk = new byte[IMAGE_CHUNK_SIZE];
            int imgOffset = 0;
            while (fileHandle.Position < fileHandle.Length)
            {
                int length = fileHandle.Read(chunk, 0, IMAGE_CHUNK_SIZE);
                txQueue.Add(dataProtocol.GetImageData(imgOffset, chunk, length));
                imgOffset += length;
                Thread.Sleep(500);
            }
            fileHandle.Close();
        }

        /// <summary>
        /// Stores a chunk of image data to the SD card.
        /// </summary>
        /// <param name="data">the data to save</param>
        private void StoreImageChunk(byte[] data)
        {
            // save the jpg image part on SD card
            FileStream fileHandle = new FileStream(currentImageFilename, FileMode.OpenOrCreate);
            //Debug.Print("Image chunksize = " + data.Length + " Bytes");
            fileHandle.Position = fileHandle.Length;
            fileHandle.Write(data, 0, data.Length);
            fileHandle.Close();
        }

        /// <summary>
        /// Resets the XBee module.
        /// </summary>
        private void ResetXbee()
        {
            TristatePort port = new TristatePort((Cpu.Pin)FEZ_Pin.Digital.Di7, false, false, Port.ResistorMode.PullUp);
            port.Active = true;  // set port as output
            port.Write(false);   // reset the xBee module 
            Thread.Sleep(1);     // wait (at least 100us)
            port.Active = false; // set port as input
            Thread.Sleep(100);
        }

        /// <summary>
        /// Sets the Xbee transmit power
        /// </summary>
        private void SetXbeeTransmitPower()
        {
            try
            {
                byte pwr = 0;
                InputPort jp0 = new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di4, true, Port.ResistorMode.PullUp);
                InputPort jp1 = new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di5, true, Port.ResistorMode.PullUp);
                InputPort jp2 = new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di6, true, Port.ResistorMode.PullUp);
                if (jp0.Read())
                    pwr |= 1;
                if (jp1.Read())
                    pwr |= 2;
                if (jp2.Read())
                    pwr |= 4;
                if (pwr > 4)
                    pwr = 4;

                // switch to AT-Command mode
                Thread.Sleep(150);
                xBeePort.Write(Encoding.UTF8.GetBytes("+++"), 0, 3);
                Thread.Sleep(150);
                byte[] readBytes = new byte[xBeePort.BytesToRead];
                xBeePort.Read(readBytes, 0, readBytes.Length);
                if (readBytes.Length == 3)
                {
                    string readString = new String(System.Text.Encoding.UTF8.GetChars(readBytes)).TrimEnd("\r".ToCharArray());
#if DEBUG
                    Debug.Print("Enter ATmode = " + readString);
#endif
                    if (readString.IndexOf("OK") == 0)
                    {
                        // now we are in at-command mode

                        // set TX power (0=1mW, 1=23mW, 2=100mW, 3=158mW, 4=316mW)
                        xBeePort.Write(Encoding.UTF8.GetBytes("ATPL" + pwr + "\r"), 0, 6);
                        Thread.Sleep(150);
                        //readBytes = new byte[xBeePort.BytesToRead];
                        //xBeePort.Read(readBytes, 0, readBytes.Length);
                        //readString = new String(System.Text.Encoding.UTF8.GetChars(readBytes)).TrimEnd("\r".ToCharArray());
                        
#if DEBUG
                        Debug.Print("TX Power Level = " + pwr);
#endif

                        // exit at-command mode
                        xBeePort.DiscardInBuffer();
                        xBeePort.Write(Encoding.UTF8.GetBytes("ATCN\r"), 0, 5);
                        Thread.Sleep(150);
                        readBytes = new byte[xBeePort.BytesToRead];
                        xBeePort.Read(readBytes, 0, readBytes.Length);
                        readString = new String(System.Text.Encoding.UTF8.GetChars(readBytes)).TrimEnd("\r".ToCharArray());
#if DEBUG
                        Debug.Print("Exit ATmode = " + readString + "\n");
#endif
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void GetDutyCycleAndReset(SerialPort xBeePort, byte resetThreshold)
        {
            try
            {
                // switch to AT-Command mode
                Thread.Sleep(150);
                xBeePort.Write(Encoding.UTF8.GetBytes("+++"), 0, 3);
                Thread.Sleep(150);
                byte[] readBytes = new byte[xBeePort.BytesToRead];
                xBeePort.Read(readBytes, 0, readBytes.Length);
                if (readBytes.Length == 3)
                {
                    string readString = new String(System.Text.Encoding.UTF8.GetChars(readBytes)).TrimEnd("\r".ToCharArray());
#if DEBUG
                    Debug.Print("Enter ATmode = " + readString);
#endif
                    if (readString.IndexOf("OK") == 0)
                    {
                        // now we are in at-command mode

                        //get DutyCycle counter (0 to 0x64) 0x64 means 10% dutycycle is reached
                        xBeePort.Write(Encoding.UTF8.GetBytes("ATDC\r"), 0, 5);
                        Thread.Sleep(150);
                        readBytes = new byte[xBeePort.BytesToRead];
                        xBeePort.Read(readBytes, 0, readBytes.Length);
                        readString = new String(System.Text.Encoding.UTF8.GetChars(readBytes)).TrimEnd("\r".ToCharArray());
                        Monitor.Enter(dutyCycleLock);
                        xBeeDutyCycle = (byte)BitConverter.Hex2Dec(readString);
                        Monitor.Exit(dutyCycleLock);
#if DEBUG
                        Debug.Print("DutyCycle   = " + xBeeDutyCycle + "%");
#endif

                        // exit at-command mode
                        xBeePort.DiscardInBuffer();
                        xBeePort.Write(Encoding.UTF8.GetBytes("ATCN\r"), 0, 5);
                        Thread.Sleep(150);
                        readBytes = new byte[xBeePort.BytesToRead];
                        xBeePort.Read(readBytes, 0, readBytes.Length);
                        readString = new String(System.Text.Encoding.UTF8.GetChars(readBytes)).TrimEnd("\r".ToCharArray());
#if DEBUG
                        Debug.Print("Exit ATmode = " + readString + "\n");
#endif
                        if (xBeeDutyCycle >= resetThreshold)
                        {
                            ResetXbee();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
