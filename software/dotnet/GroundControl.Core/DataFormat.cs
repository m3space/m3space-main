using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundControl.Core
{
    /// <summary>
    /// Data formatting.
    /// </summary>
    public class DataFormat
    {
        public const float Rad2Deg = 180.0f / (float)Math.PI;
        public const float Deg2Rad = (float)Math.PI / 180.0f;

        public const string TelemetryFormatCurrent = "UtcDate;Lat;Long;GpsAlt;PressureAlt;Heading;HSpeed;VSpeed;Sat;IntTemp;Temp1;Temp2;Press;Vin;Temp1Raw;Temp2Raw;VinRaw;DutyCycle";
        public const string TelemetryFormatV2 = "UtcDate;Lat;Long;GpsAlt;PressureAlt;Heading;Speed;Sat;IntTemp;ExtTemp;Press;Vin;IntTempRaw;ExtTempRaw;PressRaw;VinRaw;DutyCycle";

        /// <summary>
        /// Displays telemetry as string.
        /// </summary>
        /// <param name="data">the telemetry data</param>
        /// <returns>a string</returns>
        public static string TelemetryDisplayString(TelemetryData data)
        {
            // convert GPS position to decimal minutes
            float latAbs = Math.Abs(data.Latitude);
            int latDegs = (int)latAbs;
            float latDecMins = (latAbs - latDegs) * 60;
            char latOri = (data.Latitude >= 0.0f) ? 'N' : 'S';

            float lngAbs = Math.Abs(data.Longitude);
            int lngDegs = (int)lngAbs;
            float lngDecMins = (lngAbs - lngDegs) * 60;
            char lngOri = (data.Latitude >= 0.0f) ? 'E' : 'W';

            return String.Format("[Telemetry] {0:dd.MM.yyyy HH:mm:ss} Loc:{1}°{2:0.###}'{3} {4}°{5:0.###}'{6} Alt:{7:0.#}m PAlt:{8:0.#}m Head:{9}° HSpd:{10:0.#}m/s VSpd:{11:0.#}m/s Sat:{12} TInt:{13}°C T1:{14:0.#}°C T2:{15:0.#}°C P:{16:0.####}bar Vin:{17:0.##}V Duty:{18}%",
                data.UtcTimestamp.ToLocalTime(),
                latDegs,
                latDecMins,
                latOri,
                lngDegs,
                lngDecMins,
                lngOri,
                data.GpsAltitude,
                data.PressureAltitude,
                data.Heading,
                data.HorizontalSpeed,
                data.VerticalSpeed,
                data.Satellites,
                data.IntTemperature,
                data.Temperature1,
                data.Temperature2,
                data.Pressure,
                data.Vin,
                data.DutyCycle);
        }
    }
}
