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

        public const string TelemetryFormatV5 = "UtcDate;Lat;Long;GpsAlt;PressureAlt;Heading;HSpeed;VSpeed;Sat;IntTemp;Temp1;Temp2;Press;Vin;Temp1Raw;Temp2Raw;VinRaw;DutyCycle;Gamma;CPM";
        public const string TelemetryFormatV3 = "UtcDate;Lat;Long;GpsAlt;PressureAlt;Heading;HSpeed;VSpeed;Sat;IntTemp;Temp1;Temp2;Press;Vin;Temp1Raw;Temp2Raw;VinRaw;DutyCycle";
        public const string TelemetryFormatV2 = "UtcDate;Lat;Long;GpsAlt;PressureAlt;Heading;Speed;Sat;IntTemp;ExtTemp;Press;Vin;IntTempRaw;ExtTempRaw;PressRaw;VinRaw;DutyCycle";
        public const string TelemetryFormatV1 = "Utc;Lat;Lng;Alt;Spd;Head;Sat;IntTemp;ExtTemp;Pressure;Vin;Duty";
        public const string TelemetryFormatCapsuleV5 = "Utc;Lat;Lng;Alt;HSpd;VSpd;Head;Sat;IntTemp;Temp1;Temp2;Pressure;PAlt;Vin;Duty;Gamma";
        public const string TelemetryFormatCapsuleV3 = "Utc;Lat;Lng;Alt;HSpd;VSpd;Head;Sat;IntTemp;Temp1;Temp2;Pressure;PAlt;Vin;Duty";
        

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

            return String.Format("[Telemetry] {0:dd.MM.yyyy HH:mm:ss} Loc:{1}°{2:0.###}'{3} {4}°{5:0.###}'{6} {7:0.#}m Head:{8}° Vh:{9:0.#}m/s Vv:{10:0.#}m/s Sat:{11} TInt:{12}°C T1:{13:0.#}°C T2:{14:0.#}°C Baro:{15:0.###}bar {16:0.#}m Gamma:{17} Vin:{18:0.#}V Duty:{19}%",
                data.UtcTimestamp.ToLocalTime(),
                latDegs,
                latDecMins,
                latOri,
                lngDegs,
                lngDecMins,
                lngOri,
                data.GpsAltitude,                
                data.Heading,
                data.HorizontalSpeed,
                data.VerticalSpeed,
                data.Satellites,
                data.IntTemperature,
                data.Temperature1,
                data.Temperature2,
                data.Pressure,
                data.PressureAltitude,
                data.GammaCount,
                data.Vin,
                data.DutyCycle
                );
        }
    }
}
