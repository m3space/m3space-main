﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GroundControl.Gui.Properties;

namespace GroundControl.Gui
{
    public partial class PreferencesDialog : Form
    {
        public PreferencesDialog()
        {
            InitializeComponent();
        }

        public string DataDirectory { get { return dataDirBox.Text; } }

        public string ComPort { get { return comPortBox.Text; } }

        public string WebAccessUrl { get { return webAccessBox.Text; } }

        public bool WebAccessEnabled { get { return webAccessCheck.Checked; } }

        private void PreferencesDialog_Load(object sender, EventArgs e)
        {
            dataDirBox.Text = Settings.Default.DataDirectory;
            comPortBox.Text = Settings.Default.ComPort;
            webAccessBox.Text = Settings.Default.WebAccessUrl;
            webAccessCheck.Checked = Settings.Default.WebAccessEnabled;
        }

        private void dataDirBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                dataDirBox.Text = dialog.SelectedPath;
            }

        }

    }
}
