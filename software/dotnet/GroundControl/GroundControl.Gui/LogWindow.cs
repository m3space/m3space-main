using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GroundControl.Gui
{
    public partial class LogWindow : Form
    {
        public LogWindow()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            logTextBox.Clear();
        }

        public void WriteLine(string text)
        {
            logTextBox.AppendText(text);
            logTextBox.AppendText(Environment.NewLine);
        }

        private void clearLogMenuItem_Click(object sender, EventArgs e)
        {
            logTextBox.Clear();
        }
    }
}
