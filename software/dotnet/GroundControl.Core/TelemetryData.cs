using System;

namespace GroundControl.Core
{
    /// <summary>
    /// A telemetry data object.
    /// </summary>
    public class TelemetryData
    {
        /// <summary>
        /// The UTC timestamp.
        /// </summary>
        public DateTime UtcTimestamp { get; set; }

        /// <summary>
        /// GPS latitude in decimal degrees.
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// GPS longitude in decimal degrees.
        /// </summary>
        public float Longitude { get; set; }

        /// <summary>
        /// Altitude measured by GPS in meters above sea level.
        /// </summary>
        public float GpsAltitude { get; set; }

        /// <summary>
        /// The heading in radians.
        /// </summary>
        public float Heading { get; set; }

        /// <summary>
        /// The speed over ground in m/s.
        /// </summary>
        public float HorizontalSpeed { get; set; }

        /// <summary>
        /// The estimated vertical speed in m/s.
        /// </summary>
        public float VerticalSpeed { get; set; }

        /// <summary>
        /// The number of visible satellites.
        /// </summary>
        public byte Satellites { get; set; }

        /// <summary>
        /// The internal temperature in celsius.
        /// </summary>
        public float IntTemperature { get; set; }

        /// <summary>
        /// The external temperature in celsius.
        /// </summary>
        public float ExtTemperature { get; set; }

        /// <summary>
        /// The absolute pressure in bar.
        /// </summary>
        public float Pressure { get; set; }

        /// <summary>
        /// Altitude measured by pressure in meters above sea level.
        /// </summary>
        public float PressureAltitude { get; set; }

        /// <summary>
        /// Main battery voltage in volts.
        /// </summary>
        public float Vin { get; set; }

        /// <summary>
        /// Raw internal temperature (10-bit ADC)
        /// </summary>
        public ushort IntTemperatureRaw { get; set; }

        /// <summary>
        /// Raw external temperature (10-bit ADC)
        /// </summary>
        public ushort ExtTemperatureRaw { get; set; }

        /// <summary>
        /// Raw battery voltage (10-bit ADC)
        /// </summary>
        public ushort VinRaw { get; set; }

        /// <summary>
        /// XBee duty cycle in %.
        /// </summary>
        public byte DutyCycle { get; set; }
    }
}