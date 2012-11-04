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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.mapTypeDropDown = new System.Windows.Forms.ToolStripComboBox();
            this.layerSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.layer1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layer2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layer3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapTypeDropDown,
            this.layerSplitButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(357, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // mapTypeDropDown
            // 
            this.mapTypeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mapTypeDropDown.Items.AddRange(new object[] {
            "GMaps",
            "GMaps Hybrid",
            "GMaps Terrain"});
            this.mapTypeDropDown.Name = "mapTypeDropDown";
            this.mapTypeDropDown.Size = new System.Drawing.Size(121, 25);
            this.mapTypeDropDown.SelectedIndexChanged += new System.EventHandler(this.mapTypeDropDown_SelectedIndexChanged);
            // 
            // layerSplitButton
            // 
            this.layerSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.layer1ToolStripMenuItem,
            this.layer2ToolStripMenuItem,
            this.layer3ToolStripMenuItem});
            this.layerSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.layerSplitButton.Name = "layerSplitButton";
            this.layerSplitButton.Size = new System.Drawing.Size(51, 22);
            this.layerSplitButton.Text = "Layer";
            // 
            // layer1ToolStripMenuItem
            // 
            this.layer1ToolStripMenuItem.Checked = true;
            this.layer1ToolStripMenuItem.CheckOnClick = true;
            this.layer1ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.layer1ToolStripMenuItem.Name = "layer1ToolStripMenuItem";
            this.layer1ToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.layer1ToolStripMenuItem.Text = "Prediction";
            this.layer1ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.layerToolStripMenuItem_CheckedChanged);
            // 
            // layer2ToolStripMenuItem
            // 
            this.layer2ToolStripMenuItem.Checked = true;
            this.layer2ToolStripMenuItem.CheckOnClick = true;
            this.layer2ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.layer2ToolStripMenuItem.Name = "layer2ToolStripMenuItem";
            this.layer2ToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.layer2ToolStripMenuItem.Text = "Balloon";
            this.layer2ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.layerToolStripMenuItem_CheckedChanged);
            // 
            // layer3ToolStripMenuItem
            // 
            this.layer3ToolStripMenuItem.Checked = true;
            this.layer3ToolStripMenuItem.CheckOnClick = true;
            this.layer3ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.layer3ToolStripMenuItem.Name = "layer3ToolStripMenuItem";
            this.layer3ToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.layer3ToolStripMenuItem.Text = "GroundControl";
            this.layer3ToolStripMenuItem.CheckedChanged += new System.EventHandler(this.layerToolStripMenuItem_CheckedChanged);
            // 
            // MapWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 241);
            this.ControlBox = false;
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
        private System.Windows.Forms.ToolStripMenuItem layer2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layer3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layer1ToolStripMenuItem;
    }
}