using System;
using System.Collections.Generic;
using System.Text;
using M3Space.MotionAnalysis.DataModel;

namespace M3Space.MotionAnalysis
{
    /// <summary>
    /// Motion data handling and processing.
    /// </summary>
    public class MotionDataManager
    {
        const int GravityWindowSize = 10;

        const long TicksPerMs = TimeSpan.TicksPerMillisecond;

        /// <summary>
        /// Loads raw motion data from a CSV file.
        /// </summary>
        /// <param name="path">the path to the file</param>
        /// <returns>a list of raw data records, or null if an error occurs</returns>
        public static List<RawMotionRecord> LoadFromFile(string path)
        {
            
            List<RawMotionRecord> rawData = new List<RawMotionRecord>();
            rawData.Sort(CompareRecordsByTimestamp);
            return rawData;
        }

        /// <summary>
        /// Computes motion data from raw data.
        /// Converts the raw measurement values to g and degrees/sec, and also computes gravity.
        /// The data is normalized to a constant time interval.
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

            if (rawData.Count == 0)
            {
                return dataSet;
            }

            // window for running average
            MotionRecord[] gravityWindow = new MotionRecord[GravityWindowSize];
            float gravityAccu;

            // timestamp of first record -> relative time
            long refTicks = rawData[0].UtcTimestamp.Ticks;
            long totalTicks = rawData[rawData.Count-1].UtcTimestamp.Ticks - refTicks;

            long recTicks = 0;


            return dataSet;
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
