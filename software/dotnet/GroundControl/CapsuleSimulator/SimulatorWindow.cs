using System;
using System.IO.Ports;
using System.Windows.Forms;
using CapsuleSimulator.Properties;
using GroundControl.Core;

namespace CapsuleSimulator
{
    public partial class SimulatorWindow : Form
    {
        private byte[]      m_txBuffer;
        private SerialPort  m_serialPort;
        private DataCache   m_datacache;
        private int         m_index;

        public SimulatorWindow()
        {
            InitializeComponent();

            Size = Settings.Default.Size;
            tboxTelemetryFile.Text = Settings.Default.DataDirectory;
            tboxComPort.Text = Settings.Default.ComPort;
            numTelemetryInterval.Value = Settings.Default.TelemetryInterval;

            m_index = 0;
            m_txBuffer = new byte[256];
            m_datacache = new DataCache();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!DataLoader.LoadTelemetryData(tboxTelemetryFile.Text, m_datacache))
            {
                MessageBox.Show("Failed to load data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            btnStart.Enabled = false;
            m_serialPort = new SerialPort(tboxComPort.Text, 38400, Parity.None, 8, StopBits.One);
            m_serialPort.WriteTimeout = 100;
            m_serialPort.Open();
            m_timer.Enabled = true;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            m_timer.Enabled = false;
            if (m_serialPort != null)
            {
                m_serialPort.Close();
            }
            btnStart.Enabled = true;
        }

        private void btnSelectTelemetryFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tboxTelemetryFile.Text = openFileDialog1.FileName;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (m_index < m_datacache.Size)
            {
                try
                {
                    TelemetryData data = m_datacache.Telemetry[m_index];

                    // HOT FIXES
                    // always use today's date to display in live tracker
                    DateTime todayFix = DateTime.Now.Date.Add(data.UtcTimestamp.TimeOfDay);
                    data.UtcTimestamp = todayFix;
                    // simulate gamma count
                    data.GammaCount = m_index;

                    int len = DataProtocol.PreparePacket(m_txBuffer, DataProtocol.GetTelemetry(data));
                    if (m_serialPort.IsOpen)
                    {
                        m_index++;
                        tboxLogger.AppendText(DataFormat.TelemetryDisplayString(data) + Environment.NewLine);
                        m_serialPort.Write(m_txBuffer, 0, len);
                    }
                }
                catch (Exception ex)
                {
                    tboxLogger.AppendText(ex.ToString() + Environment.NewLine);
                }
            }
            else
            {
                m_index = 0;
                m_timer.Enabled = false;
                btnStart.Text = "Start";
            }
        }

        private void numTelemetryInterval_ValueChanged(object sender, EventArgs e)
        {
            m_timer.Interval = (int)numTelemetryInterval.Value * 1000;
        }

        private void SimulatorWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.ComPort = tboxComPort.Text;
            Settings.Default.DataDirectory = tboxTelemetryFile.Text;
            Settings.Default.TelemetryInterval = (int)numTelemetryInterval.Value;
            Settings.Default.Size = Size;
            Settings.Default.Save();
        }
    }
}
