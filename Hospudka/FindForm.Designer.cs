/**
 * This is open-source software licensed under the terms of the MIT License.
 *
 * Copyright (c) 2023-2024 Petr Červinka - FortSoft <cervinka@fortsoft.eu>
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
    partial class FindForm {
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
            this.labelFind = new System.Windows.Forms.Label();
            this.comboBoxFind = new System.Windows.Forms.ComboBox();
            this.buttonFind = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.checkBoxCaseSensitive = new System.Windows.Forms.CheckBox();
            this.checkBoxBackward = new System.Windows.Forms.CheckBox();
            this.checkBoxTopMost = new System.Windows.Forms.CheckBox();
            this.checkBoxStartsWith = new System.Windows.Forms.CheckBox();
            this.checkBoxEndsWith = new System.Windows.Forms.CheckBox();
            this.checkBoxRegEx = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // labelFind
            // 
            this.labelFind.AutoSize = true;
            this.labelFind.Location = new System.Drawing.Point(12, 16);
            this.labelFind.Name = "labelFind";
            this.labelFind.Size = new System.Drawing.Size(30, 13);
            this.labelFind.TabIndex = 0;
            this.labelFind.Text = "&Find:";
            // 
            // comboBoxFind
            // 
            this.comboBoxFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxFind.FormattingEnabled = true;
            this.comboBoxFind.Location = new System.Drawing.Point(77, 13);
            this.comboBoxFind.Name = "comboBoxFind";
            this.comboBoxFind.Size = new System.Drawing.Size(194, 21);
            this.comboBoxFind.TabIndex = 1;
            this.comboBoxFind.DropDown += new System.EventHandler(this.OnDropDown);
            this.comboBoxFind.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
            this.comboBoxFind.SelectionChangeCommitted += new System.EventHandler(this.OnCommitted);
            this.comboBoxFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.comboBoxFind.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            this.comboBoxFind.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            // 
            // buttonFind
            // 
            this.buttonFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFind.Location = new System.Drawing.Point(277, 12);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(75, 23);
            this.buttonFind.TabIndex = 8;
            this.buttonFind.Text = "Fi&nd next";
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.OnFindButtonClick);
            this.buttonFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.buttonFind.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(277, 70);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 9;
            this.buttonClose.Text = "Clos&e";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.buttonClose.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            // 
            // checkBoxCaseSensitive
            // 
            this.checkBoxCaseSensitive.AutoSize = true;
            this.checkBoxCaseSensitive.Location = new System.Drawing.Point(15, 41);
            this.checkBoxCaseSensitive.Name = "checkBoxCaseSensitive";
            this.checkBoxCaseSensitive.Size = new System.Drawing.Size(94, 17);
            this.checkBoxCaseSensitive.TabIndex = 2;
            this.checkBoxCaseSensitive.Text = "C&ase sensitive";
            this.checkBoxCaseSensitive.UseVisualStyleBackColor = true;
            this.checkBoxCaseSensitive.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.checkBoxCaseSensitive.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            // 
            // checkBoxBackward
            // 
            this.checkBoxBackward.AutoSize = true;
            this.checkBoxBackward.Location = new System.Drawing.Point(15, 59);
            this.checkBoxBackward.Name = "checkBoxBackward";
            this.checkBoxBackward.Size = new System.Drawing.Size(74, 17);
            this.checkBoxBackward.TabIndex = 3;
            this.checkBoxBackward.Text = "&Backward";
            this.checkBoxBackward.UseVisualStyleBackColor = true;
            this.checkBoxBackward.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.checkBoxBackward.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            // 
            // checkBoxTopMost
            // 
            this.checkBoxTopMost.AutoSize = true;
            this.checkBoxTopMost.Location = new System.Drawing.Point(15, 77);
            this.checkBoxTopMost.Name = "checkBoxTopMost";
            this.checkBoxTopMost.Size = new System.Drawing.Size(92, 17);
            this.checkBoxTopMost.TabIndex = 4;
            this.checkBoxTopMost.Text = "Always on &top";
            this.checkBoxTopMost.UseVisualStyleBackColor = true;
            this.checkBoxTopMost.CheckedChanged += new System.EventHandler(this.OnTopMostCheckedChanged);
            this.checkBoxTopMost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.checkBoxTopMost.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            // 
            // checkBoxStartsWith
            // 
            this.checkBoxStartsWith.AutoSize = true;
            this.checkBoxStartsWith.Location = new System.Drawing.Point(140, 41);
            this.checkBoxStartsWith.Name = "checkBoxStartsWith";
            this.checkBoxStartsWith.Size = new System.Drawing.Size(75, 17);
            this.checkBoxStartsWith.TabIndex = 5;
            this.checkBoxStartsWith.Text = "&Starts with";
            this.checkBoxStartsWith.UseVisualStyleBackColor = true;
            this.checkBoxStartsWith.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.checkBoxStartsWith.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            // 
            // checkBoxEndsWith
            // 
            this.checkBoxEndsWith.AutoSize = true;
            this.checkBoxEndsWith.Location = new System.Drawing.Point(140, 59);
            this.checkBoxEndsWith.Name = "checkBoxEndsWith";
            this.checkBoxEndsWith.Size = new System.Drawing.Size(72, 17);
            this.checkBoxEndsWith.TabIndex = 6;
            this.checkBoxEndsWith.Text = "En&ds with";
            this.checkBoxEndsWith.UseVisualStyleBackColor = true;
            this.checkBoxEndsWith.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.checkBoxEndsWith.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            // 
            // checkBoxRegEx
            // 
            this.checkBoxRegEx.AutoSize = true;
            this.checkBoxRegEx.Location = new System.Drawing.Point(140, 77);
            this.checkBoxRegEx.Name = "checkBoxRegEx";
            this.checkBoxRegEx.Size = new System.Drawing.Size(116, 17);
            this.checkBoxRegEx.TabIndex = 7;
            this.checkBoxRegEx.Text = "Regular e&xpression";
            this.checkBoxRegEx.UseVisualStyleBackColor = true;
            this.checkBoxRegEx.CheckedChanged += new System.EventHandler(this.OnRegExCheckedChanged);
            this.checkBoxRegEx.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.checkBoxRegEx.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            // 
            // FindForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(364, 105);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonFind);
            this.Controls.Add(this.checkBoxRegEx);
            this.Controls.Add(this.checkBoxEndsWith);
            this.Controls.Add(this.checkBoxStartsWith);
            this.Controls.Add(this.checkBoxTopMost);
            this.Controls.Add(this.checkBoxBackward);
            this.Controls.Add(this.checkBoxCaseSensitive);
            this.Controls.Add(this.comboBoxFind);
            this.Controls.Add(this.labelFind);
            this.MaximizeBox = false;
            this.Name = "FindForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Activated += new System.EventHandler(this.OnActivated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelFind;
        private System.Windows.Forms.ComboBox comboBoxFind;
        private System.Windows.Forms.CheckBox checkBoxCaseSensitive;
        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.CheckBox checkBoxBackward;
        private System.Windows.Forms.CheckBox checkBoxTopMost;
        private System.Windows.Forms.CheckBox checkBoxStartsWith;
        private System.Windows.Forms.CheckBox checkBoxEndsWith;
        private System.Windows.Forms.CheckBox checkBoxRegEx;
    }
}