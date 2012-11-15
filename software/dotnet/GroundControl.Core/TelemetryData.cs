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
        public short Heading { get; set; }

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
        public int IntTemperature { get; set; }

        /// <summary>
        /// The 1st temperature in celsius.
        /// </summary>
        public float Temperature1 { get; set; }

        /// <summary>
        /// The 2nd temperature in celsius.
        /// </summary>
        public float Temperature2 { get; set; }

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
        /// Raw 1st temperature (10-bit ADC)
        /// </summary>
        public ushort Temperature1Raw { get; set; }

        /// <summary>
        /// Raw 2nd temperature (10-bit ADC)
        /// </summary>
        public ushort Temperature2Raw { get; set; }

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