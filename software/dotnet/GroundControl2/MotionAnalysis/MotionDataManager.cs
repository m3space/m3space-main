using System;
using System.Collections.Generic;
using System.Text;
using M3Space.MotionAnalysis.DataModel;
using System.IO;
using System.Globalization;

namespace M3Space.MotionAnalysis
{
    /// <summary>
    /// Motion data handling and processing.
    /// </summary>
    public class MotionDataManager
    {
        const int GravityWindowSize = 10;

        const long TicksPerMs = TimeSpan.TicksPerMillisecond;

        const string RawMotionDataFormat = "Utc;Ax;Ay;Az;Gx;Gy;Gz";
        const string MotionDataFormat = "Utc;Ax;Ay;Az;Gx;Gy;Gz;Rx;Ry;Rz";
        const string MotionDataDateFormat = "dd.MM.yyyy HH:mm:ss.fff";

        /// <summary>
        /// Imports raw motion data from a CSV file.
        /// </summary>
        /// <param name="path">the path to the file</param>
        /// <returns>a list of raw data records, or null if an error occurs</returns>
        public static List<RawMotionRecord> ImportRawMotionData(string path)
        {
            List<RawMotionRecord> rawData = null;
            StreamReader reader = new StreamReader(path);
            if (!reader.EndOfStream)
            {
                string firstLine = reader.ReadLine();

                if (firstLine.Equals(RawMotionDataFormat))
                {
                    rawData = new List<RawMotionRecord>();

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] parts = line.Split(';');

                        RawMotionRecord record = new RawMotionRecord();
                        try
                        {
                            record.UtcTimestamp = DateTime.ParseExact(parts[0], MotionDataDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
                            record.Ax = Int32.Parse(parts[1]);
                            record.Ay = Int32.Parse(parts[2]);
                            record.Az = Int32.Parse(parts[3]);
                            record.Rx = Int32.Parse(parts[4]);
                            record.Ry = Int32.Parse(parts[5]);
                            record.Rz = Int32.Parse(parts[6]);
                        }
                        catch (Exception)
                        {
                            break;
                        }

                        rawData.Add(record);
                    }

                    // make sure timestamps are ordered
                    rawData.Sort(CompareRecordsByTimestamp);
                }
            }

            reader.Close();
            return rawData;
        }

        /// <summary>
        /// Applies an offset to a batch of raw motion data.
        /// Offsets are added to the data, and therefore should be the negative of the actual deviation.
        /// </summary>
        /// <param name="rawData">the raw motion data</param>
        /// <param name="offset">the accleration/gyro offsets to apply</param>
        public static void ApplyOffsets(List<RawMotionRecord> rawData, RawMotionRecord offset)
        {
            foreach (RawMotionRecord record in rawData)
            {
                record.Ax += offset.Ax;
                record.Ay += offset.Ay;
                record.Az += offset.Az;
                record.Rx += offset.Rx;
                record.Ry += offset.Ry;
                record.Rz += offset.Rz;
            }
        }

