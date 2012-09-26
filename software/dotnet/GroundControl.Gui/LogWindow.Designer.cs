namespace GroundControl.Gui
{
    partial class LogWindow
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
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.logContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearLogMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // logTextBox
            // 
            this.logTextBox.BackColor = System.Drawing.Color.White;
            this.logTextBox.ContextMenuStrip = this.logContextMenu;
            this.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logTextBox.ForeColor = System.Drawing.Color.Black;
            this.logTextBox.Location = new System.Drawing.Point(0, 0);
            this.logTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(484, 116);
            this.logTextBox.TabIndex = 0;
            // 
            // logContextMenu
            // 
            this.logContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearLogMenuItem});
            this.logContextMenu.Name = "logContextMenu";
            this.logContextMenu.Size = new System.Drawing.Size(153, 48);
            // 
            // clearLogMenuItem
            // 
            this.clearLogMenuItem.Name = "clearLogMenuItem";
            this.clearLogMenuItem.Size = new System.Drawing.Size(152, 22);
            this.clearLogMenuItem.Text = "&Clear Log";
            this.clearLogMenuItem.Click += new System.EventHandler(this.clearLogMenuItem_Click);
            // 
            // LogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 116);
            this.ControlBox = false;
            this.Controls.Add(this.logTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Location = new System.Drawing.Point(285, 280);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Event Log";
            this.logContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.ContextMenuStrip logContextMenu;
        private System.Windows.Forms.ToolStripMenuItem clearLogMenuItem;
    }
}