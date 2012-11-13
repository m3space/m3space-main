using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroundControl.Core;
using System.IO;
using System.Threading;

namespace GroundControl.Console
{
    /// <summary>
    /// Ground Control Console Application.
    /// Receives data from serial receiver, displays and saves telemetry and saves camera images.
    /// </summary>
    class Program
    {
        const float Rad2Deg = 180.0f / (float)Math.PI;

        static string telemetryFileName = "telemetry.csv";

        static AutoResetEvent waitHandle = new AutoResetEvent(false);
        static DataTransceiver transceiver;

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">arguments</param>
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("Usage: GroundControl.Console <COMPort>");
                return;
            }

            System.Console.CancelKeyPress += OnConsoleCancel;

            PersistenceHandler persistHandler = new PersistenceHandler();
            persistHandler.CreateTelemetryFile(DateTime.UtcNow);

            transceiver = new DataTransceiver(args[0]);
            DataProtocol protocol = new DataProtocol();
            protocol.Error += DisplayError;
            protocol.TelemetryReceived += DisplayTelemetry;
            protocol.TelemetryReceived += persistHandler.SaveTelemetry;
            protocol.ImageComplete += NotifyImageComplete;
            protocol.ImageComplete += persistHandler.SaveImage;
            transceiver.FrameReceived += protocol.DecodeFrame;
            transceiver.Error += DisplayError;

            System.Console.WriteLine("Data directory is " + persistHandler.DataDirectory);

            transceiver.Start();
            System.Console.WriteLine(String.Format("Transceiver started on {0}.", args[0]));

            waitHandle.WaitOne();
        }

        /// <summary>
        /// Displays an error message.
        /// </summary>
        /// <param name="message">the error message</param>
        static void DisplayError(string message)
        {
            System.Console.WriteLine(String.Format("[Error] {0}", message));
        }

        /// <summary>
        /// Displays telemetry data.
        /// </summary>
        /// <param name="telemetry">the telemetry data</param>
        static void DisplayTelemetry(TelemetryData telemetry)
        {
            // convert GPS position to decimal minutes
            float latAbs = Math.Abs(telemetry.Latitude);
            int latDegs = (int)latAbs;
            float latDecMins = (latAbs - latDegs) * 60;
            char latOri = (telemetry.Latitude >= 0.0f) ? 'N' : 'S';

            float lngAbs = Math.Abs(telemetry.Longitude);
            int lngDegs = (int)lngAbs;
            float lngDecMins = (lngAbs - lngDegs) * 60;
            char lngOri = (telemetry.Latitude >= 0.0f) ? 'E' : 'W';

            System.Console.WriteLine(String.Format("[Telemetry] {0:dd.MM.yyyy HH:mm:ss} Loc:{1}°{2:0.###}'{3} {4}°{5:0.###}'{6} Alt:{7:0.#}m PAlt:{8:0.#}m Head:{9:0.#}° HSpd:{10:0.#}m/s VSpd:{11:0.#}m/s Sat:{12} TInt:{13}°C T1:{14:0.#}°C T2:{15:0.#}°C P:{16:0.####}bar Vin:{17:0.##}V Duty:{18}%",
                telemetry.UtcTimestamp.ToLocalTime(),
                latDegs,
                latDecMins,
                latOri,
                lngDegs,
                lngDecMins,
                lngOri,
                telemetry.GpsAltitude,
                telemetry.PressureAltitude,
                telemetry.Heading * Rad2Deg,
                telemetry.HorizontalSpeed,
                telemetry.VerticalSpeed,
                telemetry.Satellites,
                telemetry.IntTemperature,
                telemetry.Temperature1,
                telemetry.Temperature2,
                telemetry.Pressure,
                telemetry.Vin,
                telemetry.DutyCycle));
        }

        /// <summary>
        /// Notifies when a camera image arrives.
        /// </summary>
        /// <param name="utc">the image timestamp</param>
        /// <param name="data">the image data</param>
        static void NotifyImageComplete(DateTime utc, byte[] data)
        {
            System.Console.WriteLine(String.Format("[Image] {0:dd.MM.yyyy HH:mm:ss} Size: {1} bytes", utc.ToLocalTime(), data.Length));
        }

        /// <summary>
        /// Saves telemetry data on disk.
        /// Data is appended to the default telemetry file.
        /// </summary>
        /// <param name="telemetry">the telemetry data</param>
        static void SaveTelemetry(TelemetryData telemetry)
        {
            StreamWriter writer = File.AppendText(telemetryFileName);
            writer.WriteLine(String.Format("{0:dd.MM.yyyy HH:mm:ss};{1:0.####};{2:0.####};{3:0.#};{4:0.#};{5:0.#};{6:0.#};{7:0.#};{8};{9};{10:0.#};{11:0.#};{12:0.####};{13:0.##};{14};{15};{16};{17}",
                telemetry.UtcTimestamp,
                telemetry.Latitude,
                telemetry.Longitude,
                telemetry.GpsAltitude,
                telemetry.PressureAltitude,
                telemetry.Heading * Rad2Deg,
                telemetry.HorizontalSpeed,
                telemetry.VerticalSpeed,
                telemetry.Satellites,
                telemetry.IntTemperature,
                telemetry.Temperature1,
                telemetry.Temperature2,
                telemetry.Pressure,
                telemetry.Vin,
                telemetry.Temperature1Raw,
                telemetry.Temperature2Raw,
                telemetry.VinRaw,
                telemetry.DutyCycle));
            writer.Close();
        }

        /// <summary>
        /// Saves an image to disk.
        /// </summary>
        /// <param name="utc">the image timestamp</param>
        /// <param name="data">the image data</param>
        static void SaveImage(DateTime utc, byte[] data)
        {
            BinaryWriter writer = new BinaryWriter(File.Create(utc.ToString("yyyyMMdd_HHmmss") + ".jpg"));
            writer.Write(data);
            writer.Close();
        }

        /// <summary>
        /// Cleans up when application is terminated using Ctrl+C.
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="args">the event arguments</param>
        static void OnConsoleCancel(object sender, ConsoleCancelEventArgs args)
        {
            System.Console.WriteLine("Waiting for transceiver to stop.");
            transceiver.Stop();
            System.Console.WriteLine("Transceiver stopped.");
            waitHandle.Set();
        }
    }
}
