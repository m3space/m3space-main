using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GroundControl.Core
{
    /// <summary>
    /// Loads data from files.
    /// </summary>
    public class DataLoader
    {
        /// <summary>
        /// Loads telemetry data from a Ground Control CSV file.
        /// </summary>
        /// <param name="filename">the file name</param>
        /// <param name="dataCache">the data cache to store the data</param>
        /// <returns>true if loaded, false if an error occurs</returns>
        public static bool LoadTelemetryData(string filename, DataCache dataCache)
        {
            try
            {
                dataCache.Clear();
                StreamReader reader = new StreamReader(filename);
                // ignore first line
                if (!reader.EndOfStream)
                    reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(';');
                    if ((parts != null) && (parts.Length >= 17))
                    {
                        TelemetryData data = new TelemetryData();
                        data.UtcTimestamp = DateTime.ParseExact(parts[0], "dd.MM.yyyy HH:mm:ss", null);
                        data.Latitude = Single.Parse(parts[1]);
                        data.Longitude = Single.Parse(parts[2]);
                        data.GpsAltitude = Single.Parse(parts[3]);
                        data.PressureAltitude = Single.Parse(parts[4]);
                        data.Heading = Single.Parse(parts[5]);
                        data.Speed = Single.Parse(parts[6]);
                        data.Satellites = Byte.Parse(parts[7]);
                        data.IntTemperature = Single.Parse(parts[8]);
                        data.ExtTemperature = Single.Parse(parts[9]);
                        data.Pressure = Single.Parse(parts[10]);
                        data.Vin = Single.Parse(parts[11]);
                        data.IntTemperatureRaw = UInt16.Parse(parts[12]);
                        data.ExtTemperatureRaw = UInt16.Parse(parts[13]);
                        data.PressureRaw = UInt16.Parse(parts[14]);
                        data.VinRaw = UInt16.Parse(parts[15]);
                        data.DutyCycle = Byte.Parse(parts[16]);

                        dataCache.AddTelemetry(data);
                    }
                }

                reader.Close();
                return true;
            }
            catch (Exception)
            {
                dataCache.Clear();
                return false;
            }
        }
    }
}
