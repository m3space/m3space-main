﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GraphLib;
using GroundControl.Core;

namespace GroundControl.Gui
{
    public partial class GraphWindow : Form
    {
        public GraphWindow()
        {
            InitializeComponent();
            m_display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_VER;

            addNewDataSource("Alt.GPS[m]", Color.Red, true, 0, 35000);
            addNewDataSource("Alt.P[m]", Color.Blue, false, 0, 35000);
            addNewDataSource("Bat.[V]", Color.Green, false, 6, 9);
            addNewDataSource("Tin[°C]", Color.DarkBlue, true, -60, 50);
            addNewDataSource("T1[°C]", Color.DarkCyan, true, -60, 35);
            addNewDataSource("T2[°C]", Color.LightCyan, true, -60, 35);
            addNewDataSource("P[bar]", Color.DarkGreen, true, 0, 1);
            addNewDataSource("hor.V[m/s]", Color.DarkMagenta, true, 0, 100);
            addNewDataSource("ver.V[m/s]", Color.Magenta, true, 0, 100);
            addNewDataSource("Dir[°]", Color.DarkOliveGreen, false, 0, 360);
            addNewDataSource("Sat[#]", Color.Orange, false, 0, 15);
            addNewDataSource("DutyCycle[%]", Color.Brown, false, 0, 100);
            addNewDataSource("Gamma[c]", Color.DarkKhaki, true, 0, 65000);
        }

        private void addNewDataSource( String name, Color color, bool visible, float minValue, float maxValue)
        {
            m_display.DataSources.Add(new DataSource());
            m_display.DataSources.Last().Name = name;
            m_display.DataSources.Last().GraphColor = color;
            m_display.DataSources.Last().AutoScaleY = true;
            m_display.DataSources.Last().SetDisplayRangeY(minValue, maxValue);
            m_display.DataSources.Last().SetGridDistanceY(Math.Max(1, (int)((maxValue - minValue) * 0.2)));
            m_display.DataSources.Last().OnRenderXAxisLabel += RenderXLabel;
            m_display.DataSources.Last().Active = visible;
        }

        public void AddTelemetryToGraph(TelemetryData telemetry)
        {
            m_display.DataSources[0].Samples.Add(new cPoint(m_display.DataSources[0].Samples.Count, telemetry.UtcTimestamp, telemetry.GpsAltitude));
            m_display.DataSources[1].Samples.Add(new cPoint(m_display.DataSources[1].Samples.Count, telemetry.UtcTimestamp, telemetry.PressureAltitude));
            m_display.DataSources[2].Samples.Add(new cPoint(m_display.DataSources[2].Samples.Count, telemetry.UtcTimestamp, telemetry.Vin));
            m_display.DataSources[3].Samples.Add(new cPoint(m_display.DataSources[3].Samples.Count, telemetry.UtcTimestamp, telemetry.IntTemperature));
            m_display.DataSources[4].Samples.Add(new cPoint(m_display.DataSources[4].Samples.Count, telemetry.UtcTimestamp, telemetry.Temperature1));
            m_display.DataSources[5].Samples.Add(new cPoint(m_display.DataSources[5].Samples.Count, telemetry.UtcTimestamp, telemetry.Temperature2));
            m_display.DataSources[6].Samples.Add(new cPoint(m_display.DataSources[6].Samples.Count, telemetry.UtcTimestamp, telemetry.Pressure));
            m_display.DataSources[7].Samples.Add(new cPoint(m_display.DataSources[7].Samples.Count, telemetry.UtcTimestamp, telemetry.HorizontalSpeed));
            m_display.DataSources[8].Samples.Add(new cPoint(m_display.DataSources[8].Samples.Count, telemetry.UtcTimestamp, telemetry.VerticalSpeed));
            m_display.DataSources[9].Samples.Add(new cPoint(m_display.DataSources[9].Samples.Count, telemetry.UtcTimestamp, telemetry.Heading));
            m_display.DataSources[10].Samples.Add(new cPoint(m_display.DataSources[10].Samples.Count, telemetry.UtcTimestamp, telemetry.Satellites));
            m_display.DataSources[11].Samples.Add(new cPoint(m_display.DataSources[11].Samples.Count, telemetry.UtcTimestamp, telemetry.DutyCycle));
            m_display.DataSources[12].Samples.Add(new cPoint(m_display.DataSources[12].Samples.Count, telemetry.UtcTimestamp, telemetry.GammaCount));
        }

        private String RenderXLabel(DataSource s, int idx)
        {
            return s.Samples[idx].Timestamp.ToLocalTime().ToShortTimeString();
        }

        public void AddTelemetry(TelemetryData telemetry)
        {
            AddTelemetryToGraph(telemetry);
            m_display.Refresh();
        }

        public void Clear()
        {
            foreach (var item in m_display.DataSources)
            {
                item.Samples.Clear();
            }
            m_display.Refresh();
        }

        public void LoadFromCache(DataCache dataCache)
        {
            foreach (var item in dataCache.Telemetry)
            {
                AddTelemetry(item);
            }
            m_display.Refresh();
        }
    }
}
