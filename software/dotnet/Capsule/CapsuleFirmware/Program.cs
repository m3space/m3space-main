using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using M3Space.Capsule.Drivers;

namespace M3Space.Capsule
{
    public class Program
    {
        public static void Main()
        {
            Debug.EnableGCMessages(false);  // set true for garbage collector output
#if DEBUG
            Debug.Print("Starting up FEZ Panda II");
            Debug.Print("OEM String: " + SystemInfo.OEMString);
            Debug.Print("Version: " + SystemInfo.Version.ToString());
#endif
            M3SpaceCapsule capsule = new M3SpaceCapsule();

            if (capsule.Initialize())
            {
                OnboardLed.Off();
                while (true)
                {
                    Thread.Sleep(15000);
                    capsule.CheckThreads();
                }
            }
            else
            {
                OnboardLed.Blink(100);
            }
        }

    }
}
