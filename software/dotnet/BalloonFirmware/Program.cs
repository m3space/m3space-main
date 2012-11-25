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

            if (balloon.Initialize())
            {
                Drivers.OnboardLed.Off();
                while (true)
                {
                    Thread.Sleep(10000);
                    balloon.CheckThreads();
                }
            }
            else
            {
                Drivers.OnboardLed.Blink(100);
            }
        }
    }
}
