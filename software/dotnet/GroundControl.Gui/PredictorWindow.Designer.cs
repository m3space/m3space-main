namespace GroundControl.Gui
{
    partial class PredictorWindow
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
            this.numAscentRate = new System.Windows.Forms.NumericUpDown();
            this.datepickerLaunchTime = new System.Windows.Forms.DateTimePicker();
            this.numDescentRate = new System.Windows.Forms.NumericUpDown();
            this.numLaunchAltitude = new System.Windows.Forms.NumericUpDown();
            this.numBurstAltitude = new System.Windows.Forms.NumericUpDown();
            this.datepickerLaunchDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRunPrediction = new System.Windows.Forms.Button();
            this.lblTimeRemaining = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cboxPosition = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numAscentRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDescentRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLaunchAltitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBurstAltitude)).BeginInit();
            this.SuspendLayout();
            // 
            // numAscentRate
            // 
            this.numAscentRate.DecimalPlaces = 1;
            this.numAscentRate.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numAscentRate.Location = new System.Drawing.Point(112, 112);
            this.numAscentRate.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numAscentRate.Name = "numAscentRate";
            this.numAscentRate.Size = new System.Drawing.Size(91, 20);
            this.numAscentRate.TabIndex = 3;
            this.numAscentRate.Value = new decimal(new int[] {
            45,
            0,
            0,
            65536});
            // 
            // datepickerLaunchTime
            // 
            this.datepickerLaunchTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.datepickerLaunchTime.Location = new System.Drawing.Point(112, 59);
            this.datepickerLaunchTime.Name = "datepickerLaunchTime";
            this.datepickerLaunchTime.Size = new System.Drawing.Size(91, 20);
            this.datepickerLaunchTime.TabIndex = 1;
            // 
            // numDescentRate
            // 
            this.numDescentRate.DecimalPlaces = 1;
            this.numDescentRate.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numDescentRate.Location = new System.Drawing.Point(112, 138);
            this.numDescentRate.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numDescentRate.Name = "numDescentRate";
            this.numDescentRate.Size = new System.Drawing.Size(91, 20);
            this.numDescentRate.TabIndex = 4;
            this.numDescentRate.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // numLaunchAltitude
            // 
            this.numLaunchAltitude.Location = new System.Drawing.Point(112, 33);
            this.numLaunchAltitude.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numLaunchAltitude.Name = "numLaunchAltitude";
            this.numLaunchAltitude.Size = new System.Drawing.Size(91, 20);
            this.numLaunchAltitude.TabIndex = 0;
            this.numLaunchAltitude.Value = new decimal(new int[] {
            325,
            0,
            0,
            0});
            // 
            // numBurstAltitude
            // 
            this.numBurstAltitude.Location = new System.Drawing.Point(112, 164);
            this.numBurstAltitude.Maximum = new decimal(new int[] {
            40000,
            0,
            0,
            0});
            this.numBurstAltitude.Name = "numBurstAltitude";
            this.numBurstAltitude.Size = new System.Drawing.Size(91, 20);
            this.numBurstAltitude.TabIndex = 5;
            this.numBurstAltitude.ThousandsSeparator = true;
            this.numBurstAltitude.Value = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            // 
            // datepickerLaunchDate
            // 
            this.datepickerLaunchDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datepickerLaunchDate.Location = new System.Drawing.Point(112, 85);
            this.datepickerLaunchDate.Name = "datepickerLaunchDate";
            this.datepickerLaunchDate.Size = new System.Drawing.Size(91, 20);
            this.datepickerLaunchDate.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 166);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 115;
            this.label6.Text = "Burst Altitude [m]";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 113;
            this.label5.Text = "Descent Rate [m/s]";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 111;
            this.label4.Text = "Launch Date";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 110;
            this.label3.Text = "Ascent Rate [m/s]";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 107;
            this.label2.Text = "Launch Local Time";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 106;
            this.label1.Text = "Launch Altitude [m]";
            // 
            // btnRunPrediction
            // 
            this.btnRunPrediction.Location = new System.Drawing.Point(4, 195);
            this.btnRunPrediction.Name = "btnRunPrediction";
            this.btnRunPrediction.Size = new System.Drawing.Size(91, 38);
            this.btnRunPrediction.TabIndex = 10;
            this.btnRunPrediction.Text = "Run Prediction";
            this.btnRunPrediction.UseVisualStyleBackColor = true;
            this.btnRunPrediction.Click += new System.EventHandler(this.btnRunPrediction_Click);
            // 
            // lblTimeRemaining
            // 
            this.lblTimeRemaining.AutoEllipsis = true;
            this.lblTimeRemaining.Location = new System.Drawing.Point(101, 192);
            this.lblTimeRemaining.Name = "lblTimeRemaining";
            this.lblTimeRemaining.Size = new System.Drawing.Size(114, 53);
            this.lblTimeRemaining.TabIndex = 123;
            this.lblTimeRemaining.Text = "...";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 13);
            this.label7.TabIndex = 116;
            this.label7.Text = "Launch Position";
            // 
            // cboxPosition
            // 
            this.cboxPosition.FormattingEnabled = true;
            this.cboxPosition.Items.AddRange(new object[] {
            "Center of Map",
            "Ballon",
            "GroundControl"});
            this.cboxPosition.Location = new System.Drawing.Point(112, 6);
            this.cboxPosition.Name = "cboxPosition";
            this.cboxPosition.Size = new System.Drawing.Size(91, 21);
            this.cboxPosition.TabIndex = 117;
            this.cboxPosition.SelectedIndexChanged += new System.EventHandler(this.cboxPosition_SelectedIndexChanged);
            // 
            // PredictorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(216, 245);
            this.ControlBox = false;
            this.Controls.Add(this.cboxPosition);
            this.Controls.Add(this.lblTimeRemaining);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.numAscentRate);
            this.Controls.Add(this.datepickerLaunchTime);
            this.Controls.Add(this.btnRunPrediction);
            this.Controls.Add(this.numDescentRate);
            this.Controls.Add(this.numLaunchAltitude);
            this.Controls.Add(this.numBurstAltitude);
            this.Controls.Add(this.datepickerLaunchDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PredictorWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Predictor";
            ((System.ComponentModel.ISupportInitialize)(this.numAscentRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDescentRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLaunchAltitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBurstAltitude)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker datepickerLaunchTime;
        private System.Windows.Forms.NumericUpDown numDescentRate;
        private System.Windows.Forms.NumericUpDown numLaunchAltitude;
        private System.Windows.Forms.NumericUpDown numBurstAltitude;
        private System.Windows.Forms.DateTimePicker datepickerLaunchDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numAscentRate;
        private System.Windows.Forms.Button btnRunPrediction;
        private System.Windows.Forms.Label lblTimeRemaining;
        private System.Windows.Forms.ComboBox cboxPosition;
        private System.Windows.Forms.Label label7;
    }
}