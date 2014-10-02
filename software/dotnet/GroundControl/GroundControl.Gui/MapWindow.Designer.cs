namespace GroundControl.Gui
{
    partial class MapWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapWindow));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.mapTypeDropDown = new System.Windows.Forms.ToolStripComboBox();
            this.layerSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.layerPredictionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layerBalloonMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layerGroundControlMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layerRouteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.alongPredictionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toBalloonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.map = new GMap.NET.WindowsForms.GMapControl();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapTypeDropDown,
            this.layerSplitButton,
            this.toolStripDropDownButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(482, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // mapTypeDropDown
            // 
            this.mapTypeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mapTypeDropDown.Name = "mapTypeDropDown";
            this.mapTypeDropDown.Size = new System.Drawing.Size(121, 25);
            this.mapTypeDropDown.SelectedIndexChanged += new System.EventHandler(this.mapTypeDropDown_SelectedIndexChanged);
            // 
            // layerSplitButton
            // 
            this.layerSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.layerPredictionMenuItem,
            this.layerBalloonMenuItem,
            this.layerGroundControlMenuItem,
            this.layerRouteMenuItem});
            this.layerSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.layerSplitButton.Name = "layerSplitButton";
            this.layerSplitButton.Size = new System.Drawing.Size(51, 22);
            this.layerSplitButton.Text = "Layer";
            // 
            // layerPredictionMenuItem
            // 
            this.layerPredictionMenuItem.Checked = true;
            this.layerPredictionMenuItem.CheckOnClick = true;
            this.layerPredictionMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.layerPredictionMenuItem.Name = "layerPredictionMenuItem";
            this.layerPredictionMenuItem.Size = new System.Drawing.Size(154, 22);
            this.layerPredictionMenuItem.Text = "Prediction";
            this.layerPredictionMenuItem.CheckedChanged += new System.EventHandler(this.layerToolStripMenuItem_CheckedChanged);
            // 
            // layerBalloonMenuItem
            // 
            this.layerBalloonMenuItem.Checked = true;
            this.layerBalloonMenuItem.CheckOnClick = true;
            this.layerBalloonMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.layerBalloonMenuItem.Name = "layerBalloonMenuItem";
            this.layerBalloonMenuItem.Size = new System.Drawing.Size(154, 22);
            this.layerBalloonMenuItem.Text = "Balloon";
            this.layerBalloonMenuItem.CheckedChanged += new System.EventHandler(this.layerToolStripMenuItem_CheckedChanged);
            // 
            // layerGroundControlMenuItem
            // 
            this.layerGroundControlMenuItem.Checked = true;
            this.layerGroundControlMenuItem.CheckOnClick = true;
            this.layerGroundControlMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.layerGroundControlMenuItem.Name = "layerGroundControlMenuItem";
            this.layerGroundControlMenuItem.Size = new System.Drawing.Size(154, 22);
            this.layerGroundControlMenuItem.Text = "GroundControl";
            this.layerGroundControlMenuItem.CheckedChanged += new System.EventHandler(this.layerToolStripMenuItem_CheckedChanged);
            // 
            // layerRouteMenuItem
            // 
            this.layerRouteMenuItem.Checked = true;
            this.layerRouteMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.layerRouteMenuItem.Name = "layerRouteMenuItem";
            this.layerRouteMenuItem.Size = new System.Drawing.Size(154, 22);
            this.layerRouteMenuItem.Text = "Route";
            this.layerRouteMenuItem.Click += new System.EventHandler(this.layerToolStripMenuItem_CheckedChanged);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alongPredictionToolStripMenuItem,
            this.toBalloonToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(51, 22);
            this.toolStripDropDownButton1.Text = "Route";
            // 
            // alongPredictionToolStripMenuItem
            // 
            this.alongPredictionToolStripMenuItem.Name = "alongPredictionToolStripMenuItem";
            this.alongPredictionToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.alongPredictionToolStripMenuItem.Text = "Along prediction";
            this.alongPredictionToolStripMenuItem.Click += new System.EventHandler(this.alongPredictionToolStripMenuItem_Click);
            // 
            // toBalloonToolStripMenuItem
            // 
            this.toBalloonToolStripMenuItem.Name = "toBalloonToolStripMenuItem";
            this.toBalloonToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.toBalloonToolStripMenuItem.Text = "To balloon";
            this.toBalloonToolStripMenuItem.Click += new System.EventHandler(this.toBalloonToolStripMenuItem_Click);
            // 
            // map
            // 
            this.map.Bearing = 0F;
            this.map.CanDragMap = true;
            this.map.Dock = System.Windows.Forms.DockStyle.Fill;
            this.map.EmptyTileColor = System.Drawing.Color.Navy;
            this.map.GrayScaleMode = false;
            this.map.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.map.LevelsKeepInMemmory = 5;
            this.map.Location = new System.Drawing.Point(0, 25);
            this.map.MarkersEnabled = true;
            this.map.MaxZoom = 24;
            this.map.MinZoom = 0;
            this.map.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.map.Name = "map";
            this.map.NegativeMode = false;
            this.map.PolygonsEnabled = true;
            this.map.RetryLoadTile = 0;
            this.map.RoutesEnabled = true;
            this.map.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.map.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.map.ShowTileGridLines = false;
            this.map.Size = new System.Drawing.Size(482, 216);
            this.map.TabIndex = 1;
            this.map.Zoom = 15D;
            this.map.OnPositionChanged += new GMap.NET.PositionChanged(this.map_OnPositionChanged);
            // 
            // MapWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 241);
            this.ControlBox = false;
            this.Controls.Add(this.map);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Location = new System.Drawing.Point(250, 0);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Map";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox mapTypeDropDown;
        private System.Windows.Forms.ToolStripSplitButton layerSplitButton;
        private System.Windows.Forms.ToolStripMenuItem layerBalloonMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layerGroundControlMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layerPredictionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layerRouteMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem alongPredictionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toBalloonToolStripMenuItem;
        private GMap.NET.WindowsForms.GMapControl map;
    }
}