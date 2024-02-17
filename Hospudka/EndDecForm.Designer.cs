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
 * Version 1.1.1.3
 */

namespace Hospudka {
    partial class EncDecForm {
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
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.panel = new System.Windows.Forms.Panel();
            this.groupBoxConvert = new System.Windows.Forms.GroupBox();
            this.buttonAlphanumeric = new System.Windows.Forms.Button();
            this.buttonSafeFileName = new System.Windows.Forms.Button();
            this.buttonSafePath = new System.Windows.Forms.Button();
            this.buttonFull = new System.Windows.Forms.Button();
            this.groupBoxEncode = new System.Windows.Forms.GroupBox();
            this.buttonEncodeBase64 = new System.Windows.Forms.Button();
            this.buttonRFC3986 = new System.Windows.Forms.Button();
            this.buttonRFC1738 = new System.Windows.Forms.Button();
            this.groupBoxDecode = new System.Windows.Forms.GroupBox();
            this.buttonParseUrl = new System.Windows.Forms.Button();
            this.buttonDecodeUrl = new System.Windows.Forms.Button();
            this.buttonDecodeBase64 = new System.Windows.Forms.Button();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.buttonClearOutput = new System.Windows.Forms.Button();
            this.buttonFlip = new System.Windows.Forms.Button();
            this.buttonClearInput = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panel.SuspendLayout();
            this.groupBoxConvert.SuspendLayout();
            this.groupBoxEncode.SuspendLayout();
            this.groupBoxDecode.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 239);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(774, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.textBoxInput);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.panel);
            this.splitContainer.Panel2.Controls.Add(this.textBoxOutput);
            this.splitContainer.Panel2.Controls.Add(this.buttonClearOutput);
            this.splitContainer.Panel2.Controls.Add(this.buttonFlip);
            this.splitContainer.Panel2.Controls.Add(this.buttonClearInput);
            this.splitContainer.Size = new System.Drawing.Size(774, 239);
            this.splitContainer.SplitterDistance = 75;
            this.splitContainer.TabIndex = 0;
            // 
            // textBoxInput
            // 
            this.textBoxInput.AcceptsReturn = true;
            this.textBoxInput.AcceptsTab = true;
            this.textBoxInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxInput.Location = new System.Drawing.Point(0, 0);
            this.textBoxInput.Multiline = true;
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxInput.Size = new System.Drawing.Size(774, 75);
            this.textBoxInput.TabIndex = 0;
            this.textBoxInput.TextChanged += new System.EventHandler(this.OnInputTextChanged);
            this.textBoxInput.Enter += new System.EventHandler(this.OnEnter);
            this.textBoxInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.textBoxInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this.textBoxInput.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // panel
            // 
            this.panel.Controls.Add(this.groupBoxConvert);
            this.panel.Controls.Add(this.groupBoxEncode);
            this.panel.Controls.Add(this.groupBoxDecode);
            this.panel.Location = new System.Drawing.Point(3, 3);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(679, 77);
            this.panel.TabIndex = 0;
            // 
            // groupBoxConvert
            // 
            this.groupBoxConvert.Controls.Add(this.buttonAlphanumeric);
            this.groupBoxConvert.Controls.Add(this.buttonSafeFileName);
            this.groupBoxConvert.Controls.Add(this.buttonSafePath);
            this.groupBoxConvert.Controls.Add(this.buttonFull);
            this.groupBoxConvert.Location = new System.Drawing.Point(456, 0);
            this.groupBoxConvert.Name = "groupBoxConvert";
            this.groupBoxConvert.Size = new System.Drawing.Size(222, 77);
            this.groupBoxConvert.TabIndex = 2;
            this.groupBoxConvert.TabStop = false;
            this.groupBoxConvert.Text = "Convert to ASCII";
            // 
            // buttonAlphanumeric
            // 
            this.buttonAlphanumeric.Location = new System.Drawing.Point(114, 48);
            this.buttonAlphanumeric.Name = "buttonAlphanumeric";
            this.buttonAlphanumeric.Size = new System.Drawing.Size(102, 23);
            this.buttonAlphanumeric.TabIndex = 3;
            this.buttonAlphanumeric.Text = "Alphanu&meric";
            this.buttonAlphanumeric.UseVisualStyleBackColor = true;
            this.buttonAlphanumeric.Click += new System.EventHandler(this.ConvertToASCIIAlphanumeric);
            // 
            // buttonSafeFileName
            // 
            this.buttonSafeFileName.Location = new System.Drawing.Point(114, 19);
            this.buttonSafeFileName.Name = "buttonSafeFileName";
            this.buttonSafeFileName.Size = new System.Drawing.Size(102, 23);
            this.buttonSafeFileName.TabIndex = 2;
            this.buttonSafeFileName.Text = "&Safe File Name";
            this.buttonSafeFileName.UseVisualStyleBackColor = true;
            this.buttonSafeFileName.Click += new System.EventHandler(this.ConvertToASCIISafeFileName);
            // 
            // buttonSafePath
            // 
            this.buttonSafePath.Location = new System.Drawing.Point(6, 48);
            this.buttonSafePath.Name = "buttonSafePath";
            this.buttonSafePath.Size = new System.Drawing.Size(102, 23);
            this.buttonSafePath.TabIndex = 1;
            this.buttonSafePath.Text = "Safe Pa&th";
            this.buttonSafePath.UseVisualStyleBackColor = true;
            this.buttonSafePath.Click += new System.EventHandler(this.ConvertToASCIISafePath);
            // 
            // buttonFull
            // 
            this.buttonFull.Location = new System.Drawing.Point(6, 19);
            this.buttonFull.Name = "buttonFull";
            this.buttonFull.Size = new System.Drawing.Size(102, 23);
            this.buttonFull.TabIndex = 0;
            this.buttonFull.Text = "To All Pri&ntable";
            this.buttonFull.UseVisualStyleBackColor = true;
            this.buttonFull.Click += new System.EventHandler(this.ConvertToASCIIFull);
            // 
            // groupBoxEncode
            // 
            this.groupBoxEncode.Controls.Add(this.buttonEncodeBase64);
            this.groupBoxEncode.Controls.Add(this.buttonRFC3986);
            this.groupBoxEncode.Controls.Add(this.buttonRFC1738);
            this.groupBoxEncode.Location = new System.Drawing.Point(0, 0);
            this.groupBoxEncode.Name = "groupBoxEncode";
            this.groupBoxEncode.Size = new System.Drawing.Size(222, 77);
            this.groupBoxEncode.TabIndex = 0;
            this.groupBoxEncode.TabStop = false;
            this.groupBoxEncode.Text = "Encode";
            // 
            // buttonEncodeBase64
            // 
            this.buttonEncodeBase64.Location = new System.Drawing.Point(114, 19);
            this.buttonEncodeBase64.Name = "buttonEncodeBase64";
            this.buttonEncodeBase64.Size = new System.Drawing.Size(102, 23);
            this.buttonEncodeBase64.TabIndex = 2;
            this.buttonEncodeBase64.Text = "UTF-8 to Base&64";
            this.buttonEncodeBase64.UseVisualStyleBackColor = true;
            this.buttonEncodeBase64.Click += new System.EventHandler(this.EncodeBase64);
            this.buttonEncodeBase64.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            // 
            // buttonRFC3986
            // 
            this.buttonRFC3986.Location = new System.Drawing.Point(6, 48);
            this.buttonRFC3986.Name = "buttonRFC3986";
            this.buttonRFC3986.Size = new System.Drawing.Size(102, 23);
            this.buttonRFC3986.TabIndex = 1;
            this.buttonRFC3986.Text = "URL (RFC &3986)";
            this.buttonRFC3986.UseVisualStyleBackColor = true;
            this.buttonRFC3986.Click += new System.EventHandler(this.EncodeRFC3986);
            this.buttonRFC3986.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            // 
            // buttonRFC1738
            // 
            this.buttonRFC1738.Location = new System.Drawing.Point(6, 19);
            this.buttonRFC1738.Name = "buttonRFC1738";
            this.buttonRFC1738.Size = new System.Drawing.Size(102, 23);
            this.buttonRFC1738.TabIndex = 0;
            this.buttonRFC1738.Text = "URL (RFC &1738)";
            this.buttonRFC1738.UseVisualStyleBackColor = true;
            this.buttonRFC1738.Click += new System.EventHandler(this.EncodeRFC1738);
            this.buttonRFC1738.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            // 
            // groupBoxDecode
            // 
            this.groupBoxDecode.Controls.Add(this.buttonParseUrl);
            this.groupBoxDecode.Controls.Add(this.buttonDecodeUrl);
            this.groupBoxDecode.Controls.Add(this.buttonDecodeBase64);
            this.groupBoxDecode.Location = new System.Drawing.Point(228, 0);
            this.groupBoxDecode.Name = "groupBoxDecode";
            this.groupBoxDecode.Size = new System.Drawing.Size(222, 77);
            this.groupBoxDecode.TabIndex = 1;
            this.groupBoxDecode.TabStop = false;
            this.groupBoxDecode.Text = "Decode / Parse";
            // 
            // buttonParseUrl
            // 
            this.buttonParseUrl.Location = new System.Drawing.Point(6, 19);
            this.buttonParseUrl.Name = "buttonParseUrl";
            this.buttonParseUrl.Size = new System.Drawing.Size(102, 23);
            this.buttonParseUrl.TabIndex = 0;
            this.buttonParseUrl.Text = "&Parse URL";
            this.buttonParseUrl.UseVisualStyleBackColor = true;
            this.buttonParseUrl.Click += new System.EventHandler(this.ParseUrl);
            this.buttonParseUrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            // 
            // buttonDecodeUrl
            // 
            this.buttonDecodeUrl.Location = new System.Drawing.Point(6, 48);
            this.buttonDecodeUrl.Name = "buttonDecodeUrl";
            this.buttonDecodeUrl.Size = new System.Drawing.Size(102, 23);
            this.buttonDecodeUrl.TabIndex = 1;
            this.buttonDecodeUrl.Text = "&Decode URL";
            this.buttonDecodeUrl.UseVisualStyleBackColor = true;
            this.buttonDecodeUrl.Click += new System.EventHandler(this.DecodeUrl);
            this.buttonDecodeUrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            // 
            // buttonDecodeBase64
            // 
            this.buttonDecodeBase64.Location = new System.Drawing.Point(114, 19);
            this.buttonDecodeBase64.Name = "buttonDecodeBase64";
            this.buttonDecodeBase64.Size = new System.Drawing.Size(102, 23);
            this.buttonDecodeBase64.TabIndex = 2;
            this.buttonDecodeBase64.Text = "&Base64 to UTF-8";
            this.buttonDecodeBase64.UseVisualStyleBackColor = true;
            this.buttonDecodeBase64.Click += new System.EventHandler(this.DecodeBase64);
            this.buttonDecodeBase64.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.AcceptsReturn = true;
            this.textBoxOutput.AcceptsTab = true;
            this.textBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOutput.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxOutput.Location = new System.Drawing.Point(0, 85);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ReadOnly = true;
            this.textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxOutput.Size = new System.Drawing.Size(774, 75);
            this.textBoxOutput.TabIndex = 4;
            this.textBoxOutput.TextChanged += new System.EventHandler(this.OnOutputTextChanged);
            this.textBoxOutput.Enter += new System.EventHandler(this.OnEnter);
            this.textBoxOutput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.textBoxOutput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this.textBoxOutput.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // buttonClearOutput
            // 
            this.buttonClearOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearOutput.Location = new System.Drawing.Point(686, 57);
            this.buttonClearOutput.Name = "buttonClearOutput";
            this.buttonClearOutput.Size = new System.Drawing.Size(85, 23);
            this.buttonClearOutput.TabIndex = 3;
            this.buttonClearOutput.Text = "Cle&ar Output";
            this.buttonClearOutput.UseVisualStyleBackColor = true;
            this.buttonClearOutput.Click += new System.EventHandler(this.ClearOutput);
            this.buttonClearOutput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            // 
            // buttonFlip
            // 
            this.buttonFlip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFlip.Location = new System.Drawing.Point(686, 30);
            this.buttonFlip.Name = "buttonFlip";
            this.buttonFlip.Size = new System.Drawing.Size(85, 23);
            this.buttonFlip.TabIndex = 2;
            this.buttonFlip.Text = "F&lip ↕";
            this.buttonFlip.UseVisualStyleBackColor = true;
            this.buttonFlip.Click += new System.EventHandler(this.Flip);
            this.buttonFlip.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            // 
            // buttonClearInput
            // 
            this.buttonClearInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearInput.Location = new System.Drawing.Point(686, 3);
            this.buttonClearInput.Name = "buttonClearInput";
            this.buttonClearInput.Size = new System.Drawing.Size(85, 23);
            this.buttonClearInput.TabIndex = 1;
            this.buttonClearInput.Text = "Clea&r Input";
            this.buttonClearInput.UseVisualStyleBackColor = true;
            this.buttonClearInput.Click += new System.EventHandler(this.ClearInput);
            this.buttonClearInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            // 
            // EncDecForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 261);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.statusStrip);
            this.Name = "EncDecForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Activated += new System.EventHandler(this.OnFormActivated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.OpenHelp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panel.ResumeLayout(false);
            this.groupBoxConvert.ResumeLayout(false);
            this.groupBoxEncode.ResumeLayout(false);
            this.groupBoxDecode.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TextBox textBoxInput;
        private System.Windows.Forms.Button buttonDecodeBase64;
        private System.Windows.Forms.Button buttonDecodeUrl;
        private System.Windows.Forms.Button buttonParseUrl;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.GroupBox groupBoxEncode;
        private System.Windows.Forms.GroupBox groupBoxDecode;
        private System.Windows.Forms.Button buttonRFC3986;
        private System.Windows.Forms.Button buttonRFC1738;
        private System.Windows.Forms.Button buttonEncodeBase64;
        private System.Windows.Forms.Button buttonClearOutput;
        private System.Windows.Forms.Button buttonClearInput;
        private System.Windows.Forms.Button buttonFlip;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.GroupBox groupBoxConvert;
        private System.Windows.Forms.Button buttonAlphanumeric;
        private System.Windows.Forms.Button buttonSafeFileName;
        private System.Windows.Forms.Button buttonSafePath;
        private System.Windows.Forms.Button buttonFull;
    }
}