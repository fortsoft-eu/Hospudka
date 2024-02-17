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

using FortSoft.Tools;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Hospudka {
    public abstract partial class FindForm : Form {
        private bool invertDirection;
        private bool searchKeyPressed;
        private int textBoxClicks;
        private PersistWindowState persistWindowState;
        private Point location;
        private Timer textBoxClicksTimer;

        protected bool cannotSearch;
        protected SearchHandler searchHandler;
        protected Timer searchDelayTimer;

        private delegate void FindFormCallback();
        private delegate void LocationCallback(Point point);

        public event EventHandler AltCtrlShiftEPressed;
        public event EventHandler AltCtrlShiftPPressed;
        public event EventHandler AltF10Pressed;
        public event EventHandler AltF11Pressed;
        public event EventHandler AltF12Pressed;
        public event EventHandler AltF7Pressed;
        public event EventHandler AltF8Pressed;
        public event EventHandler AltF9Pressed;
        public event EventHandler AltHomePressed;
        public event EventHandler AltLeftPressed;
        public event EventHandler AltLPressed;
        public event EventHandler AltRightPressed;
        public event EventHandler CtrlDPressed;
        public event EventHandler CtrlEPressed;
        public event EventHandler CtrlF5Pressed;
        public event EventHandler CtrlGPressed;
        public event EventHandler CtrlIPressed;
        public event EventHandler CtrlJPressed;
        public event EventHandler CtrlKPressed;
        public event EventHandler CtrlLPressed;
        public event EventHandler CtrlMinusPressed;
        public event EventHandler CtrlMPressed;
        public event EventHandler CtrlOPressed;
        public event EventHandler CtrlPlusPressed;
        public event EventHandler CtrlPPressed;
        public event EventHandler CtrlShiftDelPressed;
        public event EventHandler CtrlShiftEPressed;
        public event EventHandler CtrlShiftLPressed;
        public event EventHandler CtrlShiftMinusPressed;
        public event EventHandler CtrlShiftNPressed;
        public event EventHandler CtrlShiftPlusPressed;
        public event EventHandler CtrlShiftPPressed;
        public event EventHandler CtrlShiftTPressed;
        public event EventHandler CtrlSPressed;
        public event EventHandler CtrlTPressed;
        public event EventHandler CtrlUPressed;
        public event EventHandler CtrlZeroPressed;
        public event EventHandler DownPressed;
        public event EventHandler EndPressed;
        public event EventHandler F11Pressed;
        public event EventHandler F12Pressed;
        public event EventHandler F2Pressed;
        public event EventHandler F4Pressed;
        public event EventHandler F5Pressed;
        public event EventHandler F7Pressed;
        public event EventHandler F8Pressed;
        public event EventHandler F9Pressed;
        public event EventHandler HomePressed;
        public event EventHandler PageDownPressed;
        public event EventHandler PageUpPressed;
        public event EventHandler UpPressed;
        public event EventHandler<SearchEventArgs> Find;

        public FindForm(Search search) {
            Icon = Properties.Resources.Find;

            textBoxClicksTimer = new Timer();
            textBoxClicksTimer.Interval = SystemInformation.DoubleClickTime;
            textBoxClicksTimer.Tick += new EventHandler((sender, e) => {
                textBoxClicksTimer.Stop();
                textBoxClicks = 0;
            });

            persistWindowState = new PersistWindowState();
            persistWindowState.AllowSaveTopMost = true;
            persistWindowState.FixHeight = true;
            persistWindowState.Parent = this;
            persistWindowState.Loaded += new EventHandler<PersistWindowStateEventArgs>((sender, e) => checkBoxTopMost.Checked = TopMost);

            InitializeComponent();

            SuspendLayout();
            comboBoxFind.Text = search.searchString;
            checkBoxCaseSensitive.Checked = search.caseSensitive;
            checkBoxBackward.Checked = search.backward;
            checkBoxStartsWith.Checked = search.startsWith;
            checkBoxEndsWith.Checked = search.endsWith;
            checkBoxRegEx.Checked = search.regularExpression;
            ResumeLayout(false);
            PerformLayout();
        }

        private void OnActivated(object sender, EventArgs e) {
            comboBoxFind.Focus();
            comboBoxFind.SelectAll();
        }

        private void OnCommitted(object sender, EventArgs e) {
            Search recentSearch = searchHandler.Get((string)comboBoxFind.SelectedItem);
            checkBoxCaseSensitive.Checked = recentSearch.caseSensitive;
            checkBoxBackward.Checked = recentSearch.backward;
            checkBoxStartsWith.Checked = recentSearch.startsWith;
            checkBoxEndsWith.Checked = recentSearch.endsWith;
            checkBoxRegEx.Checked = recentSearch.regularExpression;
        }

        private void OnDropDown(object sender, EventArgs e) {
            ComboBox comboBox = (ComboBox)sender;
            Graphics graphics = comboBox.CreateGraphics();
            int scrollBarWidth = comboBox.Items.Count > comboBox.MaxDropDownItems ? SystemInformation.VerticalScrollBarWidth : 0;
            int dropDownWidth = comboBox.Width;
            foreach (object obj in comboBox.Items) {
                string str = obj.ToString();
                int newWidth = (int)graphics.MeasureString(str, comboBox.Font).Width + scrollBarWidth;
                if (newWidth > dropDownWidth) {
                    dropDownWidth = newWidth;
                }
            }
            comboBox.DropDownWidth = dropDownWidth;
        }

        private void OnFindButtonClick(object sender, EventArgs e) => Search(invertDirection);

        private void OnFormClosing(object sender, FormClosingEventArgs e) {
            if (searchDelayTimer != null) {
                searchDelayTimer.Stop();
                searchDelayTimer.Dispose();
            }
            textBoxClicksTimer.Dispose();
        }

        private void OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Control && e.KeyCode.Equals(Keys.A)) {
                e.SuppressKeyPress = true;
                if (sender is TextBox) {
                    ((TextBox)sender).SelectAll();
                } else if (sender is NumericUpDown) {
                    NumericUpDown numericUpDown = (NumericUpDown)sender;
                    numericUpDown.Select(0, numericUpDown.Text.Length);
                } else if (sender is ComboBox) {
                    ((ComboBox)sender).SelectAll();
                }
            } else if (e.Control && e.KeyCode.Equals(Keys.F)) {
                e.SuppressKeyPress = true;
                if (sender is ComboBox) {
                    ((ComboBox)sender).SelectAll();
                } else {
                    comboBoxFind.Focus();
                    comboBoxFind.SelectAll();
                }
            } else if (buttonFind.Enabled) {
                if (e.KeyCode.Equals(Keys.ShiftKey)) {
                    invertDirection = true;
                } else if (e.Alt && e.Control && e.Shift && e.KeyCode.Equals(Keys.E)) {
                    AltCtrlShiftEPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Alt && e.Control && e.Shift && e.KeyCode.Equals(Keys.P)) {
                    AltCtrlShiftPPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Alt && e.KeyCode.Equals(Keys.F7)) {
                    AltF7Pressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Alt && e.KeyCode.Equals(Keys.F8)) {
                    AltF8Pressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Alt && e.KeyCode.Equals(Keys.F9)) {
                    AltF9Pressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Alt && e.KeyCode.Equals(Keys.F10)) {
                    AltF10Pressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Alt && e.KeyCode.Equals(Keys.F11)) {
                    AltF11Pressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Alt && e.KeyCode.Equals(Keys.F12)) {
                    AltF12Pressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Alt && e.KeyCode.Equals(Keys.Home)) {
                    AltHomePressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Alt && e.KeyCode.Equals(Keys.Left)) {
                    AltLeftPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Alt && e.KeyCode.Equals(Keys.Right)) {
                    AltRightPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Alt && e.KeyCode.Equals(Keys.L)) {
                    AltLPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Shift && e.KeyCode.Equals(Keys.Enter) || e.Shift && e.KeyCode.Equals(Keys.F3)) {
                    invertDirection = true;
                    Search(true);
                    searchKeyPressed = true;
                } else if (e.KeyCode.Equals(Keys.Enter) || e.KeyCode.Equals(Keys.F3)) {
                    invertDirection = false;
                    Search(false);
                    searchKeyPressed = true;
                } else if (e.Control && e.KeyCode.Equals(Keys.Home)) {
                    e.SuppressKeyPress = true;
                    HomePressed?.Invoke(this, EventArgs.Empty);
                    SafeSelect();
                } else if (e.Control && e.KeyCode.Equals(Keys.End)) {
                    e.SuppressKeyPress = true;
                    EndPressed?.Invoke(this, EventArgs.Empty);
                    SafeSelect();
                } else if (e.Control && e.KeyCode.Equals(Keys.PageUp)) {
                    e.SuppressKeyPress = true;
                    PageUpPressed?.Invoke(this, EventArgs.Empty);
                    SafeSelect();
                } else if (e.Control && e.KeyCode.Equals(Keys.PageDown)) {
                    e.SuppressKeyPress = true;
                    PageDownPressed?.Invoke(this, EventArgs.Empty);
                    SafeSelect();
                } else if (e.Control && e.KeyCode.Equals(Keys.Up)) {
                    e.SuppressKeyPress = true;
                    UpPressed?.Invoke(this, EventArgs.Empty);
                    SafeSelect();
                } else if (e.Control && e.KeyCode.Equals(Keys.Down)) {
                    e.SuppressKeyPress = true;
                    DownPressed?.Invoke(this, EventArgs.Empty);
                    SafeSelect();
                } else if (e.Control && e.Shift && (e.KeyCode.Equals(Keys.Add) || e.KeyCode.Equals(Keys.Oemplus))) {
                    CtrlShiftPlusPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.Shift && (e.KeyCode.Equals(Keys.Subtract) || e.KeyCode.Equals(Keys.OemMinus))) {
                    CtrlShiftMinusPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.Shift && e.KeyCode.Equals(Keys.Delete)) {
                    CtrlShiftDelPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.Shift && e.KeyCode.Equals(Keys.E)) {
                    CtrlShiftEPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.Shift && e.KeyCode.Equals(Keys.L)) {
                    CtrlShiftLPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.Shift && e.KeyCode.Equals(Keys.N)) {
                    CtrlShiftNPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.Shift && e.KeyCode.Equals(Keys.P)) {
                    CtrlShiftPPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.Shift && e.KeyCode.Equals(Keys.T)) {
                    CtrlShiftTPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && (e.KeyCode.Equals(Keys.Add) || e.KeyCode.Equals(Keys.Oemplus))) {
                    CtrlPlusPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && (e.KeyCode.Equals(Keys.Subtract) || e.KeyCode.Equals(Keys.OemMinus))) {
                    CtrlMinusPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.NumPad0)) {
                    CtrlZeroPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.D)) {
                    CtrlDPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.E)) {
                    CtrlEPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.G)) {
                    CtrlGPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.I)) {
                    CtrlIPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.J)) {
                    CtrlJPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.K)) {
                    CtrlKPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.L)) {
                    CtrlLPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.M)) {
                    CtrlMPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.O)) {
                    CtrlOPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.P)) {
                    CtrlPPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.S)) {
                    CtrlSPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.T)) {
                    CtrlTPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.U)) {
                    CtrlUPressed?.Invoke(this, EventArgs.Empty);
                } else if (e.Control && e.KeyCode.Equals(Keys.F5)) {
                    CtrlF5Pressed?.Invoke(this, EventArgs.Empty);
                } else if (e.KeyCode.Equals(Keys.F2)) {
                    F2Pressed?.Invoke(this, EventArgs.Empty);
                } else if (e.KeyCode.Equals(Keys.F4)) {
                    F4Pressed?.Invoke(this, EventArgs.Empty);
                } else if (e.KeyCode.Equals(Keys.F5)) {
                    F5Pressed?.Invoke(this, EventArgs.Empty);
                } else if (e.KeyCode.Equals(Keys.F7)) {
                    F7Pressed?.Invoke(this, EventArgs.Empty);
                } else if (e.KeyCode.Equals(Keys.F8)) {
                    F8Pressed?.Invoke(this, EventArgs.Empty);
                } else if (e.KeyCode.Equals(Keys.F9)) {
                    F9Pressed?.Invoke(this, EventArgs.Empty);
                } else if (e.KeyCode.Equals(Keys.F11)) {
                    F11Pressed?.Invoke(this, EventArgs.Empty);
                } else if (e.KeyCode.Equals(Keys.F12)) {
                    F12Pressed?.Invoke(this, EventArgs.Empty);
                }
                Application.DoEvents();
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e) {
            ComboBox comboBox = (ComboBox)sender;
            if (IsKeyLocked(Keys.Insert)
                    && !char.IsControl(e.KeyChar)
                    && comboBox.SelectionLength.Equals(0)
                    && comboBox.SelectionStart < comboBox.Text.Length) {

                int selectionStart = comboBox.SelectionStart;
                StringBuilder stringBuilder = new StringBuilder(comboBox.Text);
                stringBuilder[comboBox.SelectionStart] = e.KeyChar;
                e.Handled = true;
                comboBox.Text = stringBuilder.ToString();
                comboBox.SelectionStart = selectionStart + 1;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode.Equals(Keys.ShiftKey)) {
                invertDirection = false;
            }
            searchKeyPressed = false;
        }

        private void OnMouseDown(object sender, MouseEventArgs e) {
            if (!e.Button.Equals(MouseButtons.Left)) {
                textBoxClicks = 0;
                return;
            }
            ComboBox comboBox = (ComboBox)sender;
            textBoxClicksTimer.Stop();
            if (comboBox.SelectionLength > 0) {
                textBoxClicks = 2;
            } else if (textBoxClicks.Equals(0) || Math.Abs(e.X - location.X) < 2 && Math.Abs(e.Y - location.Y) < 2) {
                textBoxClicks++;
            } else {
                textBoxClicks = 0;
            }
            location = e.Location;
            if (textBoxClicks.Equals(3)) {
                textBoxClicks = 0;
                NativeMethods.MouseEvent(Constants.MOUSEEVENTF_LEFTUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);
                comboBox.SelectAll();
                comboBox.Focus();
            } else {
                textBoxClicksTimer.Start();
            }
        }

        private void OnRegExCheckedChanged(object sender, EventArgs e) {
            if (checkBoxRegEx.Checked) {
                checkBoxStartsWith.Checked = false;
                checkBoxEndsWith.Checked = false;
                checkBoxStartsWith.Enabled = false;
                checkBoxEndsWith.Enabled = false;
            } else {
                checkBoxStartsWith.Enabled = true;
                checkBoxEndsWith.Enabled = true;
            }
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e) {
            comboBoxFind.Focus();
            comboBoxFind.SelectAll();
        }

        private void OnTopMostCheckedChanged(object sender, EventArgs e) {
            TopMost = checkBoxTopMost.Checked;
            if (!TopMost) {
                SendToBack();
            }
        }

        protected void HideExtCheckBoxes() {
            checkBoxStartsWith.Visible = false;
            checkBoxEndsWith.Visible = false;
            checkBoxRegEx.Visible = false;
        }

        public void SafeBringToFront() {
            if (InvokeRequired) {
                Invoke(new FindFormCallback(SafeBringToFront));
            } else {
                BringToFront();
            }
        }

        public void SafeClose() {
            if (InvokeRequired) {
                Invoke(new FindFormCallback(SafeClose));
            } else {
                Close();
            }
        }

        public void SafeDisable() {
            if (InvokeRequired) {
                Invoke(new FindFormCallback(SafeDisable));
            } else {
                buttonFind.Enabled = false;
                buttonClose.Enabled = false;
                DisableCloseButton();
                invertDirection = false;
                searchKeyPressed = false;
                Cursor = Cursors.AppStarting;
            }
        }

        public void SafeEnable() {
            if (InvokeRequired) {
                Invoke(new FindFormCallback(SafeEnable));
            } else {
                buttonFind.Enabled = true;
                buttonClose.Enabled = true;
                EnableCloseButton();
                comboBoxFind.Focus();
                comboBoxFind.SelectAll();
                Cursor = Cursors.Default;
            }
        }

        public void SafeHide() {
            if (InvokeRequired) {
                Invoke(new FindFormCallback(SafeHide));
            } else if (!WindowState.Equals(FormWindowState.Minimized)) {
                WindowState = FormWindowState.Minimized;
            }
        }

        public void SafeLocation(Point point) {
            if (InvokeRequired) {
                Invoke(new LocationCallback(SafeLocation), point);
            } else {
                Location = point;
            }
        }

        public void SafeSelect() {
            if (InvokeRequired) {
                Invoke(new FindFormCallback(SafeSelect));
            } else {
                persistWindowState.Restore();
                persistWindowState.BringToFront();
                comboBoxFind.Focus();
                comboBoxFind.SelectAll();
            }
        }

        public void SafeShow() {
            if (InvokeRequired) {
                Invoke(new FindFormCallback(SafeShow));
            } else {
                persistWindowState.Restore();
            }
        }

        public void SafeTopMost() {
            if (InvokeRequired) {
                Invoke(new FindFormCallback(SafeTopMost));
            } else {
                checkBoxTopMost.Checked = false;
                checkBoxTopMost.Checked = true;
            }
        }

        private void Search(bool invertDirection) {
            if (searchKeyPressed || cannotSearch) {
                return;
            }
            if (!string.IsNullOrWhiteSpace(comboBoxFind.Text)) {
                searchHandler.Add(new Search(
                    comboBoxFind.Text,
                    checkBoxCaseSensitive.Checked,
                    checkBoxBackward.Checked,
                    checkBoxStartsWith.Checked,
                    checkBoxEndsWith.Checked,
                    checkBoxRegEx.Checked));
                searchHandler.Save();
            }
            comboBoxFind.Focus();
            comboBoxFind.SelectAll();
            Find?.Invoke(this, new SearchEventArgs(
                Handle, new Search(
                    comboBoxFind.Text,
                    checkBoxCaseSensitive.Checked,
                    invertDirection ? !checkBoxBackward.Checked : checkBoxBackward.Checked,
                    checkBoxStartsWith.Checked,
                    checkBoxEndsWith.Checked,
                    checkBoxRegEx.Checked)));
            if (searchDelayTimer != null) {
                cannotSearch = true;
                searchDelayTimer.Start();
            }
        }

        protected void UpdateFindComboBoxList(object sender, EventArgs e) {
            comboBoxFind.Items.Clear();
            comboBoxFind.Items.AddRange(((SearchHandler)sender).Get());
        }

        protected void AcceptInsert() => comboBoxFind.KeyPress += new KeyPressEventHandler(OnKeyPress);

        protected void SetAcceptButton() => AcceptButton = buttonFind;

        protected void SetMaxDropDownItems(int maxDropDownItems) => comboBoxFind.MaxDropDownItems = maxDropDownItems;

        protected void DisableCloseButton() {
            NativeMethods.EnableMenuItem(NativeMethods.GetSystemMenu(Handle, false), Constants.SC_CLOSE, 1);
        }

        protected void EnableCloseButton() {
            NativeMethods.EnableMenuItem(NativeMethods.GetSystemMenu(Handle, false), Constants.SC_CLOSE, 0);
        }
    }
}
