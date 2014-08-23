namespace GroundControl.Gui
{
    partial class ExportTourDialog
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
            this.cancelBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.filenameLbl = new System.Windows.Forms.Label();
            this.fileNameTxt = new System.Windows.Forms.TextBox();
            this.selectBtn = new System.Windows.Forms.Button();
            this.minDeltaTxt = new System.Windows.Forms.TextBox();
            this.minDeltaLbl = new System.Windows.Forms.Label();
            this.speedLbl = new System.Windows.Forms.Label();
            this.speedTxt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(362, 107);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 4;
            this.cancelBtn.Text = "&Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okBtn.Location = new System.Drawing.Point(281, 107);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 3;
            this.okBtn.Text = "&OK";
            this.okBtn.UseVisualStyleBackColor = true;
            // 
            // filenameLbl
            // 
            this.filenameLbl.AutoSize = true;
            this.filenameLbl.Location = new System.Drawing.Point(12, 9);
            this.filenameLbl.Name = "filenameLbl";
            this.filenameLbl.Size = new System.Drawing.Size(54, 13);
            this.filenameLbl.TabIndex = 5;
            this.filenameLbl.Text = "File Name";
            // 
            // fileNameTxt
            // 
            this.fileNameTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileNameTxt.Location = new System.Drawing.Point(12, 25);
            this.fileNameTxt.Name = "fileNameTxt";
            this.fileNameTxt.Size = new System.Drawing.Size(338, 20);
            this.fileNameTxt.TabIndex = 6;
            // 
            // selectBtn
            // 
            this.selectBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectBtn.Location = new System.Drawing.Point(355, 23);
            this.selectBtn.Name = "selectBtn";
            this.selectBtn.Size = new System.Drawing.Size(75, 23);
            this.selectBtn.TabIndex = 7;
            this.selectBtn.Text = "Choose";
            this.selectBtn.UseVisualStyleBackColor = true;
            this.selectBtn.Click += new System.EventHandler(this.selectBtn_Click);
            // 
            // minDeltaTxt
            // 
            this.minDeltaTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.minDeltaTxt.Location = new System.Drawing.Point(87, 54);
            this.minDeltaTxt.Name = "minDeltaTxt";
            this.minDeltaTxt.Size = new System.Drawing.Size(62, 20);
            this.minDeltaTxt.TabIndex = 8;
            this.minDeltaTxt.Text = "10.0";
            // 
            // minDeltaLbl
            // 
            this.minDeltaLbl.AutoSize = true;
            this.minDeltaLbl.Location = new System.Drawing.Point(12, 57);
            this.minDeltaLbl.Name = "minDeltaLbl";
            this.minDeltaLbl.Size = new System.Drawing.Size(69, 13);
            this.minDeltaLbl.TabIndex = 9;
            this.minDeltaLbl.Text = "Min. time diff.";
            // 
            // speedLbl
            // 
            this.speedLbl.AutoSize = true;
            this.speedLbl.Location = new System.Drawing.Point(182, 57);
            this.speedLbl.Name = "speedLbl";
            this.speedLbl.Size = new System.Drawing.Size(38, 13);
            this.speedLbl.TabIndex = 11;
            this.speedLbl.Text = "Speed";
            // 
            // speedTxt
            // 
            this.speedTxt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.speedTxt.Location = new System.Drawing.Point(226, 54);
            this.speedTxt.Name = "speedTxt";
            this.speedTxt.Size = new System.Drawing.Size(62, 20);
            this.speedTxt.TabIndex = 10;
            this.speedTxt.Text = "1.0";
            // 
            // ExportTourDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 133);
            this.Controls.Add(this.speedLbl);
            this.Controls.Add(this.speedTxt);
            this.Controls.Add(this.minDeltaLbl);
            this.Controls.Add(this.minDeltaTxt);
            this.Controls.Add(this.selectBtn);
            this.Controls.Add(this.fileNameTxt);
            this.Controls.Add(this.filenameLbl);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ExportTourDialog";
            this.Text = "Export Google Earth Tour";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Label filenameLbl;
        private System.Windows.Forms.TextBox fileNameTxt;
        private System.Windows.Forms.Button selectBtn;
        private System.Windows.Forms.TextBox minDeltaTxt;
        private System.Windows.Forms.Label minDeltaLbl;
        private System.Windows.Forms.Label speedLbl;
        private System.Windows.Forms.TextBox speedTxt;
    }
}