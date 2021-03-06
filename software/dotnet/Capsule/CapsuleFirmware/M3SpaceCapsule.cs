using System;
using System.IO;
using System.Text;
using System.Threading;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.IO;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.IO;
using M3Space.Capsule.Drivers;


namespace M3Space.Capsule
{
    /// <summary>
    /// M3 Space Balloon Capsule.
    /// Version 6.0.0
    /// Made some components optional for Mission 6.
    /// </summary>
    public class M3SpaceCapsule
    {
        private const int XBEE_TX_POWER = 4;    // 0=1mW, 1=25mW, 2=100mW, 3=150mW, 4=300mW
        private const uint XBEE_RESET_THRESHOLD = 40;
        private const uint XBEE_CRITICAL_THRESHOLD = 80;
        private const int TELEMETRY_TX_INTERVAL = 5;
#if WITH_LIVE_IMAGES && WITH_CAMERA
        private const int IMAGE_TX_INTERVAL = 5;
        private const int IMAGE_CHUNK_SIZE = 64;
#endif
#if WITH_MOTION
        private const int MOTION_BUFFER_SIZE = 10;
#endif

        private const string TelemetryFormat = "Utc;Lat;Lng;Alt;HSpd;VSpd;Head;Sat;IntTemp;Temp1;Temp2;Pressure;PAlt;Vin;Duty;Gamma\r\n";
#if WITH_MOTION
        private const string MotionFormat = "Utc;Ax;Ay;Az;Gx;Gy;Gz\r\n";
#endif
        private const string FileDateFormat = "yyyyMMdd_HHmmss";
        private const string DisplayDateFormat = "dd.MM.yyyy HH:mm:ss.fff";

        private BoundedBuffer txQueue;
        private byte[] txBuffer;
        private DataProtocol dataProtocol;
        private byte xBeeDutyCycle;
        private DateTime lastXBeeResetCheck;
        private bool transmitReady;

        private PersistentStorage sdStorage;
        private string sdRootDirectory;
        private string telemetryFileName;
        private string logFileName;

        private AnalogIn tempSensor1;
        private AnalogIn tempSensor2;
        private AnalogIn vInSensor;
#if WITH_GAMMA
        private InterruptPort gammaInput;
#endif

        private TelemetryData cachedTelemetry;
        private GpsPoint cachedGpsPoint;

#if WITH_CAMERA
        private string currentImageFileName;
        private DateTime currentImageTimestamp;
        private DateTime lastSentImage;        
#endif
        private bool imageTransmitting;     // required for XBee transmission, even without live images
        
        private GpsReader gps;

#if WITH_MOTION
        private Mpu6050 mpu6050;
        private MotionData[] motionBuffer;
        private int motionBufferIndex;
        private FileStream motionFileHandle = null;
        private Thread motionThread;
        private string motionFileName;
#endif

#if WITH_BAROMETER
        private Barometer barometer;
        private ushort cachedPressureAltitude;
        private ushort cachedPressure;
        private short cachedTemperature;
#endif

        private object logFileLock = new object();
        private object gpsLock = new object();
        private object dutyCycleLock = new object();
#if WITH_BAROMETER
        private object barometerLock = new object();
#endif
        private AutoResetEvent waitTimeSync;

        private FileStream telemetryFileHandle = null;        

#if WITH_CAMERA
        private FileStream imageFileHandle = null;
        private LinkspriteCamera camera;
        private Thread cameraThread;
#endif

        private Xbee xbee;

        private Thread gpsThread;
        private Thread xbeeTransmitThread;
        private Thread telemetryThread;

#if WITH_BAROMETER
        private Thread barometerThread;
#endif

        /// <summary>
        /// Constructor.
        /// </summary>
        public M3SpaceCapsule()
        {
#if WITH_GAMMA
            gammaInput = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.An0, false, Port.ResistorMode.PullDown, Port.InterruptMode.InterruptEdgeHigh);
#endif

            vInSensor = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An2);
            tempSensor1 = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An4);
            tempSensor2 = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An5);

            CheckSDcard();

            txQueue = new BoundedBuffer();
            txBuffer = new byte[256];
            dataProtocol = new DataProtocol();
            
            sdStorage = new PersistentStorage("SD");

