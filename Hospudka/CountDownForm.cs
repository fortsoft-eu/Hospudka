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

using FortSoft.Tools;
using System;
using System.Windows.Forms;

namespace Hospudka {
    public partial class CountDownForm : Form {
        private int sec;
        private PersistWindowState persistWindowState;
        private Timer timer;

        private delegate void CountDownFormback();

        public CountDownForm() {
            persistWindowState = new PersistWindowState();
            persistWindowState.SavingOptions = PersistWindowState.PersistWindowStateSavingOptions.None;
            persistWindowState.Parent = this;

            InitializeComponent();

            sec = Constants.TurnOffTheMonitorsInterval;
            labelSec.Text = sec.ToString();

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler((sender, e) => {
                if (sec > 0) {
                    labelSec.Text = (--sec).ToString();
                }
                if (sec.Equals(0)) {
                    timer.Stop();
                    SendMessage();
                    Close();
                }
            });
        }

        private void OnClick(object sender, EventArgs e) {
            timer.Stop();
            SendMessage();
            Close();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e) {
            timer.Stop();
            timer.Dispose();
        }

        private void OnFormLoad(object sender, EventArgs e) => timer.Start();

        public void SafeClose() {
            if (InvokeRequired) {
                Invoke(new CountDownFormback(SafeClose));
            } else {
                Close();
            }
        }

        public void SafeSelect() {
            if (InvokeRequired) {
                Invoke(new CountDownFormback(SafeSelect));
            } else {
                persistWindowState.Restore();
                persistWindowState.BringToFront();
            }
        }

        private void SendMessage() {
            NativeMethods.SendMessage(
                Handle,
                Constants.WM_SYSCOMMAND,
                (IntPtr)Constants.SC_MONITORPOWER,
                (IntPtr)MonitorState.MonitorStateOff);
        }

        private enum MonitorState {
            MonitorStateOn = -1,
            MonitorStateOff = 2,
            MonitorStateStandBy = 1
        }
    }
}
