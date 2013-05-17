using System.Threading;
using GHIElectronics.NETMF.FEZ;
using Microsoft.SPOT.Hardware;

namespace M3Space.Capsule.Drivers
{
    abstract class OnboardLed
    {
        private static OutputPort LED = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, false);
        private static Timer blinkTimer = new Timer(new TimerCallback(blinkTimer_Tick), null, Timeout.Infinite, 0);

        public static void Off()
        {
            StopTimer();
            LED.Write(false);
        }

        public static void On()
        {
            StopTimer();
            LED.Write(true);
        }

        public static void Toggle()
        {
            StopTimer();
            LED.Write(!LED.Read());
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval">blink interval in ms</param>
        public static void Blink(int interval)
        {
            blinkTimer.Change(0, interval);
        }

        private static void StopTimer()
        {
            blinkTimer.Change(Timeout.Infinite, 0);
        }

        private static void blinkTimer_Tick(object o)
        {
            LED.Write(!LED.Read());
        }
    }
}
