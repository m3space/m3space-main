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
        delegate void TelemetryParseDelegate(DataCache dataCache, string[] parts);

        /// <summary>
        /// Loads telemetry data from a Ground Control CSV file.
        /// </summary>
        /// <param name="filename">the file name</param>
        /// <param name="dataCache">the data cache to store the data</param>
        /// <returns>true if loaded, false if an error occurs</returns>
        public static bool LoadTelemetryData(string filename, DataCache dataCache)
        {
            TelemetryParseDelegate parseTelemetry = null;

            bool wasLocked = dataCache.Locked;

            try
            {
                dataCache.Clear();
                StreamReader reader = new StreamReader(filename);

                if (!reader.EndOfStream)
                {
                    string firstLine = reader.ReadLine();

                    if (firstLine.Equals(DataFormat.TelemetryFormatCurrent))
                    {
                        parseTelemetry = ParseLineCurrent;
                        dataCache.Locked = false;
                    }
                    else if (firstLine.Equals(DataFormat.TelemetryFormatV2))
                    {
                        parseTelemetry = ParseLineV2;
                        dataCache.Locked = true;
                    }
                    else if (firstLine.Equals(DataFormat.TelemetryFormatCapsule))
                    {
                        parseTelemetry = ParseLineCapsule;
                        dataCache.Locked = true;
                    }
                    else
                    {
                        dataCache.Locked = wasLocked;
                        return false;
                    }
                }

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(';');

                    parseTelemetry(dataCache, parts);
                }

                reader.Close();
                return true;
            }
            catch (Exception)
            {
                dataCache.Clear();
                dataCache.Locked = wasLocked;
                return false;
            }
        }

        /// <summary>
        /// Parses V3 live telemetry.
        /// </summary>
        /// <param name="dataCache">the data cache</param>
        /// <param name="parts">the CSV fields</param>
        private static void ParseLineCurrent(DataCache dataCache, string[] parts)
        {
            if ((parts != null) && (parts.Length >= 18))
            {
                TelemetryData data = new TelemetryData();
                data.UtcTimestamp = DateTime.ParseExact(parts[0], "dd.MM.yyyy HH:mm:ss", null);
                data.Latitude = Single.Parse(parts[1]);
                data.Longitude = Single.Parse(parts[2]);
                data.GpsAltitude = Single.Parse(parts[3]);
                data.PressureAltitude = Single.Parse(parts[4]);
                data.Heading = Int16.Parse(parts[5]);
                data.HorizontalSpeed = Single.Parse(parts[6]);
                data.VerticalSpeed = Single.Parse(parts[7]);
                data.Satellites = Byte.Parse(parts[8]);
                data.IntTemperature = Int16.Parse(parts[9]);
                data.Temperature1 = Single.Parse(parts[10]);
                data.Temperature2 = Single.Parse(parts[11]);
                data.Pressure = Single.Parse(parts[12]);
                data.Vin = Single.Parse(parts[13]);
                data.Temperature1Raw = UInt16.Parse(parts[14]);
                data.Temperature2Raw = UInt16.Parse(parts[15]);
                data.VinRaw = UInt16.Parse(parts[16]);
                data.DutyCycle = Byte.Parse(parts[17]);

                dataCache.AddTelemetry(data);
            }
        }

        /// <summary>
        /// Parses V2 live telemetry.
        /// </summary>
        /// <param name="dataCache">the data cache</param>
        /// <param name="parts">the CSV fields</param>
        private static void ParseLineV2(DataCache dataCache, string[] parts)
        {
            if ((parts != null) && (parts.Length >= 17))
            {
                TelemetryData data = new TelemetryData();
                data.UtcTimestamp = DateTime.ParseExact(parts[0], "dd.MM.yyyy HH:mm:ss", null);
                data.Latitude = Single.Parse(parts[1]);
                data.Longitude = Single.Parse(parts[2]);
                data.GpsAltitude = Single.Parse(parts[3]);
                data.PressureAltitude = Single.Parse(parts[4]);
                data.Heading = (short)(DataFormat.Rad2Deg * Single.Parse(parts[5]));
                data.HorizontalSpeed = Single.Parse(parts[6]);

                // compute vertical speed from altitude data
                data.VerticalSpeed = 0.0f;
                if (dataCache.Telemetry.Count > 0)
                {
                    TelemetryData last = dataCache.Telemetry.Last();
                    TimeSpan td = new TimeSpan(data.UtcTimestamp.Ticks - last.UtcTimestamp.Ticks);
                    if (td.TotalSeconds > 0.0)
                    {
                        data.VerticalSpeed = (data.GpsAltitude - last.GpsAltitude) / (float)td.TotalSeconds;
                    }
                }
                
                data.Satellites = Byte.Parse(parts[7]);
                data.IntTemperature = 0;    // not yet included
                data.Temperature1 = Single.Parse(parts[8]);
                data.Temperature2 = Single.Parse(parts[9]);
                data.Pressure = Single.Parse(parts[10]);
                data.Vin = Single.Parse(parts[11]);
                data.Temperature1Raw = UInt16.Parse(parts[12]);
                data.Temperature2Raw = UInt16.Parse(parts[13]);
                // PressureRaw no longer supported
                data.VinRaw = UInt16.Parse(parts[15]);
                data.DutyCycle = Byte.Parse(parts[16]);

                dataCache.AddTelemetry(data);
            }
        }

        /// <summary>
        /// Parses telemetry file from Capsule.
        /// </summary>
        /// <param name="dataCache">the data cache</param>
        /// <param name="parts">the CSV fields</param>
        private static void ParseLineCapsule(DataCache dataCache, string[] parts)
        {
            // Utc;                     Lat;      Lng;     Alt;    HSpd; VSpd; Head;   Sat; IntTemp; Temp1; Temp2; Pressure; PAlt; Vin; Duty
            // 08.06.2013 11:50:00.278; 46.96256; 7.37088; 482.50; 0.08; 0.30; 359.68; 8;   33;      17;    0;     956;      485;  988; 8

            if ((parts != null) && (parts.Length >= 15))
            {
                TelemetryData data = new TelemetryData();
                data.UtcTimestamp = DateTime.ParseExact(parts[0], "dd.MM.yyyy HH:mm:ss.fff", null);
                data.Latitude = Single.Parse(parts[1]);
                data.Longitude = Single.Parse(parts[2]);
                data.GpsAltitude = Single.Parse(parts[3]);
                data.HorizontalSpeed = Single.Parse(parts[4]);
                data.VerticalSpeed = Single.Parse(parts[5]);
                data.Heading = Convert.ToInt16(Single.Parse(parts[6]));
                data.Satellites = Byte.Parse(parts[7]);
                data.IntTemperature = Int16.Parse(parts[8]);
                data.Temperature1Raw = UInt16.Parse(parts[9]);
                data.Temperature2Raw = UInt16.Parse(parts[10]);
                data.Pressure = Single.Parse(parts[11])*0.001f;
                data.PressureAltitude = Single.Parse(parts[12]);
                data.VinRaw = UInt16.Parse(parts[13]);
                data.DutyCycle = Byte.Parse(parts[14]);

                data.Vin = data.VinRaw * TelemetryDecoder.BatteryGain;
                data.Temperature1 = TelemetryDecoder.Temp1Offset - data.Temperature1Raw * TelemetryDecoder.Temp1Gain;
                data.Temperature2 = TelemetryDecoder.Temp2Offset - data.Temperature2Raw * TelemetryDecoder.Temp2Gain;

                dataCache.AddTelemetry(data);
            }
        }
    }
}