        /// <summary>
        /// Computes motion data from raw data.
        /// Converts the raw measurement values to g and degrees/sec, and also computes gravity.
        /// The data is interpolated to a constant time interval.
        /// </summary>
        /// <param name="rawData">the raw motion data</param>
        /// <param name="interval">the desired time interval (ms)</param>
        /// <param name="accelRange">the acceleration sensor range (g)</param>
        /// <param name="gyroRange">the gyro range (degrees/sec)</param>
        /// <returns>a motion data set</returns>
        public static MotionDataSet ProcessMotionData(List<RawMotionRecord> rawData, long interval, int accelRange, int gyroRange)
        {
            MotionDataSet dataSet = new MotionDataSet();
            dataSet.Interval = interval;
            float accelFactor = (float)accelRange / 32768;
            float gyroFactor = (float)gyroRange / 32768;

            if (rawData.Count == 0)
            {
                return dataSet;
            }
            dataSet.UtcStart = rawData[0].UtcTimestamp;

            // window for running average
            Queue<MotionRecord> gravityWindow = new Queue<MotionRecord>();
            float[] accelAccu = new float[3];
            MotionRecord firstRecord = new MotionRecord();
            firstRecord.Ax = rawData[0].Ax * accelFactor;
            firstRecord.Ay = rawData[0].Ay * accelFactor;
            firstRecord.Az = rawData[0].Az * accelFactor;
            firstRecord.Rx = rawData[0].Rx * gyroFactor;
            firstRecord.Ry = rawData[0].Ry * gyroFactor;
            firstRecord.Rz = rawData[0].Rz * gyroFactor;
            for (int i = 0; i < GravityWindowSize; i++)
            {
                gravityWindow.Enqueue(firstRecord);
                accelAccu[0] += firstRecord.Ax;
                accelAccu[1] += firstRecord.Ay;
                accelAccu[2] += firstRecord.Az;
            }

            // timestamp of first record -> relative time
            long refTicks = rawData[0].UtcTimestamp.Ticks;
            long totalTicks = rawData[rawData.Count-1].UtcTimestamp.Ticks - refTicks;

            long recTicks = 0;
            int prevIndex = 0;
            int nextIndex = 0;

            while (recTicks <= totalTicks)
            {
                MotionRecord newRecord = new MotionRecord();

                nextIndex = prevIndex;

                // get nearest previous neighbor
                while ((rawData[prevIndex].UtcTimestamp.Ticks - refTicks) < recTicks)
                {
                    prevIndex++;
                }
                if ((rawData[prevIndex].UtcTimestamp.Ticks - refTicks) > recTicks)
                {
                    prevIndex--;
                }
                
                // get nearest following neighbor
                while ((rawData[nextIndex].UtcTimestamp.Ticks - refTicks) < recTicks)
                {
                    nextIndex++;
                    if (nextIndex >= rawData.Count)
                    {
                        // out of range
                        return null;
                    }
                }

                // compute motion data
                if (recTicks == rawData[prevIndex].UtcTimestamp.Ticks - refTicks)
                {
                    newRecord.Ax = rawData[prevIndex].Ax * accelFactor;
                    newRecord.Ay = rawData[prevIndex].Ay * accelFactor;
                    newRecord.Az = rawData[prevIndex].Az * accelFactor;
                    newRecord.Rx = rawData[prevIndex].Rx * gyroFactor;
                    newRecord.Ry = rawData[prevIndex].Ry * gyroFactor;
                    newRecord.Rz = rawData[prevIndex].Rz * gyroFactor;
                }
                else if (recTicks == rawData[nextIndex].UtcTimestamp.Ticks - refTicks)
                {
                    newRecord.Ax = rawData[nextIndex].Ax * accelFactor;
                    newRecord.Ay = rawData[nextIndex].Ay * accelFactor;
                    newRecord.Az = rawData[nextIndex].Az * accelFactor;
                    newRecord.Rx = rawData[nextIndex].Rx * gyroFactor;
                    newRecord.Ry = rawData[nextIndex].Ry * gyroFactor;
                    newRecord.Rz = rawData[nextIndex].Rz * gyroFactor;
                }
                else {
                    // interpolate
                    long dt = rawData[nextIndex].UtcTimestamp.Ticks - rawData[prevIndex].UtcTimestamp.Ticks;
                    long t = refTicks + recTicks - rawData[prevIndex].UtcTimestamp.Ticks;
                    float prevRatio = (float)t / dt;
                    float nextRatio = 1.0f - prevRatio;
                    newRecord.Ax = (rawData[prevIndex].Ax * prevRatio + rawData[nextIndex].Ax * nextRatio) * accelFactor;
                    newRecord.Ay = (rawData[prevIndex].Ay * prevRatio + rawData[nextIndex].Ay * nextRatio) * accelFactor;
                    newRecord.Az = (rawData[prevIndex].Az * prevRatio + rawData[nextIndex].Az * nextRatio) * accelFactor;
                    newRecord.Rx = (rawData[prevIndex].Rx * prevRatio + rawData[nextIndex].Rx * nextRatio) * gyroFactor;
                    newRecord.Ry = (rawData[prevIndex].Ry * prevRatio + rawData[nextIndex].Ry * nextRatio) * gyroFactor;
                    newRecord.Rz = (rawData[prevIndex].Rz * prevRatio + rawData[nextIndex].Rz * nextRatio) * gyroFactor;
                }
                
                // compute gravity
                gravityWindow.Enqueue(newRecord);
                accelAccu[0] += newRecord.Ax;
                accelAccu[1] += newRecord.Ay;
                accelAccu[2] += newRecord.Az;
                if (gravityWindow.Count > GravityWindowSize)
                {
                    MotionRecord removed = gravityWindow.Dequeue();
                    accelAccu[0] -= removed.Ax;
                    accelAccu[1] -= removed.Ay;
                    accelAccu[2] -= removed.Az;
                }
                newRecord.Gx = accelAccu[0] / GravityWindowSize;
                newRecord.Gy = accelAccu[1] / GravityWindowSize;
                newRecord.Gz = accelAccu[2] / GravityWindowSize;

                dataSet.Records.Add(newRecord);
                recTicks += interval * TicksPerMs;
            }

            return dataSet;
        }

        /// <summary>
        /// Saves motion data to a file.
        /// </summary>
        /// <param name="data">the motion data</param>
        /// <param name="outputFilename">the output file name</param>
        public static void Save(MotionDataSet data, string outputFilename)
        {
            StreamWriter writer = File.CreateText(outputFilename);
            writer.WriteLine(MotionDataFormat);
            for (int i = 0; i < data.Records.Count; i++)
            {
                writer.WriteLine(String.Format("{0:dd.MM.yyyy HH:mm:ss.fff};{1};{2};{3};{4};{5};{6};{7};{8};{9}",
                    data.UtcStart.AddMilliseconds(i * data.Interval),
                    data.Records[i].Ax,
                    data.Records[i].Ay,
                    data.Records[i].Az,
                    data.Records[i].Gx,
                    data.Records[i].Gy,
                    data.Records[i].Gz,
                    data.Records[i].Rx,
                    data.Records[i].Ry,
                    data.Records[i].Rz));
            }
            writer.Close();
        }

        /// <summary>
        /// Comparer used to sort raw motion data.
        /// </summary>
        /// <param name="a">first record</param>
        /// <param name="b">second record</param>
        /// <returns>1 if a greater than b, -1 if a less than b, 0 if equal</returns>
        private static int CompareRecordsByTimestamp(RawMotionRecord a, RawMotionRecord b)
        {
            return a.UtcTimestamp.CompareTo(b.UtcTimestamp);
        }
    }
}
