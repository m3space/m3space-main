using System;
using Microsoft.SPOT;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace CameraController
{
    public class CameraController
    {
        public static void Main()
        {
            Debug.EnableGCMessages(false); 

            using (var camera = new LinkspriteCamera(new SerialPort("COM1", 38400, Parity.None, 8, StopBits.One)))
            {
                //DoSuccessFail(camera.Reset, "Reset successfull", "ERROR resetting");
                //DoSuccessFail(() => camera.SetPictureSize(LinkspriteCamera.SET_SIZE_640x480), "Size change successfull", "ERROR changing size");
                //DoSuccessFail(camera.Reset, "Reset successfull", "ERROR resetting");

                if(camera.Reset())
                    camera.GetPicture(ProcessChunk);

                DoSuccessFail(camera.Stop, "Stop successfull", "ERROR stopping");
            }
        }

        private static void ProcessChunk(byte[] bytes)
        {
            foreach (var byter in bytes)
                Debug.Print(byter.ToString());
        }

        private delegate bool FuncBool();

        private static void DoSuccessFail(FuncBool action, string success, string fail)
        {
            if (action())
                Debug.Print(success);
            else
                Debug.Print(fail);
        }
    }
}
    
//This just returns the bytes over the debug cable, you can do something with the bytes if you 
//have an SD card or are going to transmit, otherwise just copy the bytes to the clippboard 
//from the output window and use the below snippet to create the jpg file and look at it on your computer:

            var byteString = @"<put bytes here>";
            var lines = byteString.Split(Environment.NewLine.ToCharArray());
            var bytes = new List<byte>();
            foreach (var line in lines.Where(x => (x ?? "").Trim() != ""))
            {
                byte num = Convert.ToByte(line);
                bytes.Add(num);
            }

            string file = @"C:\temp.jpg";
            File.WriteAllBytes(file, bytes.ToArray());
            System.Diagnostics.Process.Start(file);


