using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M3Space.MotionAnalysis;
using M3Space.MotionAnalysis.DataModel;

namespace MotionDataConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: MotionDataConverter <rawdatafile> <outputfile>");
                return;
            }

            string filename = args[0];
            string outputFilename = args[1];

            RawMotionRecord offset = new RawMotionRecord();
            offset.Ax = -566;
            offset.Ay = -2598;
            offset.Az = 16780 - 16384;
            offset.Rx = -566;
            offset.Ry = -566;
            offset.Rz = -566;
            int gRange = 2;
            int rotRange = 250;

            List<RawMotionRecord> imported = MotionDataManager.ImportRawMotionData(filename);
            if (imported != null)
            {
                Console.WriteLine(String.Format("Imported {0} raw data records.", imported.Count));
                MotionDataManager.ApplyOffsets(imported, offset);
                MotionDataSet dataSet = MotionDataManager.ProcessMotionData(imported, 100, gRange, rotRange);
                if (dataSet != null)
                {
                    Console.WriteLine(String.Format("Converted {0} data records.", dataSet.Records.Count));
                    MotionDataManager.Save(dataSet, outputFilename);
                    Console.WriteLine("Saved");
                }
            }
        }
    }
}
