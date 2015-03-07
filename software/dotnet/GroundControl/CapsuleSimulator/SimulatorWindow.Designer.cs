namespace CapsuleSimulator
{
    partial class SimulatorWindow
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
            this.btnSelectTelemetryFile = new System.Windows.Forms.Button();
            this.tboxTelemetryFile = new System.Windows.Forms.TextBox();
            this.tboxComPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tboxLogger = new System.Windows.Forms.TextBox();
            this.numTelemetryInterval = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.m_timer = new System.Windows.Forms.Timer(this.components);
            this.btnStop = new System.Windows.Forms.Button();
            this.numericStep = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numTelemetryInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStep)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectTelemetryFile
            // 
            this.btnSelectTelemetryFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectTelemetryFile.Location = new System.Drawing.Point(305, 4);
            this.btnSelectTelemetryFile.Name = "btnSelectTelemetryFile";
            this.btnSelectTelemetryFile.Size = new System.Drawing.Size(75, 23);
            this.btnSelectTelemetryFile.TabIndex = 13;
            this.btnSelectTelemetryFile.Text = "Choose";
            this.btnSelectTelemetryFile.UseVisualStyleBackColor = true;
            this.btnSelectTelemetryFile.Click += new System.EventHandler(this.btnSelectTelemetryFile_Click);
            // 
            // tboxTelemetryFile
            // 
            this.tboxTelemetryFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tboxTelemetryFile.Location = new System.Drawing.Point(80, 6);
            this.tboxTelemetryFile.Name = "tboxTelemetryFile";
            this.tboxTelemetryFile.Size = new System.Drawing.Size(219, 20);
            this.tboxTelemetryFile.TabIndex = 9;
            // 
            // tboxComPort
            // 
            this.tboxComPort.Location = new System.Drawing.Point(80, 32);
            this.tboxComPort.Name = "tboxComPort";
            this.tboxComPort.Size = new System.Drawing.Size(74, 20);
            this.tboxComPort.TabIndex = 11;
            this.tboxComPort.Text = "COM1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "COM Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "File";
            // 
            // tboxLogger
            // 
            this.tboxLogger.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tboxLogger.BackColor = System.Drawing.Color.White;
            this.tboxLogger.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tboxLogger.ForeColor = System.Drawing.Color.Black;
            this.tboxLogger.Location = new System.Drawing.Point(9, 90);
            this.tboxLogger.Margin = new System.Windows.Forms.Padding(0);
            this.tboxLogger.Multiline = true;
            this.tboxLogger.Name = "tboxLogger";
            this.tboxLogger.ReadOnly = true;
            this.tboxLogger.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tboxLogger.Size = new System.Drawing.Size(372, 72);
            this.tboxLogger.TabIndex = 14;
            // 
            // numTelemetryInterval
            // 
            this.numTelemetryInterval.Location = new System.Drawing.Point(80, 58);
            this.numTelemetryInterval.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numTelemetryInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTelemetryInterval.Name = "numTelemetryInterval";
            this.numTelemetryInterval.Size = new System.Drawing.Size(45, 20);
            this.numTelemetryInterval.TabIndex = 15;
            this.numTelemetryInterval.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numTelemetryInterval.ValueChanged += new System.EventHandler(this.numTelemetryInterval_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Send every";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(131, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "s";
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(224, 33);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 43);
            this.btnStart.TabIndex = 18;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // m_timer
            // 
            this.m_timer.Interval = 5000;
            this.m_timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Location = new System.Drawing.Point(305, 33);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 43);
            this.btnStop.TabIndex = 19;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // numericStep
            // 
            this.numericStep.Location = new System.Drawing.Point(149, 58);
            this.numericStep.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericStep.Name = "numericStep";
            this.numericStep.Size = new System.Drawing.Size(37, 20);
            this.numericStep.TabIndex = 20;
            this.numericStep.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(192, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "step";
            // 
            // SimulatorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 171);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericStep);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numTelemetryInterval);
            this.Controls.Add(this.tboxLogger);
            this.Controls.Add(this.btnSelectTelemetryFile);
            this.Controls.Add(this.tboxTelemetryFile);
            this.Controls.Add(this.tboxComPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(406, 209);
            this.Name = "SimulatorWindow";
            this.Text = "Capsule Simulator";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SimulatorWindow_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.numTelemetryInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStep)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectTelemetryFile;
        private System.Windows.Forms.TextBox tboxTelemetryFile;
        private System.Windows.Forms.TextBox tboxComPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tboxLogger;
        private System.Windows.Forms.NumericUpDown numTelemetryInterval;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Timer m_timer;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.NumericUpDown numericStep;
        private System.Windows.Forms.Label label5;
    }
}

