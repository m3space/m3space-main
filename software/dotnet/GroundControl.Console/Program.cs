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
            System.Console.WriteLine(DataFormat.TelemetryDisplayString(telemetry));
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