#if WITH_GAMMA
            cachedTelemetry.GammaCount = 0;
            gammaInput.OnInterrupt += new NativeEventHandler(OnGammaPulseDetected);
            gammaInput.EnableInterrupt();
#endif

            cachedGpsPoint.UtcTimestamp = DateTime.Now;

            lastXBeeResetCheck = DateTime.Now;
            xBeeDutyCycle = 0;            
            transmitReady = false;
            xbee = new Xbee("COM1");

            gps = new GpsReader("COM4");
            gps.GpsDataReceived += SetTimeFromGps;

#if WITH_MOTION
            mpu6050 = new Mpu6050();
            motionBuffer = new MotionData[MOTION_BUFFER_SIZE];
            motionBufferIndex = 0;
#endif
#if WITH_BAROMETER
            barometer = new Barometer("COM2");
            cachedPressureAltitude = 0;
            cachedPressure = 0;
            cachedTemperature = 0;
#endif

            imageTransmitting = false;
#if WITH_CAMERA
            currentImageTimestamp = DateTime.Now;
            lastSentImage = DateTime.Now;
            camera = new LinkspriteCamera("COM3", 38400);
            camera.ImageChunkReceived += CameraImageDataReceived;
#endif

            waitTimeSync = new AutoResetEvent(false);
        }

        /// <summary>
        /// Checks if the SD card is attached.
        /// </summary>
        private void CheckSDcard()
        {
            // wait for SD-card
            OnboardLed.Blink(50);
#if DEBUG
            Debug.Print("Looking for SD card");
#endif
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
#if DEBUG
                    Debug.Print("Mounted SD card");
#endif
                sdRootDirectory = VolumeInfo.GetVolumes()[0].RootDirectory;
                if (!Directory.Exists(sdRootDirectory + @"\images\"))
                {
                    Directory.CreateDirectory(sdRootDirectory + @"\images\");
#if DEBUG
                    Debug.Print("Create image directory");
#endif
                }

                // create threads
                gpsThread = new Thread(new ThreadStart(StartGpsThread));
                xbeeTransmitThread = new Thread(new ThreadStart(StartXbeeTransmitThread));
                telemetryThread = new Thread(new ThreadStart(StartTelemetryThread));
#if WITH_CAMERA
                cameraThread = new Thread(new ThreadStart(StartCameraThread));
#endif
#if WITH_MOTION
                motionThread = new Thread(new ThreadStart(StartMotionThread));
#endif
#if WITH_BAROMETER
                barometerThread = new Thread(new ThreadStart(StartBarometerThread));
#endif

                // start GPS thread to get time.
                gpsThread.Start();

                // wait until system time is synchronized.
                waitTimeSync.WaitOne();

                // initialize SD card files
                string now = DateTime.Now.ToString(FileDateFormat);
                telemetryFileName = sdRootDirectory + @"\telemetry_" + now + ".csv";
#if WITH_MOTION
                motionFileName = sdRootDirectory + @"\motiondata_" + now + ".csv";
#endif
                logFileName = sdRootDirectory + @"\log_" + now + ".csv";
#if WITH_CAMERA
                currentImageFileName = sdRootDirectory + @"\images\" + now + ".jpg";
#endif
                telemetryFileHandle = new FileStream(telemetryFileName, FileMode.OpenOrCreate);
                byte[] writeData = Encoding.UTF8.GetBytes(TelemetryFormat);
                telemetryFileHandle.Position = telemetryFileHandle.Length;
                telemetryFileHandle.Write(writeData, 0, writeData.Length);
                telemetryFileHandle.Flush();

#if WITH_MOTION
                motionFileHandle = new FileStream(motionFileName, FileMode.OpenOrCreate);
                writeData = Encoding.UTF8.GetBytes(MotionFormat);
                motionFileHandle.Position = motionFileHandle.Length;
                motionFileHandle.Write(writeData, 0, writeData.Length);
                motionFileHandle.Flush();
#endif

                // start all threads
                xbeeTransmitThread.Start();
#if WITH_BAROMETER
                barometerThread.Start();
#endif
                telemetryThread.Start();
#if WITH_CAMERA
                cameraThread.Start();
#endif
#if WITH_MOTION
                motionThread.Start();
#endif

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
#if DEBUG
            Debug.Print("GPS initialized");
#endif
            OnboardLed.Blink(500);
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
            OnboardLed.Off();
            gps.GpsDataReceived -= SetTimeFromGps;          // deactivate handler
            Utility.SetLocalTime(gpsPoint.UtcTimestamp);    // set system time to UTC, therefore DateTime.Now returns UTC.
#if DEBUG
            Debug.Print("Time synchronized to " + gpsPoint.UtcTimestamp.ToString(DisplayDateFormat));
#endif
            // reinitialize some datetime objects
#if WITH_CAMERA
            currentImageTimestamp = DateTime.Now;
            lastSentImage = DateTime.Now;
#endif
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
                            xbee.Reset();
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
            cachedTelemetry.GammaCount = 0;

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
#if WITH_BAROMETER
                Monitor.Enter(barometerLock);
                cachedTelemetry.PressureAltitude = cachedPressureAltitude;
                cachedTelemetry.Pressure = cachedPressure;
                cachedTelemetry.IntTemperature = cachedTemperature;
                Monitor.Exit(barometerLock);
#else
                cachedTelemetry.PressureAltitude = 0;
                cachedTelemetry.Pressure = 0;
                cachedTelemetry.IntTemperature = 0;
#endif
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

#if WITH_MOTION
        /// <summary>
        /// Starts the gyro/accelerometer thread.
        /// </summary>
        private void StartMotionThread()
        {
            Thread.Sleep(100);
            if (mpu6050.Initialize())
            {
                Thread.Sleep(1000);
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
#endif

#if WITH_BAROMETER
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
#endif

        /// <summary>
        /// Stores telemetry data on SD card.
        /// </summary>
        /// <param name="data">the telemetry data to save</param>
        private void StoreTelemetry(TelemetryData data)
        {
            string text = data.UtcTimestamp.ToString(DisplayDateFormat) + ';' +
                data.GpsData.Latitude.ToString("F5") + ';' +
                data.GpsData.Longitude.ToString("F5") + ';' +
                data.GpsData.Altitude.ToString("F2") + ';' +
                data.GpsData.HorizontalSpeed.ToString("F2") + ';' +
                data.GpsData.VerticalSpeed.ToString("F2") + ';' +
                data.GpsData.Heading.ToString("F2") + ';' +
                data.GpsData.Satellites.ToString() + ';' +
                data.IntTemperature.ToString() + ';' +
                data.Temperature1Raw.ToString() + ';' +
                data.Temperature2Raw.ToString() + ';' +
                data.Pressure.ToString() + ';' +
                data.PressureAltitude.ToString() + ';' +
                data.VinRaw.ToString() + ';' +
                data.DutyCycle.ToString() + ';' +
                data.GammaCount.ToString() +
                "\r\n";

            byte[] writeData = Encoding.UTF8.GetBytes(text);

            telemetryFileHandle.Position = telemetryFileHandle.Length;
            telemetryFileHandle.Write(writeData, 0, writeData.Length);
            telemetryFileHandle.Flush();
        }

#if WITH_CAMERA
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
                    camera.Stop();
#if DEBUG
                    Debug.Print("Image captured.");
#endif
#if WITH_LIVE_IMAGES
                    if (transmitReady && (!imageTransmitting) && ((currentImageTimestamp - lastSentImage).Minutes >= IMAGE_TX_INTERVAL))
                    {
                        new Thread(new ThreadStart(StartTransmitImageThread)).Start();
                    }
#if DEBUG
                    else
                    {
                        Debug.Print("Do not transmit image.");
                    }
#endif
#endif
                    Thread.Sleep(60000);
                }
                else
                {
#if DEBUG
                    Debug.Print("Image capture failed.");
#endif
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
#if (DEBUG && VERBOSE)
                Debug.Print("Begin of JPG");
#endif
                if (imageFileHandle != null)
                {
                    imageFileHandle.Close();
                }
                currentImageTimestamp = DateTime.Now;
                currentImageFileName = sdRootDirectory + @"\images\" + currentImageTimestamp.ToString(FileDateFormat) + ".jpg";
                imageFileHandle = new FileStream(currentImageFileName, FileMode.OpenOrCreate);
            }

            StoreImageChunk(chunk, chunkSize);

            // the end of a jpg image
            if (complete)
            {
#if (DEBUG && VERBOSE)
                Debug.Print("End of JPG");
#endif
                if (imageFileHandle != null)
                {
                    imageFileHandle.Close();
                }
                imageFileHandle = null;
            }
        }

#if WITH_LIVE_IMAGES
        /// <summary>
        /// Starts the transmission of a camera image.
        /// </summary>
        private void StartTransmitImageThread()
        {
#if DEBUG
            Debug.Print("Image transmission start.");
#endif
            string fileToSend = currentImageFileName;
            lastSentImage = currentImageTimestamp;
            FileStream fileHandle = new FileStream(fileToSend, FileMode.Open);
            int imgSize = (int)fileHandle.Length;
            byte[] chunk = new byte[IMAGE_CHUNK_SIZE];
            while (!transmitReady)
            {
                Thread.Sleep(150);
            }
            imageTransmitting = true;            
            txQueue.Add(dataProtocol.GetBeginImage(lastSentImage, imgSize));            
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
                    Thread.Sleep(150);
                }
                else
                {
                    break;
                }
            }
            Thread.Sleep(250);
            imageTransmitting = false;
            fileHandle.Close();
            
#if DEBUG
            Debug.Print("Image transmission end.");
#endif
        }
#endif

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
                imageFileHandle = new FileStream(currentImageFileName, FileMode.OpenOrCreate);
            }
            if (!imageFileHandle.Name.Equals(currentImageFileName))
            {
                imageFileHandle.Close();
                imageFileHandle = new FileStream(currentImageFileName, FileMode.OpenOrCreate);
            }
            imageFileHandle.Position = imageFileHandle.Length;
            imageFileHandle.Write(data, 0, size);
            imageFileHandle.Flush();
        }
