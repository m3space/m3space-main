﻿namespace GroundControl.Gui
{
    partial class PreferencesDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comPortRadioBox = new System.Windows.Forms.TextBox();
            this.dataDirBox = new System.Windows.Forms.TextBox();
            this.dataDirBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.webAccessBox = new System.Windows.Forms.TextBox();
            this.webAccessCheck = new System.Windows.Forms.CheckBox();
            this.comPortGPSBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(306, 123);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 7;
            this.cancelBtn.Text = "&Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Data Directory";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "COM Port Radio";
            // 
            // comPortRadioBox
            // 
            this.comPortRadioBox.Location = new System.Drawing.Point(95, 33);
            this.comPortRadioBox.Name = "comPortRadioBox";
            this.comPortRadioBox.Size = new System.Drawing.Size(74, 20);
            this.comPortRadioBox.TabIndex = 2;
            // 
            // dataDirBox
            // 
            this.dataDirBox.Location = new System.Drawing.Point(95, 6);
            this.dataDirBox.Name = "dataDirBox";
            this.dataDirBox.Size = new System.Drawing.Size(207, 20);
            this.dataDirBox.TabIndex = 1;
            // 
            // dataDirBtn
            // 
            this.dataDirBtn.Location = new System.Drawing.Point(308, 4);
            this.dataDirBtn.Name = "dataDirBtn";
            this.dataDirBtn.Size = new System.Drawing.Size(75, 23);
            this.dataDirBtn.TabIndex = 8;
            this.dataDirBtn.Text = "Choose";
            this.dataDirBtn.UseVisualStyleBackColor = true;
            this.dataDirBtn.Click += new System.EventHandler(this.dataDirBtn_Click);
            // 
            // okBtn
            // 
            this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okBtn.Location = new System.Drawing.Point(225, 123);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 6;
            this.okBtn.Text = "&OK";
            this.okBtn.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 100;
            this.label3.Text = "Web access";
            // 
            // webAccessBox
            // 
            this.webAccessBox.Location = new System.Drawing.Point(95, 85);
            this.webAccessBox.Name = "webAccessBox";
            this.webAccessBox.Size = new System.Drawing.Size(207, 20);
            this.webAccessBox.TabIndex = 4;
            // 
            // webAccessCheck
            // 
            this.webAccessCheck.AutoSize = true;
            this.webAccessCheck.Location = new System.Drawing.Point(308, 87);
            this.webAccessCheck.Name = "webAccessCheck";
            this.webAccessCheck.Size = new System.Drawing.Size(65, 17);
            this.webAccessCheck.TabIndex = 5;
            this.webAccessCheck.Text = "Enabled";
            this.webAccessCheck.UseVisualStyleBackColor = true;
            // 
            // comPortGPSBox
            // 
            this.comPortGPSBox.Location = new System.Drawing.Point(95, 59);
            this.comPortGPSBox.Name = "comPortGPSBox";
            this.comPortGPSBox.Size = new System.Drawing.Size(74, 20);
            this.comPortGPSBox.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 103;
            this.label4.Text = "COM Port GPS";
            // 
            // PreferencesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 155);
            this.Controls.Add(this.comPortGPSBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.webAccessCheck);
            this.Controls.Add(this.webAccessBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.dataDirBtn);
            this.Controls.Add(this.dataDirBox);
            this.Controls.Add(this.comPortRadioBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "PreferencesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
            this.Load += new System.EventHandler(this.PreferencesDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox comPortRadioBox;
        private System.Windows.Forms.TextBox dataDirBox;
        private System.Windows.Forms.Button dataDirBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox webAccessBox;
        private System.Windows.Forms.CheckBox webAccessCheck;
        private System.Windows.Forms.TextBox comPortGPSBox;
        private System.Windows.Forms.Label label4;
    }
}