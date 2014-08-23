namespace GroundControl.Gui
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.transceiverStateLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.elapsedTitleLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.elapsedLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.filenameLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPredictedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectGPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.captureMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startCaptureMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopCaptureMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eventLogMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.liveImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.telemetryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.predictorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blogMessageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flightradar24MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportGoogleEarthTourMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transceiverStateLbl,
            this.elapsedTitleLbl,
            this.elapsedLbl,
            this.filenameLbl});
            this.statusBar.Location = new System.Drawing.Point(0, 390);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(734, 22);
            this.statusBar.TabIndex = 0;
            this.statusBar.Text = "statusStrip1";
            // 
            // transceiverStateLbl
            // 
            this.transceiverStateLbl.Name = "transceiverStateLbl";
            this.transceiverStateLbl.Size = new System.Drawing.Size(51, 17);
            this.transceiverStateLbl.Text = "Stopped";
            // 
            // elapsedTitleLbl
            // 
            this.elapsedTitleLbl.Name = "elapsedTitleLbl";
            this.elapsedTitleLbl.Size = new System.Drawing.Size(47, 17);
            this.elapsedTitleLbl.Text = "Elapsed";
            // 
            // elapsedLbl
            // 
            this.elapsedLbl.Name = "elapsedLbl";
            this.elapsedLbl.Size = new System.Drawing.Size(49, 17);
            this.elapsedLbl.Text = "00:00:00";
            // 
            // filenameLbl
            // 
            this.filenameLbl.Name = "filenameLbl";
            this.filenameLbl.Size = new System.Drawing.Size(40, 17);
            this.filenameLbl.Text = "no file";
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.captureMenuItem,
            this.windowMenuItem,
            this.toolsMenuItem,
            this.settingsMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(734, 24);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadDataMenuItem,
            this.loadPredictedMenuItem,
            this.clearDataMenuItem,
            this.connectGPSToolStripMenuItem,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileMenuItem.Text = "&File";
            // 
            // loadDataMenuItem
            // 
            this.loadDataMenuItem.Name = "loadDataMenuItem";
            this.loadDataMenuItem.Size = new System.Drawing.Size(191, 22);
            this.loadDataMenuItem.Text = "&Load captured data";
            this.loadDataMenuItem.Click += new System.EventHandler(this.loadDataMenuItem_Click);
            // 
            // loadPredictedMenuItem
            // 
            this.loadPredictedMenuItem.Name = "loadPredictedMenuItem";
            this.loadPredictedMenuItem.Size = new System.Drawing.Size(191, 22);
            this.loadPredictedMenuItem.Text = "Load &predicted course";
            this.loadPredictedMenuItem.Click += new System.EventHandler(this.loadPredictedMenuItem_Click);
            // 
            // clearDataMenuItem
            // 
            this.clearDataMenuItem.Name = "clearDataMenuItem";
            this.clearDataMenuItem.Size = new System.Drawing.Size(191, 22);
            this.clearDataMenuItem.Text = "&Clear data";
            this.clearDataMenuItem.Click += new System.EventHandler(this.clearDataMenuItem_Click);
            // 
            // connectGPSToolStripMenuItem
            // 
            this.connectGPSToolStripMenuItem.Name = "connectGPSToolStripMenuItem";
            this.connectGPSToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.connectGPSToolStripMenuItem.Text = "Connect GPS";
            this.connectGPSToolStripMenuItem.Click += new System.EventHandler(this.connectGPSToolStripMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(191, 22);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // captureMenuItem
            // 
            this.captureMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startCaptureMenuItem,
            this.stopCaptureMenuItem});
            this.captureMenuItem.Name = "captureMenuItem";
            this.captureMenuItem.Size = new System.Drawing.Size(61, 20);
            this.captureMenuItem.Text = "&Capture";
            // 
            // startCaptureMenuItem
            // 
            this.startCaptureMenuItem.Name = "startCaptureMenuItem";
            this.startCaptureMenuItem.Size = new System.Drawing.Size(152, 22);
            this.startCaptureMenuItem.Text = "&Start capture";
            this.startCaptureMenuItem.Click += new System.EventHandler(this.startCaptureMenuItem_Click);
            // 
            // stopCaptureMenuItem
            // 
            this.stopCaptureMenuItem.Enabled = false;
            this.stopCaptureMenuItem.Name = "stopCaptureMenuItem";
            this.stopCaptureMenuItem.Size = new System.Drawing.Size(152, 22);
            this.stopCaptureMenuItem.Text = "S&top capture";
            this.stopCaptureMenuItem.Click += new System.EventHandler(this.stopCaptureMenuItem_Click);
            // 
            // windowMenuItem
            // 
            this.windowMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.eventLogMenuItem,
            this.liveImageMenuItem,
            this.mapMenuItem,
            this.telemetryMenuItem,
            this.graphMenuItem,
            this.predictorMenuItem});
            this.windowMenuItem.Name = "windowMenuItem";
            this.windowMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowMenuItem.Text = "&Window";
            // 
            // eventLogMenuItem
            // 
            this.eventLogMenuItem.Name = "eventLogMenuItem";
            this.eventLogMenuItem.Size = new System.Drawing.Size(152, 22);
            this.eventLogMenuItem.Text = "Event &Log";
            this.eventLogMenuItem.Click += new System.EventHandler(this.eventLogMenuItem_Click);
            // 
            // liveImageMenuItem
            // 
            this.liveImageMenuItem.Name = "liveImageMenuItem";
            this.liveImageMenuItem.Size = new System.Drawing.Size(152, 22);
            this.liveImageMenuItem.Text = "Live &Image";
            this.liveImageMenuItem.Click += new System.EventHandler(this.liveImageMenuItem_Click);
            // 
            // mapMenuItem
            // 
            this.mapMenuItem.Name = "mapMenuItem";
            this.mapMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mapMenuItem.Text = "&Map";
            this.mapMenuItem.Click += new System.EventHandler(this.mapMenuItem_Click);
            // 
            // telemetryMenuItem
            // 
            this.telemetryMenuItem.Name = "telemetryMenuItem";
            this.telemetryMenuItem.Size = new System.Drawing.Size(152, 22);
            this.telemetryMenuItem.Text = "&Telemetry";
            this.telemetryMenuItem.Click += new System.EventHandler(this.telemetryMenuItem_Click);
            // 
            // graphMenuItem
            // 
            this.graphMenuItem.Name = "graphMenuItem";
            this.graphMenuItem.Size = new System.Drawing.Size(152, 22);
            this.graphMenuItem.Text = "Graph";
            this.graphMenuItem.Click += new System.EventHandler(this.graphMenuItem_Click);
            // 
            // predictorMenuItem
            // 
            this.predictorMenuItem.Name = "predictorMenuItem";
            this.predictorMenuItem.Size = new System.Drawing.Size(152, 22);
            this.predictorMenuItem.Text = "Predictor";
            this.predictorMenuItem.Click += new System.EventHandler(this.predictorMenuItem_Click);
            // 
            // toolsMenuItem
            // 
            this.toolsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blogMessageMenuItem,
            this.flightradar24MenuItem,
            this.exportGoogleEarthTourMenuItem});
            this.toolsMenuItem.Name = "toolsMenuItem";
            this.toolsMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsMenuItem.Text = "&Tools";
            // 
            // blogMessageMenuItem
            // 
            this.blogMessageMenuItem.Name = "blogMessageMenuItem";
            this.blogMessageMenuItem.Size = new System.Drawing.Size(206, 22);
            this.blogMessageMenuItem.Text = "&Blog message";
            this.blogMessageMenuItem.Click += new System.EventHandler(this.blogMessageMenuItem_Click);
            // 
            // flightradar24MenuItem
            // 
            this.flightradar24MenuItem.CheckOnClick = true;
            this.flightradar24MenuItem.Name = "flightradar24MenuItem";
            this.flightradar24MenuItem.Size = new System.Drawing.Size(206, 22);
            this.flightradar24MenuItem.Text = "Enable Flightradar 24";
            this.flightradar24MenuItem.Click += new System.EventHandler(this.flightradar24ToolStripMenuItem_Click);
            // 
            // settingsMenuItem
            // 
            this.settingsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferencesMenuItem});
            this.settingsMenuItem.Name = "settingsMenuItem";
            this.settingsMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsMenuItem.Text = "&Settings";
            // 
            // preferencesMenuItem
            // 
            this.preferencesMenuItem.Name = "preferencesMenuItem";
            this.preferencesMenuItem.Size = new System.Drawing.Size(135, 22);
            this.preferencesMenuItem.Text = "&Preferences";
            this.preferencesMenuItem.Click += new System.EventHandler(this.preferencesMenuItem_Click);
            // 
            // exportGoogleEarthTourMenuItem
            // 
            this.exportGoogleEarthTourMenuItem.Name = "exportGoogleEarthTourMenuItem";
            this.exportGoogleEarthTourMenuItem.Size = new System.Drawing.Size(206, 22);
            this.exportGoogleEarthTourMenuItem.Text = "Export &Google Earth Tour";
            this.exportGoogleEarthTourMenuItem.Click += new System.EventHandler(this.exportGoogleEarthTourMenuItem_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 412);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.mainMenu);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainWindow";
            this.Text = "M3 Space Ground Control";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel transceiverStateLbl;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eventLogMenuItem;
        private System.Windows.Forms.ToolStripMenuItem liveImageMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem telemetryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadDataMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadPredictedMenuItem;
        private System.Windows.Forms.ToolStripMenuItem captureMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startCaptureMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopCaptureMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearDataMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel elapsedTitleLbl;
        private System.Windows.Forms.ToolStripStatusLabel elapsedLbl;
        private System.Windows.Forms.ToolStripMenuItem settingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel filenameLbl;
        private System.Windows.Forms.ToolStripMenuItem graphMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectGPSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem predictorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blogMessageMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flightradar24MenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportGoogleEarthTourMenuItem;
    }
}

