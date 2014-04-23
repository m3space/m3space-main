namespace TelemetryAnalyzer
{
    partial class ImportMissionDialog
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
            this.tboxTelemetryFile = new System.Windows.Forms.TextBox();
            this.btnSelectTelemetryFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelectImagePath = new System.Windows.Forms.Button();
            this.tboxImagePath = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnOk = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSelectVideoPath = new System.Windows.Forms.Button();
            this.tboxVideoPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tboxMissionName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tboxTelemetryFile
            // 
            this.tboxTelemetryFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tboxTelemetryFile.Location = new System.Drawing.Point(102, 47);
            this.tboxTelemetryFile.Margin = new System.Windows.Forms.Padding(2);
            this.tboxTelemetryFile.Name = "tboxTelemetryFile";
            this.tboxTelemetryFile.Size = new System.Drawing.Size(254, 20);
            this.tboxTelemetryFile.TabIndex = 1;
            // 
            // btnSelectTelemetryFile
            // 
            this.btnSelectTelemetryFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectTelemetryFile.Location = new System.Drawing.Point(360, 45);
            this.btnSelectTelemetryFile.Margin = new System.Windows.Forms.Padding(2);
            this.btnSelectTelemetryFile.Name = "btnSelectTelemetryFile";
            this.btnSelectTelemetryFile.Size = new System.Drawing.Size(28, 23);
            this.btnSelectTelemetryFile.TabIndex = 4;
            this.btnSelectTelemetryFile.Text = "...";
            this.btnSelectTelemetryFile.UseVisualStyleBackColor = true;
            this.btnSelectTelemetryFile.Click += new System.EventHandler(this.btnSelectTelemetryFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 50);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Telemetry file:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 77);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Images path:";
            // 
            // btnSelectImagePath
            // 
            this.btnSelectImagePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectImagePath.Location = new System.Drawing.Point(360, 72);
            this.btnSelectImagePath.Margin = new System.Windows.Forms.Padding(2);
            this.btnSelectImagePath.Name = "btnSelectImagePath";
            this.btnSelectImagePath.Size = new System.Drawing.Size(28, 23);
            this.btnSelectImagePath.TabIndex = 5;
            this.btnSelectImagePath.Text = "...";
            this.btnSelectImagePath.UseVisualStyleBackColor = true;
            this.btnSelectImagePath.Click += new System.EventHandler(this.btnSelectImagePath_Click);
            // 
            // tboxImagePath
            // 
            this.tboxImagePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tboxImagePath.Location = new System.Drawing.Point(102, 74);
            this.tboxImagePath.Margin = new System.Windows.Forms.Padding(2);
            this.tboxImagePath.Name = "tboxImagePath";
            this.tboxImagePath.Size = new System.Drawing.Size(254, 20);
            this.tboxImagePath.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(324, 144);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(64, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(256, 144);
            this.btnOk.Margin = new System.Windows.Forms.Padding(2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(64, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 104);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Video path:";
            // 
            // btnSelectVideoPath
            // 
            this.btnSelectVideoPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectVideoPath.Location = new System.Drawing.Point(360, 99);
            this.btnSelectVideoPath.Margin = new System.Windows.Forms.Padding(2);
            this.btnSelectVideoPath.Name = "btnSelectVideoPath";
            this.btnSelectVideoPath.Size = new System.Drawing.Size(28, 23);
            this.btnSelectVideoPath.TabIndex = 6;
            this.btnSelectVideoPath.Text = "...";
            this.btnSelectVideoPath.UseVisualStyleBackColor = true;
            this.btnSelectVideoPath.Click += new System.EventHandler(this.btnSelectVideoPath_Click);
            // 
            // tboxVideoPath
            // 
            this.tboxVideoPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tboxVideoPath.Location = new System.Drawing.Point(102, 101);
            this.tboxVideoPath.Margin = new System.Windows.Forms.Padding(2);
            this.tboxVideoPath.Name = "tboxVideoPath";
            this.tboxVideoPath.Size = new System.Drawing.Size(254, 20);
            this.tboxVideoPath.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 22);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Mission name:";
            // 
            // tboxMissionName
            // 
            this.tboxMissionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tboxMissionName.Location = new System.Drawing.Point(102, 19);
            this.tboxMissionName.Margin = new System.Windows.Forms.Padding(2);
            this.tboxMissionName.Name = "tboxMissionName";
            this.tboxMissionName.Size = new System.Drawing.Size(254, 20);
            this.tboxMissionName.TabIndex = 0;
            // 
            // ImportMissionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(399, 178);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tboxMissionName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSelectVideoPath);
            this.Controls.Add(this.tboxVideoPath);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSelectImagePath);
            this.Controls.Add(this.tboxImagePath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSelectTelemetryFile);
            this.Controls.Add(this.tboxTelemetryFile);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ImportMissionDialog";
            this.Text = "Import Mission";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tboxTelemetryFile;
        private System.Windows.Forms.Button btnSelectTelemetryFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSelectImagePath;
        private System.Windows.Forms.TextBox tboxImagePath;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSelectVideoPath;
        private System.Windows.Forms.TextBox tboxVideoPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tboxMissionName;
    }
}