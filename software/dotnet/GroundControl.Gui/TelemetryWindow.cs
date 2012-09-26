﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GroundControl.Core;

namespace GroundControl.Gui
{
    /// <summary>
    /// A telemetry display form.
    /// Displays telemetry data in text labels.
    /// </summary>
    public partial class TelemetryWindow : Form
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TelemetryWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Updates the display with new telemetry data.
        /// </summary>
        /// <param name="data">the telemetry data</param>
        public void DisplayTelemetry(TelemetryData data)
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

            dateLbl.Text = String.Format("{0:dd.MM.yyyy HH:mm:ss}", data.UtcTimestamp.ToLocalTime());
            latLbl.Text = String.Format("{0}° {1:0.###}' {2}", latDegs, latDecMins, latOri);
            lngLbl.Text = String.Format("{0}° {1:0.###}' {2}", lngDegs, lngDecMins, lngOri);
            altLbl.Text = String.Format("{0:0.#} m", data.GpsAltitude);
            headLbl.Text = String.Format("{0:0.#}°", data.Heading);
            spdLbl.Text = String.Format("{0:0.#} m/s", data.Speed);
            intTempLbl.Text = String.Format("{0:0.#}°C", data.IntTemperature);
            extTempLbl.Text = String.Format("{0:0.#}°C", data.ExtTemperature);
            pressureLbl.Text = String.Format("{0:0.####} bar", data.Pressure);
            pAltLbl.Text = String.Format("{0:0.#} m", data.PressureAltitude);
            vinLbl.Text = String.Format("{0:0.#} V", data.Vin);
        }

    }
}
