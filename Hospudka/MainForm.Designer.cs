/**
 * This is open-source software licensed under the terms of the MIT License.
 *
 * Copyright (c) 2023 Petr Červinka - FortSoft <cervinka@fortsoft.eu>
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 **
 * Version 1.1.0.0
 */

namespace Hospudka {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonDeploy = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.fileSystemWatcher = new System.IO.FileSystemWatcher();
            this.buttonConfig = new System.Windows.Forms.Button();
            this.buttonAbout = new System.Windows.Forms.Button();
            this.labelProgress = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.checkBoxChime = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonDeploy
            // 
            this.buttonDeploy.Location = new System.Drawing.Point(12, 125);
            this.buttonDeploy.Name = "buttonDeploy";
            this.buttonDeploy.Size = new System.Drawing.Size(75, 23);
            this.buttonDeploy.TabIndex = 0;
            this.buttonDeploy.Text = "&Deploy";
            this.buttonDeploy.UseVisualStyleBackColor = true;
            this.buttonDeploy.Click += new System.EventHandler(this.OnButtonDeployClick);
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(12, 3);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(294, 115);
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 173);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(294, 23);
            this.progressBar.TabIndex = 5;
            // 
            // fileSystemWatcher
            // 
            this.fileSystemWatcher.EnableRaisingEvents = true;
            this.fileSystemWatcher.SynchronizingObject = this;
            // 
            // buttonConfig
            // 
            this.buttonConfig.Location = new System.Drawing.Point(122, 125);
            this.buttonConfig.Name = "buttonConfig";
            this.buttonConfig.Size = new System.Drawing.Size(75, 23);
            this.buttonConfig.TabIndex = 1;
            this.buttonConfig.Text = "&Config...";
            this.buttonConfig.UseVisualStyleBackColor = true;
            this.buttonConfig.Click += new System.EventHandler(this.EditRemoteConfig);
            // 
            // buttonAbout
            // 
            this.buttonAbout.Location = new System.Drawing.Point(231, 125);
            this.buttonAbout.Name = "buttonAbout";
            this.buttonAbout.Size = new System.Drawing.Size(75, 23);
            this.buttonAbout.TabIndex = 2;
            this.buttonAbout.Text = "Ab&out...";
            this.buttonAbout.UseVisualStyleBackColor = true;
            this.buttonAbout.Click += new System.EventHandler(this.ShowAbout);
            // 
            // labelProgress
            // 
            this.labelProgress.Location = new System.Drawing.Point(209, 153);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(100, 23);
            this.labelProgress.TabIndex = 4;
            this.labelProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // statusStrip
            // 
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip.Location = new System.Drawing.Point(0, 198);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(318, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 6;
            this.statusStrip.Text = "statusStrip";
            // 
            // checkBoxChime
            // 
            this.checkBoxChime.AutoSize = true;
            this.checkBoxChime.Location = new System.Drawing.Point(12, 153);
            this.checkBoxChime.Name = "checkBoxChime";
            this.checkBoxChime.Size = new System.Drawing.Size(111, 17);
            this.checkBoxChime.TabIndex = 3;
            this.checkBoxChime.Text = "Chi&me when done";
            this.checkBoxChime.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(318, 220);
            this.Controls.Add(this.checkBoxChime);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonAbout);
            this.Controls.Add(this.buttonConfig);
            this.Controls.Add(this.buttonDeploy);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.labelProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Activated += new System.EventHandler(this.OnFormActivated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.OnFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonDeploy;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.IO.FileSystemWatcher fileSystemWatcher;
        private System.Windows.Forms.Button buttonAbout;
        private System.Windows.Forms.Button buttonConfig;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.CheckBox checkBoxChime;
    }
}

