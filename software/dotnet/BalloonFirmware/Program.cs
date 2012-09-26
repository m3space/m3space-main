using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace BalloonFirmware
{
    public class Program
    {
        public static void Main()
        {
            Debug.EnableGCMessages(false);  // set true for garbage collector output
#if DEBUG
            Debug.Print("Starting up FEZ Panda II.");
            Debug.Print("OEM String: " + SystemInfo.OEMString);
            Debug.Print("Version: " + SystemInfo.Version.ToString());
#endif
            SpaceBalloon balloon = new SpaceBalloon();

            balloon.Initialize();

            while (true)
            {
                // check threads and ports periodically
                Thread.Sleep(10000);
            }
        }

    }
}
