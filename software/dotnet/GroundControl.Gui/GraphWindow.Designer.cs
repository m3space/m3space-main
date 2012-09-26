namespace GroundControl.Gui
{
    partial class GraphWindow
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
            this.m_display = new GraphLib.PlotterDisplayEx();
            this.SuspendLayout();
            // 
            // m_display
            // 
            this.m_display.BackColor = System.Drawing.Color.Transparent;
            this.m_display.BackgroundColorBot = System.Drawing.Color.White;
            this.m_display.BackgroundColorTop = System.Drawing.Color.White;
            this.m_display.DashedGridColor = System.Drawing.Color.DarkGray;
            this.m_display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_display.DoubleBuffering = true;
            this.m_display.Location = new System.Drawing.Point(0, 0);
            this.m_display.Name = "m_display";
            this.m_display.PlaySpeed = 0.5F;
            this.m_display.Size = new System.Drawing.Size(633, 333);
            this.m_display.SolidGridColor = System.Drawing.Color.DarkGray;
            this.m_display.TabIndex = 4;
            // 
            // GraphWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 333);
            this.ControlBox = false;
            this.Controls.Add(this.m_display);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GraphWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "GraphWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private GraphLib.PlotterDisplayEx m_display;
    }
}