using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GroundControl.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class PersistenceHandler
    {
        private static string baseDirName = "m3space";
        private static string imageDirName = "images";
        private static string telemetryDirName = "telemetry";
        private static string telemetryPrefix = "telemetry_";

        const float Rad2Deg = 180.0f / (float)Math.PI;

        private string dataDirectory;
        private string telemetryFileName;
        
        /// <summary>
        /// Gets or sets the data directory.
        /// </summary>
        public string DataDirectory
        {
            get
            {
                return dataDirectory;
            }

            set
            {
                dataDirectory = value;
                if (!Directory.Exists(dataDirectory))
                    Directory.CreateDirectory(dataDirectory);
                string imageDir = dataDirectory + Path.DirectorySeparatorChar + imageDirName;
                if (!Directory.Exists(imageDir))
                    Directory.CreateDirectory(imageDir);
                string telemetryDir = dataDirectory + Path.DirectorySeparatorChar + telemetryDirName;
                if (!Directory.Exists(telemetryDir))
                    Directory.CreateDirectory(telemetryDir);
            }
        }

        /// <summary>
        /// Gets the telemetry file name.
        /// </summary>
        public string TelemetryFileName { get { return telemetryFileName; } }

        /// <summary>
        /// Constructor.
        /// Sets the default data directory.
        /// </summary>
        public PersistenceHandler()
        {
            DataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar + baseDirName;
            telemetryFileName = null;
        }

        /// <summary>
        /// Saves an image to disk.
        /// </summary>
        /// <param name="utc">the image timestamp</param>
        /// <param name="data">the image data</param>
        public void SaveImage(DateTime utc, byte[] data)
        {
            string filename = DataDirectory + Path.DirectorySeparatorChar + imageDirName + Path.DirectorySeparatorChar + utc.ToString("yyyyMMdd_HHmmss") + ".jpg";
            BinaryWriter writer = new BinaryWriter(File.Create(filename));
            writer.Write(data);
            writer.Close();
        }

        /// <summary>
        /// Initializes a new telemetry file.
        /// </summary>
        /// <param name="createDate">the date of creation</param>
        public void CreateTelemetryFile(DateTime createDate)
        {
            string filename = DataDirectory + Path.DirectorySeparatorChar + telemetryDirName + Path.DirectorySeparatorChar + telemetryPrefix + createDate.ToString("yyyyMMdd_HHmmss") + ".csv";
            CreateTelemetryFile(filename);
        }

        /// <summary>
        /// Initializes a telemetry file.
        /// </summary>
        /// <param name="filename">the file name</param>
        public void CreateTelemetryFile(string filename)
        {
            if (!File.Exists(filename))
            {
                StreamWriter writer = File.CreateText(filename);
                writer.WriteLine("UtcDate;Lat;Long;GpsAlt;PressureAlt;Heading;HSpeed;VSpeed;Sat;IntTemp;ExtTemp;Press;Vin;IntTempRaw;ExtTempRaw;VinRaw;DutyCycle");
                writer.Close();
            }
            telemetryFileName = filename;
        }

        /// <summary>
        /// Saves telemetry data on disk.
        /// Data is appended to the default telemetry file.
        /// </summary>
        /// <param name="telemetry">the telemetry data</param>
        public void SaveTelemetry(TelemetryData telemetry)
        {
            if (telemetryFileName != null)
            {
                StreamWriter writer = File.AppendText(telemetryFileName);
                writer.WriteLine(String.Format("{0:dd.MM.yyyy HH:mm:ss};{1:0.####};{2:0.####};{3:0.#};{4:0.#};{5:0.#};{6:0.#};{7:0.#};{8};{9:0.#};{10:0.#};{11:0.####};{12:0.##};{13};{14};{15};{16}",
                    telemetry.UtcTimestamp,
                    telemetry.Latitude,
                    telemetry.Longitude,
                    telemetry.GpsAltitude,
                    telemetry.PressureAltitude,
                    telemetry.Heading * Rad2Deg,
                    telemetry.HorizontalSpeed,
                    telemetry.VerticalSpeed,
                    telemetry.Satellites,
                    telemetry.IntTemperature,
                    telemetry.ExtTemperature,
                    telemetry.Pressure,
                    telemetry.Vin,
                    telemetry.IntTemperatureRaw,
                    telemetry.ExtTemperatureRaw,
                    telemetry.VinRaw,
                    telemetry.DutyCycle));
                writer.Close();
            }
        }
    }
}
