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
        private const int IMAGE_CHUNK_SIZE = 64;
        private const int MOTION_BUFFER_SIZE = 10;

        private const string TelemetryFormat = "Utc;Lat;Lng;Alt;HSpd;VSpd;Head;Sat;IntTemp;Temp1;Temp2;Pressure;PAlt;Vin;Duty\r\n";
        private const string MotionFormat = "Utc;Ax;Ay;Az;Gx;Gy;Gz\r\n";

        private BoundedBuffer txQueue;
        private byte[] txBuffer;
        private DataProtocol dataProtocol;

        private PersistentStorage sdStorage;
        private string telemetryFileName;
        private string motionFileName;
        private string errorLogFilename;

        private AnalogIn tempSensor1;
        private AnalogIn tempSensor2;
        private AnalogIn vInSensor;

        private TelemetryData cachedTelemetry;
        private GpsPoint cachedGpsPoint;

        private SerialPort gpsPort;
        private SerialPort xBeePort;
        private SerialPort cameraPort;
        private SerialPort barometerPort;

        private byte xBeeDutyCycle;
        private DateTime lastXBeeResetCheck;

        private string sdRootDirectory;
        private string currentImageFilename;
        private DateTime currentImageTimestamp;
        private DateTime lastSentImage;
        
        private GpsReader2 gps;

        private Mpu6050 mpu6050;
        private MotionData[] motionBuffer;
        private int motionBufferIndex;

        private Barometer barometer;
        private ushort cachedPressureAltitude;
        private ushort cachedPressure;
        private short cachedTemperature;

        private object gpsLock = new object();
        private object dutyCycleLock = new object();
        private object barometerLock = new object();
        private AutoResetEvent waitTimeSync;

        private FileStream motionFileHandle = null;
        private FileStream telemetryFileHandle = null;
        private FileStream imageFileHandle = null;

        private Thread gpsThread;
        private Thread transmitThread;
        private Thread telemetryThread;
        private Thread cameraThread;
        private Thread motionThread;
        private Thread barometerThread;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SpaceBalloon()
        {
            CheckSDcard();

            txQueue = new BoundedBuffer();
            txBuffer = new byte[256];
            dataProtocol = new DataProtocol();
            
            sdStorage = new PersistentStorage("SD");

            tempSensor1 = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An4);
            tempSensor2 = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An5);
            vInSensor = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An2);

            gpsPort = new SerialPort("COM4");
            xBeePort = new SerialPort("COM1", 38400, Parity.None, 8, StopBits.One);
            cameraPort = new SerialPort("COM3", 38400, Parity.None, 8, StopBits.One);
            barometerPort = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);

            currentImageTimestamp = DateTime.Now;
            lastSentImage = DateTime.Now;
            cachedGpsPoint.UtcTimestamp = DateTime.Now;

            lastXBeeResetCheck = DateTime.Now;
            xBeeDutyCycle = 0;

            gps = new GpsReader2(gpsPort);
            gps.GpsDataReceived += SetTimeFromGps;

            mpu6050 = new Mpu6050();
            motionBuffer = new MotionData[MOTION_BUFFER_SIZE];
            motionBufferIndex = 0;

            barometer = new Barometer(barometerPort);
            cachedPressureAltitude = 0;
            cachedPressure = 0;
            cachedTemperature = 0;

            waitTimeSync = new AutoResetEvent(false);
        }

        private void CheckSDcard()
        {
            // wait for SD-card
            OnboardLed.Blink(100);
            while (!PersistentStorage.DetectSDCard())
            {
                Thread.Sleep(1000);
            }
            OnboardLed.Off();
        }


        /// <summary>
        /// Initializes the balloon software.
        /// </summary>
        public bool Initialize()
        {
            try
            {
                sdStorage.MountFileSystem();
                sdRootDirectory = VolumeInfo.GetVolumes()[0].RootDirectory;

                // create threads
                gpsThread = new Thread(new ThreadStart(StartGpsThread));
                transmitThread = new Thread(new ThreadStart(StartTransmitThread));
                telemetryThread = new Thread(new ThreadStart(StartTelemetryThread));
                cameraThread = new Thread(new ThreadStart(StartCameraThread));
                motionThread = new Thread(new ThreadStart(StartMotionThread));
                barometerThread = new Thread(new ThreadStart(StartBarometerThread));

                // start GPS thread to get time.
                gpsThread.Start();

                // wait until system time is synchronized.
                waitTimeSync.WaitOne();

                // initialize SD card files
                string now = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                telemetryFileName = sdRootDirectory + @"\telemetry_" + now + ".csv";
                motionFileName = sdRootDirectory + @"\motiondata_" + now + ".csv";
                errorLogFilename = sdRootDirectory + @"\errorLog_" + now + ".csv";
                currentImageFilename = sdRootDirectory + @"\" + now + ".jpg";

                telemetryFileHandle = new FileStream(telemetryFileName, FileMode.OpenOrCreate);
                byte[] writeData = Encoding.UTF8.GetBytes(TelemetryFormat);
                telemetryFileHandle.Position = telemetryFileHandle.Length;
                telemetryFileHandle.Write(writeData, 0, writeData.Length);
                telemetryFileHandle.Flush();

                motionFileHandle = new FileStream(motionFileName, FileMode.OpenOrCreate);
                writeData = Encoding.UTF8.GetBytes(MotionFormat);
                motionFileHandle.Position = motionFileHandle.Length;
                motionFileHandle.Write(writeData, 0, writeData.Length);
                motionFileHandle.Flush();

                barometer.Initialize();

                // start all threads
                transmitThread.Start();
                telemetryThread.Start();
                cameraThread.Start();
                motionThread.Start();
                barometerThread.Start();

                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.Print("Initialization failed: " + e.Message);
#endif
                return false;
            }
        }


        /// <summary>
        /// Starts the GPS receiver thread.
        /// </summary>
        private void StartGpsThread()
        {
            gps.Initialize();

            while (true)
            {
                gps.ReadNmeaData();
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
            float timediff = (gpsPoint.UtcTimestamp.Ticks - cachedGpsPoint.UtcTimestamp.Ticks) * 1e-7f;
            if (timediff != 0)
                gpsPoint.VerticalSpeed = (gpsPoint.Altitude - cachedGpsPoint.Altitude) / timediff;
            else
                gpsPoint.VerticalSpeed = 0;
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
                cachedTelemetry.Temperature1Raw = (ushort)tempSensor1.Read();
                cachedTelemetry.Temperature2Raw = (ushort)tempSensor2.Read();
                cachedTelemetry.VinRaw = (ushort)vInSensor.Read();
                Monitor.Enter(gpsLock);
                cachedTelemetry.GpsData = cachedGpsPoint;
                Monitor.Exit(gpsLock);
                Monitor.Enter(dutyCycleLock);
                cachedTelemetry.DutyCycle = xBeeDutyCycle;
                Monitor.Exit(dutyCycleLock);
                Monitor.Enter(barometerLock);
                cachedTelemetry.PressureAltitude = cachedPressureAltitude;
                cachedTelemetry.Pressure = cachedPressure;
                cachedTelemetry.IntTemperature = cachedTemperature;
                Monitor.Exit(barometerLock);
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
        /// Starts the gyro/accelerometer thread.
        /// </summary>
        private void StartMotionThread()
        {
            mpu6050.Initialize();
            while (true)
            {
                if (mpu6050.GetMotionData(ref motionBuffer[motionBufferIndex++]))
                {
                    if (motionBufferIndex == MOTION_BUFFER_SIZE)
                    {
                        StoreMotionBuffer();
                        motionBufferIndex = 0;
                    }
                }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Starts the barometer thread.
        /// </summary>
        private void StartBarometerThread()
        {
            ushort alt;
            ushort p;
            short t;
            while (true)
            {
                alt = barometer.GetAltitude();
                Thread.Sleep(300);
                p = barometer.GetPressure();
                Thread.Sleep(300);
                t = barometer.GetTemperature();
                Monitor.Enter(barometerLock);
                cachedPressureAltitude = alt;
                cachedPressure = p;
                cachedTemperature = t;
                Monitor.Exit(barometerLock);

                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// Stores the buffered gyro/accelerometer data.
        /// </summary>
        private void StoreMotionBuffer()
        {
            for (int i = 0; i < MOTION_BUFFER_SIZE; i++)
            {
                string text = motionBuffer[i].UtcTimestamp.ToString("dd.MM.yyyy HH:mm:ss") + ';' +
                    motionBuffer[i].Ax.ToString() + ';' +
                    motionBuffer[i].Ay.ToString() + ';' +
                    motionBuffer[i].Az.ToString() + ';' +
                    motionBuffer[i].Gx.ToString() + ';' +
                    motionBuffer[i].Gy.ToString() + ';' +
                    motionBuffer[i].Gz.ToString() +
                    "\r\n";

                byte[] writeData = Encoding.UTF8.GetBytes(text);

                motionFileHandle.Position = motionFileHandle.Length;
                motionFileHandle.Write(writeData, 0, writeData.Length);
            }
            motionFileHandle.Flush();
        }

        /// <summary>
        /// Stores telemetry data on SD card.
        /// </summary>
        /// <param name="data">the telemetry data to save</param>
        private void StoreTelemetry(TelemetryData data)
        {
            string text = data.UtcTimestamp.ToString("dd.MM.yyyy HH:mm:ss") + ';' +
                data.GpsData.Latitude.ToString("F5") + ';' +
                data.GpsData.Longitude.ToString("F5") + ';' +
                data.GpsData.Altitude.ToString() + ';' +
                data.GpsData.HorizontalSpeed.ToString("F2") + ';' +
                data.GpsData.VerticalSpeed.ToString("F2") + ';' +
                data.GpsData.Heading.ToString() + ';' +
                data.GpsData.Satellites.ToString() + ';' +
                data.IntTemperature.ToString() + ';' +
                data.Temperature1Raw.ToString() + ';' +
                data.Temperature2Raw.ToString() + ';' +
                data.Pressure.ToString() + ';' +
                data.PressureAltitude.ToString() + ';' +
                data.VinRaw.ToString() + ';' +
                data.DutyCycle.ToString() +
                "\r\n";

            byte[] writeData = Encoding.UTF8.GetBytes(text);

            telemetryFileHandle.Position = telemetryFileHandle.Length;
            telemetryFileHandle.Write(writeData, 0, writeData.Length);
            telemetryFileHandle.Flush();
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
                    if (imageFileHandle != null && imageFileHandle.CanWrite)
                    {
                        imageFileHandle.Close();
                    }
                    currentImageTimestamp = DateTime.Now;
                    currentImageFilename = sdRootDirectory + @"\" + currentImageTimestamp.ToString("yyyyMMdd_HHmmss") + ".jpg";
                    imageFileHandle = new FileStream(currentImageFilename, FileMode.OpenOrCreate);
                }

                StoreImageChunk(data);

                // the end of a jpg image
                if ((data.Length >= 2) && (data[data.Length - 2] == 0xFF) && (data[data.Length - 1] == 0xD9))
                {
#if DEBUG
                    Debug.Print("End of JPG");
#endif
                    imageFileHandle.Close();
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
            //// save the jpg image part on SD card
            //FileStream fileHandle = new FileStream(currentImageFilename, FileMode.OpenOrCreate);
            ////Debug.Print("Image chunksize = " + data.Length + " Bytes");
            //fileHandle.Position = fileHandle.Length;
            //fileHandle.Write(data, 0, data.Length);
            //fileHandle.Close();

            if (imageFileHandle == null)
                imageFileHandle = new FileStream(currentImageFilename, FileMode.OpenOrCreate);
            if (!imageFileHandle.Name.Equals(currentImageFilename))
            {
                imageFileHandle.Close();
                imageFileHandle = new FileStream(currentImageFilename, FileMode.OpenOrCreate);
            }
            imageFileHandle.Position = imageFileHandle.Length;
            imageFileHandle.Write(data, 0, data.Length);
            imageFileHandle.Flush();
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

        public void CheckThreads()
        {
            if (!gpsThread.IsAlive)
            {
                gpsThread = new Thread(new ThreadStart(StartGpsThread));
                gpsThread.Start();
                LogError("GPS Thread restarted");
            }
            if (!transmitThread.IsAlive)
            {
                transmitThread = new Thread(new ThreadStart(StartTransmitThread));
                transmitThread.Start();
                LogError("Transmit Thread restarted");
            }
            if (!telemetryThread.IsAlive)
            {
                telemetryThread = new Thread(new ThreadStart(StartTelemetryThread));
                telemetryThread.Start();
                LogError("Telemetry Thread restarted");
            }
            if (!cameraThread.IsAlive)
            {
                cameraThread = new Thread(new ThreadStart(StartCameraThread));
                cameraThread.Start();
                LogError("Camera Thread restarted");
            }
            if (!motionThread.IsAlive)
            {
                motionThread = new Thread(new ThreadStart(StartMotionThread));
                motionThread.Start();
                LogError("Motion Thread restarted");
            }
            if (!barometerThread.IsAlive)
            {
                barometerThread = new Thread(new ThreadStart(StartBarometerThread));
                barometerThread.Start();
                LogError("Barometer Thread restarted");
            }
        }

        private void LogError(string message)
        {
            FileStream fileHandle = new FileStream(errorLogFilename, FileMode.OpenOrCreate);
            fileHandle.Position = fileHandle.Length;
            string text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ";" + message + "\r\n";
            fileHandle.Write(Encoding.UTF8.GetBytes(text), 0, text.Length);
            fileHandle.Close();
#if DEBUG
            Debug.Print(text);
#endif
        }
    }
}
