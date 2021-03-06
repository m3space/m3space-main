﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroundControl.Core.WebAccess;
using GroundControl.Core;
using System.IO;

namespace TestWebUpload
{
    class Program
    {
        const float maxstep = 0.003f;

        static void Main(string[] args)
        {
            Random rnd = new Random();

            LiveTrackerWebClient webaccess = new LiveTrackerWebClient();
            webaccess.Url = "http://localhost/live";

            TelemetryData telemetry = new TelemetryData();
            telemetry.UtcTimestamp = DateTime.UtcNow;
            telemetry.Latitude = 47.50000f + ((float)rnd.NextDouble() - 0.5f) * maxstep;
            telemetry.Longitude = 7.50000f + ((float)rnd.NextDouble() - 0.5f) * maxstep;
            telemetry.GpsAltitude = 300.0f;
            telemetry.PressureAltitude = 280.0f;
            telemetry.Heading = 360;
            telemetry.HorizontalSpeed = 0.25f;
            telemetry.VerticalSpeed = 0.0f;
            telemetry.Satellites = 4;
            telemetry.IntTemperature = 18;
            telemetry.Temperature1 = 23.5f;
            telemetry.Temperature2 = 16.2f;
            telemetry.Pressure = 1.001f;
            telemetry.Vin = 8.25f;
            telemetry.GammaCount = 123;

            webaccess.UploadTelemetry(telemetry);

            byte[] img = File.ReadAllBytes("test.jpg");

            webaccess.UploadLiveImage(DateTime.UtcNow, img);

            webaccess.PostBlog(DateTime.Now, "Test message");

            Console.Write("Press [Enter]...");
            Console.ReadLine();
        }
    }
}
