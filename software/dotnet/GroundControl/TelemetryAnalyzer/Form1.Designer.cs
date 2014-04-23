namespace TelemetryAnalyzer
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnOpen = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.OverviewDataTable = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flightTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timeToBurstDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flightDistanceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.launchLandingDistanceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.maxAltitudeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.maxHSpeedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.maxVSpeedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.avgAscentRateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.avgDescentRateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.coldestTemperatureDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.burstTemperatureDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.batteryAtLandingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.missionManagerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.m_map = new GMap.NET.WindowsForms.GMapControl();
            this.MapTypeDropDown = new System.Windows.Forms.ComboBox();
            this.splitMap = new System.Windows.Forms.SplitContainer();
            this.splitData = new System.Windows.Forms.SplitContainer();
            this.splitGraph = new System.Windows.Forms.SplitContainer();
            this.splitVideo = new System.Windows.Forms.SplitContainer();
            this.imageViewer1 = new TelemetryAnalyzer.ImageViewer();
            this.imageViewer2 = new TelemetryAnalyzer.ImageViewer();
            this.TelemetryDataTable = new System.Windows.Forms.DataGridView();
            this.utcTimestampDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.latitudeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.longitudeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gpsAltitudeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.headingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.horizontalSpeedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.verticalSpeedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.satellitesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.intTemperatureDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.temperature1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.temperature2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pressureDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pressureAltitudeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vinDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.temperature1RawDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.temperature2RawDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vinRawDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dutyCycleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.missionBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.splitMissions = new System.Windows.Forms.SplitContainer();
            this.chkboxSyncMode = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.OverviewDataTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.missionManagerBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMap)).BeginInit();
            this.splitMap.Panel1.SuspendLayout();
            this.splitMap.Panel2.SuspendLayout();
            this.splitMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitData)).BeginInit();
            this.splitData.Panel1.SuspendLayout();
            this.splitData.Panel2.SuspendLayout();
            this.splitData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitGraph)).BeginInit();
            this.splitGraph.Panel1.SuspendLayout();
            this.splitGraph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitVideo)).BeginInit();
            this.splitVideo.Panel1.SuspendLayout();
            this.splitVideo.Panel2.SuspendLayout();
            this.splitVideo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TelemetryDataTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.missionBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMissions)).BeginInit();
            this.splitMissions.Panel1.SuspendLayout();
            this.splitMissions.Panel2.SuspendLayout();
            this.splitMissions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(11, 11);
            this.btnOpen.Margin = new System.Windows.Forms.Padding(2);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(93, 23);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Text = "Import Mission";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Multiselect = true;
            // 
            // OverviewDataTable
            // 
            this.OverviewDataTable.AllowUserToAddRows = false;
            this.OverviewDataTable.AllowUserToDeleteRows = false;
            this.OverviewDataTable.AutoGenerateColumns = false;
            this.OverviewDataTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.916231F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.OverviewDataTable.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.OverviewDataTable.ColumnHeadersHeight = 60;
            this.OverviewDataTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.startDateDataGridViewTextBoxColumn,
            this.flightTimeDataGridViewTextBoxColumn,
            this.timeToBurstDataGridViewTextBoxColumn,
            this.flightDistanceDataGridViewTextBoxColumn,
            this.launchLandingDistanceDataGridViewTextBoxColumn,
            this.maxAltitudeDataGridViewTextBoxColumn,
            this.maxHSpeedDataGridViewTextBoxColumn,
            this.maxVSpeedDataGridViewTextBoxColumn,
            this.avgAscentRateDataGridViewTextBoxColumn,
            this.avgDescentRateDataGridViewTextBoxColumn,
            this.coldestTemperatureDataGridViewTextBoxColumn,
            this.burstTemperatureDataGridViewTextBoxColumn,
            this.batteryAtLandingDataGridViewTextBoxColumn});
            this.OverviewDataTable.DataSource = this.missionManagerBindingSource;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.916231F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.OverviewDataTable.DefaultCellStyle = dataGridViewCellStyle6;
            this.OverviewDataTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OverviewDataTable.Location = new System.Drawing.Point(0, 0);
            this.OverviewDataTable.Margin = new System.Windows.Forms.Padding(2);
            this.OverviewDataTable.Name = "OverviewDataTable";
            this.OverviewDataTable.ReadOnly = true;
            this.OverviewDataTable.RowHeadersVisible = false;
            this.OverviewDataTable.RowTemplate.Height = 33;
            this.OverviewDataTable.Size = new System.Drawing.Size(967, 148);
            this.OverviewDataTable.TabIndex = 1;
            this.OverviewDataTable.SelectionChanged += new System.EventHandler(this.OverviewDataTable_SelectionChanged);
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Mission Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            this.nameDataGridViewTextBoxColumn.Width = 90;
            // 
            // startDateDataGridViewTextBoxColumn
            // 
            this.startDateDataGridViewTextBoxColumn.DataPropertyName = "StartDate";
            this.startDateDataGridViewTextBoxColumn.HeaderText = "Launch Date";
            this.startDateDataGridViewTextBoxColumn.Name = "startDateDataGridViewTextBoxColumn";
            this.startDateDataGridViewTextBoxColumn.ReadOnly = true;
            this.startDateDataGridViewTextBoxColumn.Width = 87;
            // 
            // flightTimeDataGridViewTextBoxColumn
            // 
            this.flightTimeDataGridViewTextBoxColumn.DataPropertyName = "FlightTime";
            this.flightTimeDataGridViewTextBoxColumn.HeaderText = "Flight Time [hh:mm:ss]";
            this.flightTimeDataGridViewTextBoxColumn.Name = "flightTimeDataGridViewTextBoxColumn";
            this.flightTimeDataGridViewTextBoxColumn.ReadOnly = true;
            this.flightTimeDataGridViewTextBoxColumn.Width = 124;
            // 
            // timeToBurstDataGridViewTextBoxColumn
            // 
            this.timeToBurstDataGridViewTextBoxColumn.DataPropertyName = "TimeToBurst";
            this.timeToBurstDataGridViewTextBoxColumn.HeaderText = "Time to burst [hh:mm:ss]";
            this.timeToBurstDataGridViewTextBoxColumn.Name = "timeToBurstDataGridViewTextBoxColumn";
            this.timeToBurstDataGridViewTextBoxColumn.ReadOnly = true;
            this.timeToBurstDataGridViewTextBoxColumn.Width = 88;
            // 
            // flightDistanceDataGridViewTextBoxColumn
            // 
            this.flightDistanceDataGridViewTextBoxColumn.DataPropertyName = "FlightDistance";
            this.flightDistanceDataGridViewTextBoxColumn.HeaderText = "Flight Distance [km]";
            this.flightDistanceDataGridViewTextBoxColumn.Name = "flightDistanceDataGridViewTextBoxColumn";
            this.flightDistanceDataGridViewTextBoxColumn.ReadOnly = true;
            this.flightDistanceDataGridViewTextBoxColumn.Width = 96;
            // 
            // launchLandingDistanceDataGridViewTextBoxColumn
            // 
            this.launchLandingDistanceDataGridViewTextBoxColumn.DataPropertyName = "LaunchLandingDistance";
            this.launchLandingDistanceDataGridViewTextBoxColumn.HeaderText = "Launch - Landing Distance [km]";
            this.launchLandingDistanceDataGridViewTextBoxColumn.Name = "launchLandingDistanceDataGridViewTextBoxColumn";
            this.launchLandingDistanceDataGridViewTextBoxColumn.ReadOnly = true;
            this.launchLandingDistanceDataGridViewTextBoxColumn.Width = 89;
            // 
            // maxAltitudeDataGridViewTextBoxColumn
            // 
            this.maxAltitudeDataGridViewTextBoxColumn.DataPropertyName = "MaxAltitude";
            this.maxAltitudeDataGridViewTextBoxColumn.HeaderText = "Maximum Altitude [m]";
            this.maxAltitudeDataGridViewTextBoxColumn.Name = "maxAltitudeDataGridViewTextBoxColumn";
            this.maxAltitudeDataGridViewTextBoxColumn.ReadOnly = true;
            this.maxAltitudeDataGridViewTextBoxColumn.Width = 78;
            // 
            // maxHSpeedDataGridViewTextBoxColumn
            // 
            this.maxHSpeedDataGridViewTextBoxColumn.DataPropertyName = "MaxHSpeed";
            this.maxHSpeedDataGridViewTextBoxColumn.HeaderText = "Maximum Horizontal Speed [km/h]";
            this.maxHSpeedDataGridViewTextBoxColumn.Name = "maxHSpeedDataGridViewTextBoxColumn";
            this.maxHSpeedDataGridViewTextBoxColumn.ReadOnly = true;
            this.maxHSpeedDataGridViewTextBoxColumn.Width = 89;
            // 
            // maxVSpeedDataGridViewTextBoxColumn
            // 
            this.maxVSpeedDataGridViewTextBoxColumn.DataPropertyName = "MaxVSpeed";
            this.maxVSpeedDataGridViewTextBoxColumn.HeaderText = "Maximum Vertical Speed [km/h]";
            this.maxVSpeedDataGridViewTextBoxColumn.Name = "maxVSpeedDataGridViewTextBoxColumn";
            this.maxVSpeedDataGridViewTextBoxColumn.ReadOnly = true;
            this.maxVSpeedDataGridViewTextBoxColumn.Width = 96;
            // 
            // avgAscentRateDataGridViewTextBoxColumn
            // 
            this.avgAscentRateDataGridViewTextBoxColumn.DataPropertyName = "AvgAscentRate";
            this.avgAscentRateDataGridViewTextBoxColumn.HeaderText = "Average Ascent Rate [m/s]";
            this.avgAscentRateDataGridViewTextBoxColumn.Name = "avgAscentRateDataGridViewTextBoxColumn";
            this.avgAscentRateDataGridViewTextBoxColumn.ReadOnly = true;
            this.avgAscentRateDataGridViewTextBoxColumn.Width = 76;
            // 
            // avgDescentRateDataGridViewTextBoxColumn
            // 
            this.avgDescentRateDataGridViewTextBoxColumn.DataPropertyName = "AvgDescentRate";
            this.avgDescentRateDataGridViewTextBoxColumn.HeaderText = "Average Descent Rate [m/s]";
            this.avgDescentRateDataGridViewTextBoxColumn.Name = "avgDescentRateDataGridViewTextBoxColumn";
            this.avgDescentRateDataGridViewTextBoxColumn.ReadOnly = true;
            this.avgDescentRateDataGridViewTextBoxColumn.Width = 76;
            // 
            // coldestTemperatureDataGridViewTextBoxColumn
            // 
            this.coldestTemperatureDataGridViewTextBoxColumn.DataPropertyName = "ColdestTemperature";
            this.coldestTemperatureDataGridViewTextBoxColumn.HeaderText = "Coldest Temperature [°C]";
            this.coldestTemperatureDataGridViewTextBoxColumn.Name = "coldestTemperatureDataGridViewTextBoxColumn";
            this.coldestTemperatureDataGridViewTextBoxColumn.ReadOnly = true;
            this.coldestTemperatureDataGridViewTextBoxColumn.Width = 103;
            // 
            // burstTemperatureDataGridViewTextBoxColumn
            // 
            this.burstTemperatureDataGridViewTextBoxColumn.DataPropertyName = "BurstTemperature";
            this.burstTemperatureDataGridViewTextBoxColumn.HeaderText = "Burst Temperature [°C]";
            this.burstTemperatureDataGridViewTextBoxColumn.Name = "burstTemperatureDataGridViewTextBoxColumn";
            this.burstTemperatureDataGridViewTextBoxColumn.ReadOnly = true;
            this.burstTemperatureDataGridViewTextBoxColumn.Width = 103;
            // 
            // batteryAtLandingDataGridViewTextBoxColumn
            // 
            this.batteryAtLandingDataGridViewTextBoxColumn.DataPropertyName = "BatteryAtLanding";
            this.batteryAtLandingDataGridViewTextBoxColumn.HeaderText = "Battery at Landing [V]";
            this.batteryAtLandingDataGridViewTextBoxColumn.Name = "batteryAtLandingDataGridViewTextBoxColumn";
            this.batteryAtLandingDataGridViewTextBoxColumn.ReadOnly = true;
            this.batteryAtLandingDataGridViewTextBoxColumn.Width = 71;
            // 
            // missionManagerBindingSource
            // 
            this.missionManagerBindingSource.DataSource = typeof(TelemetryAnalyzer.Mission);
            // 
            // m_map
            // 
            this.m_map.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_map.Bearing = 0F;
            this.m_map.CanDragMap = true;
            this.m_map.EmptyTileColor = System.Drawing.Color.Navy;
            this.m_map.GrayScaleMode = false;
            this.m_map.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.m_map.LevelsKeepInMemmory = 5;
            this.m_map.Location = new System.Drawing.Point(3, 28);
            this.m_map.MarkersEnabled = true;
            this.m_map.MaxZoom = 24;
            this.m_map.MinZoom = 0;
            this.m_map.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.m_map.Name = "m_map";
            this.m_map.NegativeMode = false;
            this.m_map.PolygonsEnabled = true;
            this.m_map.RetryLoadTile = 0;
            this.m_map.RoutesEnabled = true;
            this.m_map.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Fractional;
            this.m_map.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.m_map.ShowTileGridLines = false;
            this.m_map.Size = new System.Drawing.Size(506, 393);
            this.m_map.TabIndex = 2;
            this.m_map.Zoom = 8D;
            // 
            // MapTypeDropDown
            // 
            this.MapTypeDropDown.FormattingEnabled = true;
            this.MapTypeDropDown.Location = new System.Drawing.Point(2, 2);
            this.MapTypeDropDown.Margin = new System.Windows.Forms.Padding(2);
            this.MapTypeDropDown.Name = "MapTypeDropDown";
            this.MapTypeDropDown.Size = new System.Drawing.Size(244, 21);
            this.MapTypeDropDown.TabIndex = 3;
            this.MapTypeDropDown.SelectedIndexChanged += new System.EventHandler(this.MapTypeDropDown_SelectedIndexChanged);
            // 
            // splitMap
            // 
            this.splitMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMap.Location = new System.Drawing.Point(0, 0);
            this.splitMap.Margin = new System.Windows.Forms.Padding(2);
            this.splitMap.Name = "splitMap";
            // 
            // splitMap.Panel1
            // 
            this.splitMap.Panel1.Controls.Add(this.splitData);
            // 
            // splitMap.Panel2
            // 
            this.splitMap.Panel2.Controls.Add(this.m_map);
            this.splitMap.Panel2.Controls.Add(this.MapTypeDropDown);
            this.splitMap.Size = new System.Drawing.Size(969, 426);
            this.splitMap.SplitterDistance = 459;
            this.splitMap.SplitterWidth = 2;
            this.splitMap.TabIndex = 4;
            // 
            // splitData
            // 
            this.splitData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitData.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitData.Location = new System.Drawing.Point(0, 0);
            this.splitData.Margin = new System.Windows.Forms.Padding(2);
            this.splitData.Name = "splitData";
            this.splitData.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitData.Panel1
            // 
            this.splitData.Panel1.Controls.Add(this.splitGraph);
            // 
            // splitData.Panel2
            // 
            this.splitData.Panel2.Controls.Add(this.TelemetryDataTable);
            this.splitData.Size = new System.Drawing.Size(459, 426);
            this.splitData.SplitterDistance = 261;
            this.splitData.SplitterWidth = 2;
            this.splitData.TabIndex = 0;
            // 
            // splitGraph
            // 
            this.splitGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitGraph.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitGraph.Location = new System.Drawing.Point(0, 0);
            this.splitGraph.Margin = new System.Windows.Forms.Padding(2);
            this.splitGraph.Name = "splitGraph";
            this.splitGraph.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitGraph.Panel1
            // 
            this.splitGraph.Panel1.Controls.Add(this.splitVideo);
            // 
            // splitGraph.Panel2
            // 
            this.splitGraph.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitGraph_Panel2_Paint);
            this.splitGraph.Size = new System.Drawing.Size(459, 261);
            this.splitGraph.SplitterDistance = 155;
            this.splitGraph.SplitterWidth = 2;
            this.splitGraph.TabIndex = 0;
            // 
            // splitVideo
            // 
            this.splitVideo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitVideo.Location = new System.Drawing.Point(0, 0);
            this.splitVideo.Margin = new System.Windows.Forms.Padding(2);
            this.splitVideo.Name = "splitVideo";
            // 
            // splitVideo.Panel1
            // 
            this.splitVideo.Panel1.Controls.Add(this.imageViewer1);
            // 
            // splitVideo.Panel2
            // 
            this.splitVideo.Panel2.Controls.Add(this.imageViewer2);
            this.splitVideo.Size = new System.Drawing.Size(459, 155);
            this.splitVideo.SplitterDistance = 219;
            this.splitVideo.SplitterWidth = 2;
            this.splitVideo.TabIndex = 1;
            // 
            // imageViewer1
            // 
            this.imageViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageViewer1.Image = null;
            this.imageViewer1.Location = new System.Drawing.Point(0, 0);
            this.imageViewer1.Margin = new System.Windows.Forms.Padding(2);
            this.imageViewer1.Name = "imageViewer1";
            this.imageViewer1.Size = new System.Drawing.Size(217, 153);
            this.imageViewer1.TabIndex = 0;
            this.imageViewer1.ImageChanged += new TelemetryAnalyzer.ImageViewer.ImageChangeDelegate(this.imageViewer1_ImageChanged);
            // 
            // imageViewer2
            // 
            this.imageViewer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageViewer2.Image = null;
            this.imageViewer2.Location = new System.Drawing.Point(0, 0);
            this.imageViewer2.Margin = new System.Windows.Forms.Padding(2);
            this.imageViewer2.Name = "imageViewer2";
            this.imageViewer2.Size = new System.Drawing.Size(236, 153);
            this.imageViewer2.TabIndex = 1;
            this.imageViewer2.ImageChanged += new TelemetryAnalyzer.ImageViewer.ImageChangeDelegate(this.imageViewer2_ImageChanged);
            // 
            // TelemetryDataTable
            // 
            this.TelemetryDataTable.AutoGenerateColumns = false;
            this.TelemetryDataTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TelemetryDataTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.utcTimestampDataGridViewTextBoxColumn,
            this.latitudeDataGridViewTextBoxColumn,
            this.longitudeDataGridViewTextBoxColumn,
            this.gpsAltitudeDataGridViewTextBoxColumn,
            this.headingDataGridViewTextBoxColumn,
            this.horizontalSpeedDataGridViewTextBoxColumn,
            this.verticalSpeedDataGridViewTextBoxColumn,
            this.satellitesDataGridViewTextBoxColumn,
            this.intTemperatureDataGridViewTextBoxColumn,
            this.temperature1DataGridViewTextBoxColumn,
            this.temperature2DataGridViewTextBoxColumn,
            this.pressureDataGridViewTextBoxColumn,
            this.pressureAltitudeDataGridViewTextBoxColumn,
            this.vinDataGridViewTextBoxColumn,
            this.temperature1RawDataGridViewTextBoxColumn,
            this.temperature2RawDataGridViewTextBoxColumn,
            this.vinRawDataGridViewTextBoxColumn,
            this.dutyCycleDataGridViewTextBoxColumn});
            this.TelemetryDataTable.DataSource = this.missionBindingSource;
            this.TelemetryDataTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TelemetryDataTable.Location = new System.Drawing.Point(0, 0);
            this.TelemetryDataTable.Margin = new System.Windows.Forms.Padding(2);
            this.TelemetryDataTable.Name = "TelemetryDataTable";
            this.TelemetryDataTable.RowHeadersVisible = false;
            this.TelemetryDataTable.RowTemplate.Height = 33;
            this.TelemetryDataTable.Size = new System.Drawing.Size(457, 161);
            this.TelemetryDataTable.TabIndex = 0;
            this.TelemetryDataTable.SelectionChanged += new System.EventHandler(this.TelemetryDataTable_SelectionChanged);
            // 
            // utcTimestampDataGridViewTextBoxColumn
            // 
            this.utcTimestampDataGridViewTextBoxColumn.DataPropertyName = "UtcTimestamp";
            this.utcTimestampDataGridViewTextBoxColumn.HeaderText = "UtcTimestamp";
            this.utcTimestampDataGridViewTextBoxColumn.Name = "utcTimestampDataGridViewTextBoxColumn";
            // 
            // latitudeDataGridViewTextBoxColumn
            // 
            this.latitudeDataGridViewTextBoxColumn.DataPropertyName = "Latitude";
            this.latitudeDataGridViewTextBoxColumn.HeaderText = "Latitude";
            this.latitudeDataGridViewTextBoxColumn.Name = "latitudeDataGridViewTextBoxColumn";
            // 
            // longitudeDataGridViewTextBoxColumn
            // 
            this.longitudeDataGridViewTextBoxColumn.DataPropertyName = "Longitude";
            this.longitudeDataGridViewTextBoxColumn.HeaderText = "Longitude";
            this.longitudeDataGridViewTextBoxColumn.Name = "longitudeDataGridViewTextBoxColumn";
            // 
            // gpsAltitudeDataGridViewTextBoxColumn
            // 
            this.gpsAltitudeDataGridViewTextBoxColumn.DataPropertyName = "GpsAltitude";
            this.gpsAltitudeDataGridViewTextBoxColumn.HeaderText = "GpsAltitude";
            this.gpsAltitudeDataGridViewTextBoxColumn.Name = "gpsAltitudeDataGridViewTextBoxColumn";
            // 
            // headingDataGridViewTextBoxColumn
            // 
            this.headingDataGridViewTextBoxColumn.DataPropertyName = "Heading";
            this.headingDataGridViewTextBoxColumn.HeaderText = "Heading";
            this.headingDataGridViewTextBoxColumn.Name = "headingDataGridViewTextBoxColumn";
            // 
            // horizontalSpeedDataGridViewTextBoxColumn
            // 
            this.horizontalSpeedDataGridViewTextBoxColumn.DataPropertyName = "HorizontalSpeed";
            this.horizontalSpeedDataGridViewTextBoxColumn.HeaderText = "HorizontalSpeed";
            this.horizontalSpeedDataGridViewTextBoxColumn.Name = "horizontalSpeedDataGridViewTextBoxColumn";
            // 
            // verticalSpeedDataGridViewTextBoxColumn
            // 
            this.verticalSpeedDataGridViewTextBoxColumn.DataPropertyName = "VerticalSpeed";
            this.verticalSpeedDataGridViewTextBoxColumn.HeaderText = "VerticalSpeed";
            this.verticalSpeedDataGridViewTextBoxColumn.Name = "verticalSpeedDataGridViewTextBoxColumn";
            // 
            // satellitesDataGridViewTextBoxColumn
            // 
            this.satellitesDataGridViewTextBoxColumn.DataPropertyName = "Satellites";
            this.satellitesDataGridViewTextBoxColumn.HeaderText = "Satellites";
            this.satellitesDataGridViewTextBoxColumn.Name = "satellitesDataGridViewTextBoxColumn";
            // 
            // intTemperatureDataGridViewTextBoxColumn
            // 
            this.intTemperatureDataGridViewTextBoxColumn.DataPropertyName = "IntTemperature";
            this.intTemperatureDataGridViewTextBoxColumn.HeaderText = "IntTemperature";
            this.intTemperatureDataGridViewTextBoxColumn.Name = "intTemperatureDataGridViewTextBoxColumn";
            // 
            // temperature1DataGridViewTextBoxColumn
            // 
            this.temperature1DataGridViewTextBoxColumn.DataPropertyName = "Temperature1";
            this.temperature1DataGridViewTextBoxColumn.HeaderText = "Temperature1";
            this.temperature1DataGridViewTextBoxColumn.Name = "temperature1DataGridViewTextBoxColumn";
            // 
            // temperature2DataGridViewTextBoxColumn
            // 
            this.temperature2DataGridViewTextBoxColumn.DataPropertyName = "Temperature2";
            this.temperature2DataGridViewTextBoxColumn.HeaderText = "Temperature2";
            this.temperature2DataGridViewTextBoxColumn.Name = "temperature2DataGridViewTextBoxColumn";
            // 
            // pressureDataGridViewTextBoxColumn
            // 
            this.pressureDataGridViewTextBoxColumn.DataPropertyName = "Pressure";
            this.pressureDataGridViewTextBoxColumn.HeaderText = "Pressure";
            this.pressureDataGridViewTextBoxColumn.Name = "pressureDataGridViewTextBoxColumn";
            // 
            // pressureAltitudeDataGridViewTextBoxColumn
            // 
            this.pressureAltitudeDataGridViewTextBoxColumn.DataPropertyName = "PressureAltitude";
            this.pressureAltitudeDataGridViewTextBoxColumn.HeaderText = "PressureAltitude";
            this.pressureAltitudeDataGridViewTextBoxColumn.Name = "pressureAltitudeDataGridViewTextBoxColumn";
            // 
            // vinDataGridViewTextBoxColumn
            // 
            this.vinDataGridViewTextBoxColumn.DataPropertyName = "Vin";
            this.vinDataGridViewTextBoxColumn.HeaderText = "Vin";
            this.vinDataGridViewTextBoxColumn.Name = "vinDataGridViewTextBoxColumn";
            // 
            // temperature1RawDataGridViewTextBoxColumn
            // 
            this.temperature1RawDataGridViewTextBoxColumn.DataPropertyName = "Temperature1Raw";
            this.temperature1RawDataGridViewTextBoxColumn.HeaderText = "Temperature1Raw";
            this.temperature1RawDataGridViewTextBoxColumn.Name = "temperature1RawDataGridViewTextBoxColumn";
            // 
            // temperature2RawDataGridViewTextBoxColumn
            // 
            this.temperature2RawDataGridViewTextBoxColumn.DataPropertyName = "Temperature2Raw";
            this.temperature2RawDataGridViewTextBoxColumn.HeaderText = "Temperature2Raw";
            this.temperature2RawDataGridViewTextBoxColumn.Name = "temperature2RawDataGridViewTextBoxColumn";
            // 
            // vinRawDataGridViewTextBoxColumn
            // 
            this.vinRawDataGridViewTextBoxColumn.DataPropertyName = "VinRaw";
            this.vinRawDataGridViewTextBoxColumn.HeaderText = "VinRaw";
            this.vinRawDataGridViewTextBoxColumn.Name = "vinRawDataGridViewTextBoxColumn";
            // 
            // dutyCycleDataGridViewTextBoxColumn
            // 
            this.dutyCycleDataGridViewTextBoxColumn.DataPropertyName = "DutyCycle";
            this.dutyCycleDataGridViewTextBoxColumn.HeaderText = "DutyCycle";
            this.dutyCycleDataGridViewTextBoxColumn.Name = "dutyCycleDataGridViewTextBoxColumn";
            // 
            // missionBindingSource
            // 
            this.missionBindingSource.DataMember = "Flight";
            this.missionBindingSource.DataSource = typeof(TelemetryAnalyzer.Mission);
            // 
            // splitMissions
            // 
            this.splitMissions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitMissions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitMissions.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitMissions.Location = new System.Drawing.Point(11, 38);
            this.splitMissions.Margin = new System.Windows.Forms.Padding(2);
            this.splitMissions.Name = "splitMissions";
            this.splitMissions.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMissions.Panel1
            // 
            this.splitMissions.Panel1.Controls.Add(this.OverviewDataTable);
            // 
            // splitMissions.Panel2
            // 
            this.splitMissions.Panel2.Controls.Add(this.splitMap);
            this.splitMissions.Size = new System.Drawing.Size(969, 578);
            this.splitMissions.SplitterDistance = 150;
            this.splitMissions.SplitterWidth = 2;
            this.splitMissions.TabIndex = 6;
            // 
            // chkboxSyncMode
            // 
            this.chkboxSyncMode.AutoSize = true;
            this.chkboxSyncMode.Location = new System.Drawing.Point(149, 15);
            this.chkboxSyncMode.Name = "chkboxSyncMode";
            this.chkboxSyncMode.Size = new System.Drawing.Size(80, 17);
            this.chkboxSyncMode.TabIndex = 7;
            this.chkboxSyncMode.Text = "Sync Mode";
            this.chkboxSyncMode.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(991, 627);
            this.Controls.Add(this.chkboxSyncMode);
            this.Controls.Add(this.splitMissions);
            this.Controls.Add(this.btnOpen);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Telemetry Data Analyzer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.OverviewDataTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.missionManagerBindingSource)).EndInit();
            this.splitMap.Panel1.ResumeLayout(false);
            this.splitMap.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMap)).EndInit();
            this.splitMap.ResumeLayout(false);
            this.splitData.Panel1.ResumeLayout(false);
            this.splitData.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitData)).EndInit();
            this.splitData.ResumeLayout(false);
            this.splitGraph.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitGraph)).EndInit();
            this.splitGraph.ResumeLayout(false);
            this.splitVideo.Panel1.ResumeLayout(false);
            this.splitVideo.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitVideo)).EndInit();
            this.splitVideo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TelemetryDataTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.missionBindingSource)).EndInit();
            this.splitMissions.Panel1.ResumeLayout(false);
            this.splitMissions.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMissions)).EndInit();
            this.splitMissions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.DataGridView OverviewDataTable;
        private System.Windows.Forms.DataGridViewCheckBoxColumn lockedDataGridViewCheckBoxColumn;
        private System.Windows.Forms.BindingSource missionManagerBindingSource;
        private GMap.NET.WindowsForms.GMapControl m_map;
        private System.Windows.Forms.ComboBox MapTypeDropDown;
        private System.Windows.Forms.SplitContainer splitMap;
        private System.Windows.Forms.SplitContainer splitMissions;
        private System.Windows.Forms.DataGridView TelemetryDataTable;
        private System.Windows.Forms.SplitContainer splitData;
        private System.Windows.Forms.BindingSource missionBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn utcTimestampDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn latitudeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn longitudeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn gpsAltitudeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn headingDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn horizontalSpeedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn verticalSpeedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn satellitesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn intTemperatureDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn temperature1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn temperature2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pressureDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pressureAltitudeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn vinDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn temperature1RawDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn temperature2RawDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn vinRawDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dutyCycleDataGridViewTextBoxColumn;
        private System.Windows.Forms.SplitContainer splitVideo;
        private System.Windows.Forms.SplitContainer splitGraph;
        private ImageViewer imageViewer1;
        private ImageViewer imageViewer2;
        private System.Windows.Forms.CheckBox chkboxSyncMode;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn startDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn flightTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeToBurstDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn flightDistanceDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn launchLandingDistanceDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn maxAltitudeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn maxHSpeedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn maxVSpeedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn avgAscentRateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn avgDescentRateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn coldestTemperatureDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn burstTemperatureDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn batteryAtLandingDataGridViewTextBoxColumn;
    }
}

