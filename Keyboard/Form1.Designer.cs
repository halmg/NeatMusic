using System;
using System.Windows.Forms;

namespace Keyboard
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
            this.button1 = new System.Windows.Forms.Button();
            this.labelPitches = new System.Windows.Forms.Label();
            this.labelDurations = new System.Windows.Forms.Label();
            this.labelPitchesInfo = new System.Windows.Forms.Label();
            this.labelDurationsInfo = new System.Windows.Forms.Label();
            this.labelUpdate = new System.Windows.Forms.Label();
            this.labelUpdatesInfo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(99, 56);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 52);
            this.button1.TabIndex = 0;
            this.button1.Text = "Toggle Modules";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.button1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            // 
            // labelPitches
            // 
            this.labelPitches.AutoSize = true;
            this.labelPitches.Location = new System.Drawing.Point(12, 137);
            this.labelPitches.Name = "labelPitches";
            this.labelPitches.Size = new System.Drawing.Size(91, 13);
            this.labelPitches.TabIndex = 1;
            this.labelPitches.Text = "Recorded pitches";
            // 
            // labelDurations
            // 
            this.labelDurations.AutoSize = true;
            this.labelDurations.Location = new System.Drawing.Point(12, 176);
            this.labelDurations.Name = "labelDurations";
            this.labelDurations.Size = new System.Drawing.Size(100, 13);
            this.labelDurations.TabIndex = 2;
            this.labelDurations.Text = "Recorded durations";
            // 
            // labelPitchesInfo
            // 
            this.labelPitchesInfo.AutoSize = true;
            this.labelPitchesInfo.Location = new System.Drawing.Point(13, 154);
            this.labelPitchesInfo.Name = "labelPitchesInfo";
            this.labelPitchesInfo.Size = new System.Drawing.Size(81, 13);
            this.labelPitchesInfo.TabIndex = 3;
            this.labelPitchesInfo.Text = "waiting for input";
            // 
            // labelDurationsInfo
            // 
            this.labelDurationsInfo.AutoSize = true;
            this.labelDurationsInfo.Location = new System.Drawing.Point(12, 189);
            this.labelDurationsInfo.Name = "labelDurationsInfo";
            this.labelDurationsInfo.Size = new System.Drawing.Size(81, 13);
            this.labelDurationsInfo.TabIndex = 4;
            this.labelDurationsInfo.Text = "waiting for input";
            // 
            // labelUpdate
            // 
            this.labelUpdate.AutoSize = true;
            this.labelUpdate.Location = new System.Drawing.Point(12, 224);
            this.labelUpdate.Name = "labelUpdate";
            this.labelUpdate.Size = new System.Drawing.Size(55, 13);
            this.labelUpdate.TabIndex = 5;
            this.labelUpdate.Text = "Messages";
            // 
            // labelUpdatesInfo
            // 
            this.labelUpdatesInfo.AutoSize = true;
            this.labelUpdatesInfo.Enabled = false;
            this.labelUpdatesInfo.Location = new System.Drawing.Point(12, 240);
            this.labelUpdatesInfo.Name = "labelUpdatesInfo";
            this.labelUpdatesInfo.Size = new System.Drawing.Size(0, 13);
            this.labelUpdatesInfo.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(265, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "You have a piano octave on the keys \"q2w3er5t6y7u\"";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 386);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelUpdatesInfo);
            this.Controls.Add(this.labelUpdate);
            this.Controls.Add(this.labelDurationsInfo);
            this.Controls.Add(this.labelPitchesInfo);
            this.Controls.Add(this.labelDurations);
            this.Controls.Add(this.labelPitches);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button button1;
        private Label labelPitches;
        private Label labelDurations;
        private Label labelPitchesInfo;
        private Label labelDurationsInfo;
        private Label labelUpdate;
        private Label labelUpdatesInfo;
        private Label label1;
    }
}

