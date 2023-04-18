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

using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Hospudka {
    public sealed class StatusStripHandler : IDisposable {
        private bool muted;
        private ContextMenu contextMenu;
        private DisplayMode displayMode;
        private double zoomLevel;
        private int maximum, temp;
        private long dataSize;
        private Settings settings;
        private StatusStrip statusStrip;
        private string msg4CtxMenu, msgA, msgB, tempMsg;
        private System.Timers.Timer finishedTimer;
        private System.Windows.Forms.Timer heartBeatTimer, labelTimer, timer;
        private ToolStripProgressBar progressBar;
        private ToolStripStatusLabel labelCache;
        private ToolStripStatusLabel labelCaps;
        private ToolStripStatusLabel labelIns;
        private ToolStripStatusLabel labelMessage;
        private ToolStripStatusLabel labelMuted;
        private ToolStripStatusLabel labelNum;
        private ToolStripStatusLabel labelScrl;
        private ToolStripStatusLabel labelSearchResult;
        private ToolStripStatusLabel labelUrl;
        private ToolStripStatusLabel labelZoomLvl;

        private delegate void SetMaximumCallback(int maximum);
        private delegate void SetMessageCallback(StatusMessageType statusMessageType, string message);
        private delegate void SetSetSearchResultCallback(int count, int activeMatchOrdinal);
        private delegate void SetUrlCallback(string url);
        private delegate void SetValueCallback(int value);
        private delegate void StatusStripHandlerCallback();

        public StatusStripHandler(StatusStrip statusStrip, DisplayMode displayMode, Settings settings) {

            labelMessage = new ToolStripStatusLabel() {
                Alignment = ToolStripItemAlignment.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                BorderSides = ToolStripStatusLabelBorderSides.All,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                Spring = true
            };
            labelMessage.MouseDown += new MouseEventHandler(SetStatusLabelText);
            labelMessage.MouseUp += new MouseEventHandler(SetStatusLabelText);
            labelMessage.MouseMove += new MouseEventHandler(SetStatusLabelText);

            labelUrl = new ToolStripStatusLabel() {
                Alignment = ToolStripItemAlignment.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                BorderSides = ToolStripStatusLabelBorderSides.All,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                Spring = true
            };
            labelUrl.MouseDown += new MouseEventHandler(SetStatusLabelText);
            labelUrl.MouseUp += new MouseEventHandler(SetStatusLabelText);
            labelUrl.MouseMove += new MouseEventHandler(SetStatusLabelText);

            progressBar = new ToolStripProgressBar() {
                Padding = new Padding(3),
                Margin = new Padding(1),
            };
            progressBar.ProgressBar.MinimumSize = new Size(10, 5);
            progressBar.ProgressBar.Style = ProgressBarStyle.Continuous;
            progressBar.MouseDown += new MouseEventHandler(SetNull);
            progressBar.MouseUp += new MouseEventHandler(SetNull);
            progressBar.MouseMove += new MouseEventHandler(SetNull);

            labelSearchResult = new ToolStripStatusLabel() {
                Alignment = ToolStripItemAlignment.Right,
                BorderSides = ToolStripStatusLabelBorderSides.All,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                AutoSize = false
            };
            labelSearchResult.MouseDown += new MouseEventHandler(SetStatusLabelText);
            labelSearchResult.MouseUp += new MouseEventHandler(SetStatusLabelText);
            labelSearchResult.MouseMove += new MouseEventHandler(SetStatusLabelText);

            labelMuted = new ToolStripStatusLabel() {
                Alignment = ToolStripItemAlignment.Right,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderSides = ToolStripStatusLabelBorderSides.All,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                AutoSize = false
            };
            labelMuted.MouseDown += new MouseEventHandler(SetStatusLabelText);
            labelMuted.MouseUp += new MouseEventHandler(SetStatusLabelText);
            labelMuted.MouseMove += new MouseEventHandler(SetStatusLabelText);

            labelZoomLvl = new ToolStripStatusLabel() {
                Alignment = ToolStripItemAlignment.Right,
                TextAlign = ContentAlignment.MiddleRight,
                BorderSides = ToolStripStatusLabelBorderSides.All,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                AutoSize = false
            };
            labelZoomLvl.MouseDown += new MouseEventHandler(SetStatusLabelText);
            labelZoomLvl.MouseUp += new MouseEventHandler(SetStatusLabelText);
            labelZoomLvl.MouseMove += new MouseEventHandler(SetStatusLabelText);

            labelCache = new ToolStripStatusLabel() {
                Alignment = ToolStripItemAlignment.Right,
                TextAlign = ContentAlignment.MiddleRight,
                BorderSides = ToolStripStatusLabelBorderSides.All,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                AutoSize = false
            };
            labelCache.MouseDown += new MouseEventHandler(SetStatusLabelText);
            labelCache.MouseUp += new MouseEventHandler(SetStatusLabelText);
            labelCache.MouseMove += new MouseEventHandler(SetStatusLabelText);

            labelCaps = new ToolStripStatusLabel() {
                Alignment = ToolStripItemAlignment.Right,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderSides = ToolStripStatusLabelBorderSides.All,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                AutoSize = false
            };
            labelCaps.MouseDown += new MouseEventHandler(SetNull);
            labelCaps.MouseUp += new MouseEventHandler(SetNull);
            labelCaps.MouseMove += new MouseEventHandler(SetNull);

            labelNum = new ToolStripStatusLabel() {
                Alignment = ToolStripItemAlignment.Right,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderSides = ToolStripStatusLabelBorderSides.All,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                AutoSize = false
            };
            labelNum.MouseDown += new MouseEventHandler(SetNull);
            labelNum.MouseUp += new MouseEventHandler(SetNull);
            labelNum.MouseMove += new MouseEventHandler(SetNull);

            labelIns = new ToolStripStatusLabel() {
                Alignment = ToolStripItemAlignment.Right,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderSides = ToolStripStatusLabelBorderSides.All,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                AutoSize = false
            };
            labelIns.MouseDown += new MouseEventHandler(SetNull);
            labelIns.MouseUp += new MouseEventHandler(SetNull);
            labelIns.MouseMove += new MouseEventHandler(SetNull);

            labelScrl = new ToolStripStatusLabel() {
                Alignment = ToolStripItemAlignment.Right,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderSides = ToolStripStatusLabelBorderSides.All,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                AutoSize = false
            };
            labelScrl.MouseDown += new MouseEventHandler(SetNull);
            labelScrl.MouseUp += new MouseEventHandler(SetNull);
            labelScrl.MouseMove += new MouseEventHandler(SetNull);

            statusStrip.MouseDown += new MouseEventHandler(SetNull);
            statusStrip.MouseUp += new MouseEventHandler(SetNull);
            statusStrip.MouseMove += new MouseEventHandler(SetNull);
            statusStrip.LayoutStyle = ToolStripLayoutStyle.Table;
            statusStrip.MaximumSize = new Size(0, 20);
            statusStrip.MinimumSize = new Size(0, 20);
            statusStrip.Items.Add(labelMessage);
            if (displayMode.Equals(DisplayMode.Standard)) {
                statusStrip.Items.Add(labelUrl);
                statusStrip.Items.Add(progressBar);
                statusStrip.Items.Add(labelSearchResult);
            }
            if (displayMode.Equals(DisplayMode.Basic)) {
                statusStrip.Items.Add(labelSearchResult);
            }
            statusStrip.Items.Add(labelMuted);
            statusStrip.Items.Add(labelZoomLvl);
            statusStrip.Items.Add(labelCache);
            statusStrip.Items.Add(labelCaps);
            statusStrip.Items.Add(labelNum);
            if (displayMode.Equals(DisplayMode.Basic)) {
                statusStrip.Items.Add(labelIns);
            }
            statusStrip.Items.Add(labelScrl);
            SetStatusStripControlsSize();

            InitializeProgressBarTimer();
            InitializeProgressBarFinishedTimer();
            InitializeStatusLabelTimer();
            InitializeHeartBeatTimer();

            statusStrip.Paint += new PaintEventHandler((sender, e) => e.Graphics.DrawLine(
                SystemPens.Control,
                e.ClipRectangle.X,
                e.ClipRectangle.Y,
                e.ClipRectangle.X + e.ClipRectangle.Width,
                e.ClipRectangle.Y));

            statusStrip.SizeChanged += new EventHandler(OnStatusStripSizeChanged);
            if (displayMode.Equals(DisplayMode.None)) {
                statusStrip.Visible = false;
            }
            this.displayMode = displayMode;
            this.settings = settings;
            this.statusStrip = statusStrip;
            settings.Saved += new EventHandler((sender, e) => {
                SetRenderMode();
                SetBorderStyle();
                SetMuted();
                SetZoomLevel();
                SetDataSize();
            });
            SetRenderMode();
            SetBorderStyle();
            BuildContextMenu();
            EnableContextMenu = true;
        }

        public bool EnableContextMenu {
            get {
                return statusStrip.ContextMenu != null;
            }
            set {
                statusStrip.ContextMenu = value ? statusStrip.ContextMenu ?? contextMenu : null;
            }
        }

        private void BuildContextMenu() {
            contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemAnalyzeUrl,
                new EventHandler((sender, e) => {
                    if (!string.IsNullOrEmpty(msg4CtxMenu)
                            && msg4CtxMenu.StartsWith(Constants.SchemeHttps, StringComparison.OrdinalIgnoreCase)) {

                        try {
                            StringBuilder arguments = new StringBuilder()
                                .Append(Constants.CommandLineSwitchWD)
                                .Append(Constants.Space)
                                .Append(StaticMethods.EscapeArgument(msg4CtxMenu));
                            Process.Start(Application.ExecutablePath, arguments.ToString());
                        } catch (Exception exception) {
                            Debug.WriteLine(exception);
                            ErrorLog.WriteLine(exception);
                        }
                    }
                })));
            contextMenu.MenuItems.Add(Constants.Hyphen.ToString());
            contextMenu.MenuItems.Add(new MenuItem(string.Empty,
                new EventHandler((sender, e) => {
                    if (!string.IsNullOrEmpty(msg4CtxMenu)) {
                        ClipboardSetText(msg4CtxMenu);
                    }
                })));
            contextMenu.Popup += new EventHandler((sender, e) => {
                contextMenu.MenuItems[0].Visible = !string.IsNullOrWhiteSpace(msg4CtxMenu);
            });
        }

        private void InitializeHeartBeatTimer() {
            heartBeatTimer = new System.Windows.Forms.Timer();
            heartBeatTimer.Interval = 250;
            heartBeatTimer.Tick += new EventHandler((sender, e) => {
                bool capsChanged = SetStatusLabelCaps();
                bool numChanged = SetStatusLabelNum();
                bool insChanged = SetStatusLabelIns();
                bool scrlChanged = SetStatusLabelScrl();
                if (capsChanged || numChanged || insChanged || scrlChanged) {
                    SetStatusStripControlsSize();
                }
            });
            heartBeatTimer.Start();
        }

        private void InitializeStatusLabelTimer() {
            labelTimer = new System.Windows.Forms.Timer();
            labelTimer.Interval = 30000;
            labelTimer.Tick += new EventHandler((sender, e) => {
                labelTimer.Stop();
                labelMessage.Text = Properties.Resources.MessageReady;
            });
        }

        private void InitializeProgressBarFinishedTimer() {
            finishedTimer = new System.Timers.Timer(500);
            finishedTimer.Elapsed += new System.Timers.ElapsedEventHandler((sender, e) => ResetProgressBar());
        }

        private void InitializeProgressBarTimer() {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = Constants.StripProgressBarInterval;
            timer.Tick += new EventHandler((sender, e) => {
                try {
                    int pValue = progressBar.Value + 3;
                    if (pValue < progressBar.Maximum) {
                        temp++;
                        progressBar.Value = pValue;
                        progressBar.Invalidate();
                    } else {
                        progressBar.Value = progressBar.Maximum;
                    }
                    if (temp > Constants.StripProgressBarInterval * 2) {
                        timer.Interval = temp / 2;
                    }
                    if ((float)progressBar.Value / progressBar.Maximum < 0.9f) {
                        return;
                    }
                    timer.Stop();
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                }
            });
        }

        private void SetStatusStripControlsSize() {
            labelSearchResult.Size = new Size(97, 10);
            labelMuted.Size = new Size(48, 10);
            labelZoomLvl.Size = new Size(48, 10);
            labelCache.Size = new Size(59, 10);
            Size size = new Size(39, 10);
            labelCaps.Size = size;
            labelNum.Size = size;
            labelIns.Size = size;
            labelScrl.Size = size;
        }

        private void SetRenderMode() {
            try {
                if (statusStrip.InvokeRequired) {
                    statusStrip.Invoke(new StatusStripHandlerCallback(SetRenderMode));
                } else {
                    statusStrip.RenderMode = settings.StripRenderMode == 0 ? ToolStripRenderMode.System : settings.StripRenderMode;
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        private void SetBorderStyle() {
            try {
                if (statusStrip.InvokeRequired) {
                    statusStrip.Invoke(new StatusStripHandlerCallback(SetBorderStyle));
                } else {
                    foreach (object item in statusStrip.Items) {
                        if (item is ToolStripStatusLabel) {
                            ((ToolStripStatusLabel)item).BorderStyle = settings.Border3DStyle == 0
                                ? Border3DStyle.Adjust
                                : settings.Border3DStyle;
                        }
                    }
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        public void ClearSearchResult() {
            try {
                if (statusStrip.InvokeRequired) {
                    statusStrip.Invoke(new StatusStripHandlerCallback(ClearSearchResult));
                } else {
                    labelSearchResult.Text = string.Empty;
                    SetStatusStripControlsSize();
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        private void SetStatusLabelText(object sender, EventArgs e) => msg4CtxMenu = ((ToolStripStatusLabel)sender).Text;

        private void SetStatusLabelText(object sender, MouseEventArgs e) {
            if (e.Button.Equals(MouseButtons.Right)) {
                msg4CtxMenu = ((ToolStripStatusLabel)sender).Text;
            }
        }

        private void SetNull(object sender, EventArgs e) => msg4CtxMenu = null;

        private void SetNull(object sender, MouseEventArgs e) {
            if (e.Button.Equals(MouseButtons.Right)) {
                msg4CtxMenu = null;
            }
        }

        public void SetMessage() => SetMessage(StatusMessageType.Empty);

        public void SetMessage(StatusMessageType statusMessageType) => SetMessage(statusMessageType, string.Empty);

        public void SetMessage(StatusMessageType statusMessageType, string message) {
            try {
                if (statusStrip.InvokeRequired) {
                    statusStrip.Invoke(new SetMessageCallback(SetMessage), statusMessageType, message);
                } else {
                    labelTimer.Stop();
                    message = message.Trim();
                    switch (statusMessageType) {
                        case StatusMessageType.Temporary:
                            if (string.IsNullOrWhiteSpace(message)) {
                                labelTimer.Stop();
                                tempMsg = null;
                                if (!string.IsNullOrWhiteSpace(msgA)) {
                                    labelMessage.Text = msgA.EndsWith(Constants.ThreeDots)
                                        ? message
                                        : message.Trim(Constants.Period) + Constants.Period;
                                } else if (!string.IsNullOrWhiteSpace(msgB)) {
                                    labelMessage.Text = msgB.EndsWith(Constants.ThreeDots)
                                        ? message
                                        : message.Trim(Constants.Period) + Constants.Period;
                                } else {
                                    labelMessage.Text = Properties.Resources.MessageReady;
                                }
                            } else {
                                tempMsg = message;
                                labelMessage.Text = message.EndsWith(Constants.ThreeDots)
                                    ? message
                                    : message.Trim(Constants.Period) + Constants.Period;
                                labelTimer.Start();
                            }
                            break;
                        case StatusMessageType.PersistentB:
                            if (string.IsNullOrWhiteSpace(message)) {
                                msgB = null;
                                if (string.IsNullOrWhiteSpace(msgA)) {
                                    if (string.IsNullOrWhiteSpace(tempMsg)) {
                                        labelMessage.Text = string.Empty;
                                    }
                                }
                            } else if (string.IsNullOrWhiteSpace(msgA)) {
                                labelTimer.Stop();
                                tempMsg = null;
                                msgB = message;
                                labelMessage.Text = message;
                            }
                            break;
                        case StatusMessageType.PersistentA:
                            if (string.IsNullOrWhiteSpace(message)) {
                                msgA = null;
                                if (string.IsNullOrWhiteSpace(msgB)) {
                                    if (string.IsNullOrWhiteSpace(tempMsg)) {
                                        labelMessage.Text = string.Empty;
                                    }
                                } else {
                                    labelMessage.Text = msgB;
                                }
                            } else {
                                labelTimer.Stop();
                                tempMsg = null;
                                msgA = message;
                                if (message.StartsWith(Constants.SchemeHttps, StringComparison.OrdinalIgnoreCase)) {
                                    labelMessage.Text = message;
                                } else {
                                    labelMessage.Text = message.EndsWith(Constants.ThreeDots)
                                        ? message
                                        : message.Trim(Constants.Period) + Constants.Period;
                                }
                            }
                            break;
                        default:
                            labelTimer.Stop();
                            tempMsg = null;
                            msgB = null;
                            msgA = null;
                            labelMessage.Text = string.Empty;
                            break;
                    }
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        public void SetUrl(string url) {
            if (Constants.BlankPageUri.Equals(url)) {
                url = string.Empty;
            }
            try {
                if (statusStrip.InvokeRequired) {
                    statusStrip.Invoke(new SetUrlCallback(SetUrl), url);
                } else {
                    labelUrl.Text = url;
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        public void SetSearchResult(int count, int activeMatchOrdinal) {
            try {
                if (statusStrip.InvokeRequired) {
                    statusStrip.Invoke(new SetSetSearchResultCallback(SetSearchResult), count, activeMatchOrdinal);
                } else {
                    if (count > 0) {
                        string match;
                        if (count < 1000) {
                            match = Constants.StripSearchMatches;
                        } else if (count < 2000) {
                            match = Constants.StripSearchMatchesShort1;
                        } else {
                            match = Constants.StripSearchMatchesShort2;
                        }
                        labelSearchResult.TextAlign = ContentAlignment.MiddleRight;
                        labelSearchResult.ForeColor = SystemColors.ControlText;
                        labelSearchResult.Text = string.Format(
                            Constants.StripSearchFormat,
                            activeMatchOrdinal,
                            count,
                            match);
                    } else {
                        labelSearchResult.TextAlign = ContentAlignment.MiddleCenter;
                        labelSearchResult.ForeColor = Color.Crimson;
                        labelSearchResult.Text = Constants.StripSearchNotFound;
                    }
                    SetStatusStripControlsSize();
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        public void SetMuted(bool muted) {
            this.muted = muted;
            SetMuted();
        }

        private void SetMuted() {
            try {
                if (statusStrip.InvokeRequired) {
                    statusStrip.Invoke(new StatusStripHandlerCallback(SetMuted));
                } else {
                    labelMuted.Text = muted ? Constants.Muted : string.Empty;
                    SetStatusStripControlsSize();
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        public void SetZoomLevel(double zoomLevel) {
            this.zoomLevel = zoomLevel;
            SetZoomLevel();
        }

        private void SetZoomLevel() {
            try {
                if (statusStrip.InvokeRequired) {
                    statusStrip.Invoke(new StatusStripHandlerCallback(SetZoomLevel));
                } else {
                    labelZoomLvl.Text = ShowZoomLevel(zoomLevel, settings.NumberFormat.cultureInfo);
                    SetStatusStripControlsSize();
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        public void SetDataSize(long dataSize) {
            this.dataSize = dataSize;
            SetDataSize();
        }

        private void SetDataSize() {
            if (dataSize > 0) {
                try {
                    if (statusStrip.InvokeRequired) {
                        statusStrip.Invoke(new StatusStripHandlerCallback(SetDataSize));
                    } else {
                        labelCache.Text = StaticMethods.ShowSize(
                            dataSize,
                            settings.NumberFormat.cultureInfo,
                            settings.UseDecimalPrefix);
                        SetStatusStripControlsSize();
                    }
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                }
            }
        }

        public void SetMaximum(int maximum) {
            try {
                if (statusStrip.InvokeRequired) {
                    statusStrip.Invoke(new SetMaximumCallback(SetMaximum), maximum);
                } else {
                    timer.Interval = Constants.StripProgressBarInterval;
                    progressBar.Maximum = Constants.StripProgressBarInternalMax;
                    this.maximum = maximum;
                    timer.Start();
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        public void SetValue(int value) {
            try {
                if (statusStrip.InvokeRequired) {
                    statusStrip.Invoke(new SetValueCallback(SetValue), value);
                } else {
                    if (value > maximum) {
                        value = maximum;
                    }
                    int pValue = value * Constants.StripProgressBarInternalMax / maximum;
                    if (pValue > progressBar.Value) {
                        temp = Constants.StripProgressBarInterval;
                        timer.Interval = Constants.StripProgressBarInterval;
                        progressBar.Value = pValue;
                    }
                    progressBar.Invalidate();
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        public void SetFinished() {
            try {
                if (statusStrip.InvokeRequired) {
                    statusStrip.Invoke(new StatusStripHandlerCallback(SetFinished));
                } else {
                    progressBar.Maximum = 1;
                    progressBar.Value = 1;
                    timer.Stop();
                    finishedTimer.Start();
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        public void ResetProgressBar() {
            try {
                if (statusStrip.InvokeRequired) {
                    statusStrip.Invoke(new StatusStripHandlerCallback(ResetProgressBar));
                } else {
                    timer.Stop();
                    finishedTimer.Stop();
                    progressBar.Value = 0;
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        private bool SetStatusLabelCaps() {
            string str = Control.IsKeyLocked(Keys.CapsLock)
                ? Properties.Resources.CaptionCapsLock
                : string.Empty;
            if (str.Equals(labelCaps.Text)) {
                return false;
            }
            labelCaps.Text = str;
            return true;
        }

        private bool SetStatusLabelNum() {
            string str = Control.IsKeyLocked(Keys.NumLock)
                ? Properties.Resources.CaptionNumLock
                : string.Empty;
            if (str.Equals(labelNum.Text)) {
                return false;
            }
            labelNum.Text = str;
            return true;
        }

        private bool SetStatusLabelIns() {
            string str = Control.IsKeyLocked(Keys.Insert)
                ? Properties.Resources.CaptionOverWrite
                : Properties.Resources.CaptionInsert;
            if (str.Equals(labelIns.Text)) {
                return false;
            }
            labelIns.Text = str;
            return true;
        }

        private bool SetStatusLabelScrl() {
            string str = Control.IsKeyLocked(Keys.Scroll)
                ? Properties.Resources.CaptionScrollLock
                : string.Empty;
            if (str.Equals(labelScrl.Text)) {
                return false;
            }
            labelScrl.Text = str;
            return true;
        }

        private void OnStatusStripSizeChanged(object sender, EventArgs e) {
            StatusStrip statusStrip = (StatusStrip)sender;
            labelUrl.Visible = statusStrip.Width > Constants.StripStatusLblUrlVLimit;
            labelSearchResult.Visible = statusStrip.Width > Constants.StripStatusLblSearchResVLimit;
            progressBar.Visible = statusStrip.Width > Constants.StripProgressBarVLimit;
            progressBar.ProgressBar.Width = statusStrip.Width / Constants.StripProgressBarWRatio;
            bool visible = statusStrip.Width > (displayMode.Equals(DisplayMode.Standard)
                ? Constants.StripStatusLblCacheVLimit
                : Constants.StripStatusLblCacheVLimitReduced);
            labelMuted.Visible = visible;
            labelZoomLvl.Visible = visible;
            labelCache.Visible = visible;
        }

        public void Dispose() {
            labelTimer.Stop();
            labelTimer.Dispose();
            finishedTimer.Stop();
            finishedTimer.Dispose();
            timer.Stop();
            timer.Dispose();
            heartBeatTimer.Stop();
            heartBeatTimer.Dispose();
        }

        private static void ClipboardSetText(string text) {
            try {
                Thread thread = new Thread(new ThreadStart(() => {
                    try {
                        Clipboard.SetText(text);
                    } catch (Exception exception) {
                        Debug.WriteLine(exception);
                        ErrorLog.WriteLine(exception);
                    }
                }));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        private static string ShowZoomLevel(double zoomLevel, IFormatProvider provider) {
            double perCent = zoomLevel < 0 ? 11.1111d * zoomLevel + 100 : 100 * zoomLevel / 3 + 100;
            return new StringBuilder()
                .Append(perCent.ToString(Constants.OneDecimalDigitFormat, provider))
                .Append(Constants.Space)
                .Append(Constants.Percent)
                .ToString();
        }

        public enum DisplayMode {
            None,
            Standard,
            Reduced,
            Basic
        }

        /// <summary>
        /// Type of the status strip message.
        /// </summary>
        public enum StatusMessageType {
            ///<summary>
            ///Empty status strip label.
            ///</summary>
            Empty,
            ///<summary>
            ///Temporary message, e.g. "Export finished." Expires in 30 seconds.
            ///After expiration text "Ready" will appear or a higher level
            ///message if any.
            ///</summary>
            Temporary,
            ///<summary>
            ///Persistent message level B, e.g. browser console messages.
            ///Overwrites a temporary message without replacement. Remains
            ///permanently until reset. After the reset, the status strip label
            ///will be empty.
            ///</summary>
            PersistentB,
            ///<summary>
            ///Persistent message level A, e.g. links while mouse hovering.
            ///Overwrites a temporary message without replacement. Remains
            ///permanently until reset. Replaces a lower-level persistent message
            ///until reset. After the reset, the status strip label will be empty
            ///or lower-level persistent message will be restored if any.
            ///</summary>
            PersistentA
        }
    }
}