#endif

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
#if WITH_CAMERA
            if (!cameraThread.IsAlive)
            {
                cameraThread = new Thread(new ThreadStart(StartCameraThread));
                cameraThread.Start();
                LogError("Camera Thread restarted");
            }
#endif
#if WITH_MOTION
            if (!motionThread.IsAlive)
            {
                motionThread = new Thread(new ThreadStart(StartMotionThread));
                motionThread.Start();
                LogError("Motion Thread restarted");
            }
#endif
#if WITH_BAROMETER
            if (!barometerThread.IsAlive)
            {
                barometerThread = new Thread(new ThreadStart(StartBarometerThread));
                barometerThread.Start();
                LogError("Barometer Thread restarted");
            }
#endif
        }

        /// <summary>
        /// Writes an error message to a log file.
        /// </summary>
        /// <param name="message">the error message</param>
        private void LogError(string message)
        {
            Monitor.Enter(logFileLock);
            FileStream fileHandle = new FileStream(logFileName, FileMode.OpenOrCreate);
            fileHandle.Position = fileHandle.Length;
            string text = DateTime.Now.ToString(DisplayDateFormat) + ";" + message + "\r\n";
            fileHandle.Write(Encoding.UTF8.GetBytes(text), 0, text.Length);
            fileHandle.Close();
#if DEBUG
            Debug.Print(text);
#endif
            Monitor.Exit(logFileLock);
        }

#if WITH_GAMMA
        /// <summary>
        /// Received impulse from geiger counter.
        /// </summary>
        /// <param name="port">the port</param>
        /// <param name="state">the state</param>
        /// <param name="time">the event timestamp</param>
        private void OnGammaPulseDetected(uint port, uint state, DateTime time)
        {
            cachedTelemetry.GammaCount++;
#if DEBUG
            Debug.Print("Gamma pulse");
#endif
        }
#endif
    }
}
