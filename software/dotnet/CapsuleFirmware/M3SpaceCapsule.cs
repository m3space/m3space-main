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
using M3Space.Capsule.Drivers;
using M3Space.Capsule.Util;


namespace M3Space.Capsule
{
    public class M3SpaceCapsule
    {
        private const int XBEE_TX_POWER = 4;    // 0=1mW, 1=25mW, 2=100mW, 3=150mW, 4=300mW
        private const uint XBEE_RESET_THRESHOLD = 40;
        private const uint XBEE_CRITICAL_THRESHOLD = 85;
        private const int TELEMETRY_TX_INTERVAL = 5;
        private const int IMAGE_TX_INTERVAL = 5;
        private const int IMAGE_CHUNK_SIZE = 64;
        private const int MOTION_BUFFER_SIZE = 10;
        

        private const string TelemetryFormat = "Utc;Lat;Lng;Alt;HSpd;VSpd;Head;Sat;IntTemp;Temp1;Temp2;Pressure;PAlt;Vin;Duty\r\n";
        private const string MotionFormat = "Utc;Ax;Ay;Az;Gx;Gy;Gz\r\n";
        private const string FileDateFormat = "yyyyMMdd_HHmmss";
        private const string DisplayDateFormat = "dd.MM.yyyy HH:mm:ss";

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

        private byte xBeeDutyCycle;
        private DateTime lastXBeeResetCheck;

        private string sdRootDirectory;
        private string currentImageFilename;
        private DateTime currentImageTimestamp;
        private DateTime lastSentImage;
        private bool imageTransmitting;
        private bool transmitReady;
        
        private GpsReader gps;

        private Mpu6050 mpu6050;
        private MotionData[] motionBuffer;
        private int motionBufferIndex;

        private Barometer barometer;
        private ushort cachedPressureAltitude;
        private ushort cachedPressure;
        private short cachedTemperature;

        private object logFileLock = new object();
        private object gpsLock = new object();
        private object dutyCycleLock = new object();
        private object barometerLock = new object();
        private AutoResetEvent waitTimeSync;

        private FileStream motionFileHandle = null;
        private FileStream telemetryFileHandle = null;
        private FileStream imageFileHandle = null;

        private LinkspriteCamera camera;

        private Xbee xbee;

        private Thread gpsThread;
        private Thread xbeeTransmitThread;
        private Thread telemetryThread;
        private Thread cameraThread;
        private Thread motionThread;
        private Thread barometerThread;

