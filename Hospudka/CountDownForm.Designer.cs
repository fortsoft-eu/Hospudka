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
 * Version 1.0.0.0
 */

namespace Hospudka {
    partial class CountDownForm {
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
            this.labelInfo = new System.Windows.Forms.Label();
            this.labelSec = new System.Windows.Forms.Label();
            this.buttonImmediate = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelInfo
            // 
            this.labelInfo.AutoEllipsis = true;
            this.labelInfo.Location = new System.Drawing.Point(12, 9);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(320, 27);
            this.labelInfo.TabIndex = 0;
            this.labelInfo.Text = "The monitors will turn off after the specified number of seconds. To turn it back" +
    " on, press any key or move the mouse.";
            this.labelInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelSec
            // 
            this.labelSec.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelSec.Location = new System.Drawing.Point(157, 43);
            this.labelSec.Name = "labelSec";
            this.labelSec.Size = new System.Drawing.Size(30, 23);
            this.labelSec.TabIndex = 1;
            this.labelSec.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonImmediate
            // 
            this.buttonImmediate.Location = new System.Drawing.Point(42, 73);
            this.buttonImmediate.Name = "buttonImmediate";
            this.buttonImmediate.Size = new System.Drawing.Size(90, 23);
            this.buttonImmediate.TabIndex = 2;
            this.buttonImmediate.Text = "Tur&n off Now";
            this.buttonImmediate.UseVisualStyleBackColor = true;
            this.buttonImmediate.Click += new System.EventHandler(this.OnClick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(212, 73);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(90, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Canc&el";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // CountDownForm
            // 
            this.AcceptButton = this.buttonImmediate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(344, 108);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonImmediate);
            this.Controls.Add(this.labelSec);
            this.Controls.Add(this.labelInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CountDownForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Turn off the Monitors";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Label labelSec;
        private System.Windows.Forms.Button buttonImmediate;
        private System.Windows.Forms.Button buttonCancel;
    }
}