        /// <summary>
        /// Constructor.
        /// </summary>
        public M3SpaceCapsule()
        {
            CheckSDcard();

            txQueue = new BoundedBuffer();
            txBuffer = new byte[256];
            dataProtocol = new DataProtocol();
            
            sdStorage = new PersistentStorage("SD");

            tempSensor1 = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An4);
            tempSensor2 = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An5);
            vInSensor = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An2);

            currentImageTimestamp = DateTime.Now;
            lastSentImage = DateTime.Now;
            cachedGpsPoint.UtcTimestamp = DateTime.Now;

            lastXBeeResetCheck = DateTime.Now;
            xBeeDutyCycle = 0;
            imageTransmitting = false;
            transmitReady = false;
            xbee = new Xbee("COM1");

            gps = new GpsReader("COM4");
            gps.GpsDataReceived += SetTimeFromGps;

            mpu6050 = new Mpu6050();
            motionBuffer = new MotionData[MOTION_BUFFER_SIZE];
            motionBufferIndex = 0;

            barometer = new Barometer("COM2");
            cachedPressureAltitude = 0;
            cachedPressure = 0;
            cachedTemperature = 0;

            camera = new LinkspriteCamera("COM3", 38400);
            camera.ImageChunkReceived += CameraImageDataReceived;

            waitTimeSync = new AutoResetEvent(false);
        }

        private void CheckSDcard()
        {
            // wait for SD-card
            OnboardLed.Blink(100);
            while (!PersistentStorage.DetectSDCard())
            {
#if DEBUG
                Debug.Print("SD card missing");
#endif
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
                xbeeTransmitThread = new Thread(new ThreadStart(StartXbeeTransmitThread));
                telemetryThread = new Thread(new ThreadStart(StartTelemetryThread));
                cameraThread = new Thread(new ThreadStart(StartCameraThread));
                motionThread = new Thread(new ThreadStart(StartMotionThread));
                barometerThread = new Thread(new ThreadStart(StartBarometerThread));

                // start GPS thread to get time.
                gpsThread.Start();

                // wait until system time is synchronized.
                waitTimeSync.WaitOne();

                // initialize SD card files
                string now = DateTime.Now.ToString(FileDateFormat);
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

                // start all threads
                xbeeTransmitThread.Start();
                barometerThread.Start();
                telemetryThread.Start();
                cameraThread.Start();
                motionThread.Start();

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
        private void GpsDataReceived(GpsPoint gpsPoint)
        {
            Monitor.Enter(gpsLock);
            // compute vertical speed
            float timediff = (gpsPoint.UtcTimestamp.Ticks - cachedGpsPoint.UtcTimestamp.Ticks) * 1e-7f;
            if (timediff > 0.0f)
            {
                gpsPoint.VerticalSpeed = (gpsPoint.Altitude - cachedGpsPoint.Altitude) / timediff;
            }
            else
            {
                gpsPoint.VerticalSpeed = cachedGpsPoint.VerticalSpeed;
            }
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
            Debug.Print("Time synchronized to " + gpsPoint.UtcTimestamp.ToString(FileDateFormat));
#endif
            // reinitialize some datetime objects
            currentImageTimestamp = DateTime.Now;
            lastSentImage = DateTime.Now;
            cachedGpsPoint.UtcTimestamp = DateTime.Now;
            lastXBeeResetCheck = DateTime.Now;

            waitTimeSync.Set();
            gps.GpsDataReceived += GpsDataReceived;
        }

        /// <summary>
        /// Starts the XBee transmitter thread.
        /// </summary>
        private void StartXbeeTransmitThread()
        {
            xbee.Initialize();

            // set XBee power level
            //xbee.SetTransmitPower(XBEE_TX_POWER);

            transmitReady = true;

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
                    xbee.Send(txBuffer, 0, len);
                    Thread.Sleep(100);

                    // read dutycycle value every 2 minutes and reset xBee if value >= 40%
                    
                    if ((!imageTransmitting) && ((DateTime.Now - lastXBeeResetCheck).Minutes >= 2))
                    {
                        transmitReady = false;
                        uint dc = xbee.GetDutyCycle();
                        Monitor.Enter(dutyCycleLock);
                        xBeeDutyCycle = (byte)dc;
                        Monitor.Exit(dutyCycleLock);
                        if ((dc >= XBEE_CRITICAL_THRESHOLD) && (dc < Byte.MaxValue))
                        {
                            // pause all transmissions to recover
                            Thread.Sleep(60000);
                        }
                        else if ((dc >= XBEE_RESET_THRESHOLD) && (dc < Byte.MaxValue))
                        {
                            xbee.Reset();
                            Thread.Sleep(1000);
                        }                        
                        lastXBeeResetCheck = DateTime.Now;
                        transmitReady = true;
                    }                    
                }
                catch (Exception e)
                {
#if DEBUG
                    Debug.Print(e.Message);
#endif
                    LogError("Transmit failure");
                    Thread.Sleep(500);
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
                if (count <= 0)
                {
                    // transmit data
                    if (transmitReady)
                    {
                        txQueue.Add(dataProtocol.GetTelemetry(cachedTelemetry));
                        count = TELEMETRY_TX_INTERVAL;
                    }                    
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Starts the gyro/accelerometer thread.
        /// </summary>
        private void StartMotionThread()
        {
            if (mpu6050.Initialize())
            {
                motionBufferIndex = 0;
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
        }

        /// <summary>
        /// Starts the barometer thread.
        /// </summary>
        private void StartBarometerThread()
        {
            ushort alt;
            ushort p;
            short t;
            barometer.Initialize();
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
                string text = motionBuffer[i].UtcTimestamp.ToString(DisplayDateFormat) + ';' +
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
            string text = data.UtcTimestamp.ToString(DisplayDateFormat) + ';' +
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
            camera.Initialize();
            Thread.Sleep(5000);     // wait for camera to initialize
            camera.FlushInput();
            imageTransmitting = false;

            while (true)
            {
                if (camera.CaptureImage())
                {
                    if (transmitReady && (!imageTransmitting) && ((currentImageTimestamp - lastSentImage).Minutes >= IMAGE_TX_INTERVAL))
                    {
                        new Thread(new ThreadStart(StartTransmitImageThread)).Start();
                    }
                    Thread.Sleep(60000);
                }
                else
                {
                    LogError("Capture failed");
                    camera.Reset();
                    Thread.Sleep(5000);
                }
            }
        }

        /// <summary>
        /// Processes image data received from camera.
        /// </summary>
        /// <param name="data">the data</param>
        private void CameraImageDataReceived(byte[] chunk, int chunkSize, bool complete)
        {

            // the begin of a new jpg image
            if ((chunkSize >= 2) && (chunk[0] == 0xFF) && (chunk[1] == 0xD8))
            {
#if DEBUG
                Debug.Print("Begin of JPG");
#endif
                if (imageFileHandle != null)
                {
                    imageFileHandle.Close();
                }
                currentImageTimestamp = DateTime.Now;
                currentImageFilename = sdRootDirectory + @"\" + currentImageTimestamp.ToString(FileDateFormat) + ".jpg";
                imageFileHandle = new FileStream(currentImageFilename, FileMode.OpenOrCreate);
            }

            StoreImageChunk(chunk, chunkSize);

            // the end of a jpg image
            if (complete)
            {
#if DEBUG
                Debug.Print("End of JPG");
#endif
                if (imageFileHandle != null)
                {
                    imageFileHandle.Close();
                }
                imageFileHandle = null;
            }
        }

        /// <summary>
        /// Starts the transmission of a camera image.
        /// </summary>
        private void StartTransmitImageThread()
        {
            imageTransmitting = true;
            FileStream fileHandle = new FileStream(currentImageFilename, FileMode.Open);
            lastSentImage = currentImageTimestamp;

            while (!transmitReady)
            {
                Thread.Sleep(150);
            }
            int imgSize = (int)fileHandle.Length;
            txQueue.Add(dataProtocol.GetBeginImage(lastSentImage, imgSize));
            byte[] chunk = new byte[IMAGE_CHUNK_SIZE];
            int imgOffset = 0;
            while (imgOffset < imgSize)
            {
                int length = fileHandle.Read(chunk, 0, IMAGE_CHUNK_SIZE);
                if (length > 0)
                {
                    while (!transmitReady)
                    {
                        Thread.Sleep(150);
                    }
                    txQueue.Add(dataProtocol.GetImageData(imgOffset, chunk, length));
                    imgOffset += length;
                    Thread.Sleep(300);
                }
            }
            fileHandle.Close();
            imageTransmitting = false;
        }

        /// <summary>
        /// Stores a chunk of image data to the SD card.
        /// </summary>
        /// <param name="data">the data to save</param>
        /// <param name="size">the data size</param>
        private void StoreImageChunk(byte[] data, int size)
        {
            if (imageFileHandle == null)
            {
                // missed begin of JPG
                imageFileHandle = new FileStream(currentImageFilename, FileMode.OpenOrCreate);
            }
            if (!imageFileHandle.Name.Equals(currentImageFilename))
            {
                imageFileHandle.Close();
                imageFileHandle = new FileStream(currentImageFilename, FileMode.OpenOrCreate);
            }
            imageFileHandle.Position = imageFileHandle.Length;
            imageFileHandle.Write(data, 0, size);
            imageFileHandle.Flush();
        }

        /// <summary>
        /// Checks if any threads have crashed.
        /// </summary>
        public void CheckThreads()
        {
            if (!gpsThread.IsAlive)
            {
                gpsThread = new Thread(new ThreadStart(StartGpsThread));
                gpsThread.Start();
                LogError("GPS Thread restarted");
            }
            if (!xbeeTransmitThread.IsAlive)
            {
                xbeeTransmitThread = new Thread(new ThreadStart(StartXbeeTransmitThread));
                xbeeTransmitThread.Start();
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
            Monitor.Enter(logFileLock);
            FileStream fileHandle = new FileStream(errorLogFilename, FileMode.OpenOrCreate);
            fileHandle.Position = fileHandle.Length;
            string text = DateTime.Now.ToString(DisplayDateFormat) + ";" + message + "\r\n";
            fileHandle.Write(Encoding.UTF8.GetBytes(text), 0, text.Length);
            fileHandle.Close();
#if DEBUG
            Debug.Print(text);
#endif
            Monitor.Exit(logFileLock);
        }
    }
}
