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
 * Version 1.1.0.1
 */

using FortSoft.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace Hospudka {
    public partial class MainForm : Form {
        private Form dialog;
        private FtpWebHandler ftpWebHandler;
        private Gong gong;
        private long removeCount, removedCount, totalCount, uploadCount;
        private PersistWindowState persistWindowState;
        private Settings settings;
        private ToolStripStatusLabel labelMessage;
        private WebPatcher webPatcher;

        public MainForm(Settings settings) {
            gong = new Gong();
            this.settings = settings;

            Icon = Properties.Resources.Icon;
            Text = Program.GetTitle();

            ftpWebHandler = new FtpWebHandler();
            ftpWebHandler.ContentUploaded += new EventHandler(OnContentUploaded);
            ftpWebHandler.Error += new ErrorEventHandler(OnError);
            ftpWebHandler.RemoteDirectoryEmpty += new EventHandler(OnRemoteDirectoryEmpty);
            ftpWebHandler.RemoveItem += new EventHandler<RemoteEventArgs>(OnRemoveItem);
            ftpWebHandler.UploadItem += new EventHandler<RemoteEventArgs>(OnUploadItem);

            webPatcher = new WebPatcher();
            webPatcher.Error += new ErrorEventHandler(OnError);
            webPatcher.Patched += new EventHandler<PatchedEventArgs>(OnPatched);

            ParseConfig(settings.Config);

            persistWindowState = new PersistWindowState();
            persistWindowState.DisableSaveSize = true;
            persistWindowState.DisableSaveWindowState = true;
            persistWindowState.Parent = this;

            InitializeComponent();

            LoadLogo();

            fileSystemWatcher.Path = webPatcher.SourcePath;
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileSystemWatcher.Changed += new FileSystemEventHandler(OnFileSystemWatcher);
            fileSystemWatcher.Created += new FileSystemEventHandler(OnFileSystemWatcher);
            fileSystemWatcher.Deleted += new FileSystemEventHandler(OnFileSystemWatcher);
            fileSystemWatcher.Error += new ErrorEventHandler(OnFileSystemWatcherError);
            fileSystemWatcher.Filter = Constants.HtmlFileExtension;
            fileSystemWatcher.IncludeSubdirectories = false;

            labelMessage = new ToolStripStatusLabel() {
                Alignment = ToolStripItemAlignment.Left,
                BorderStyle = Border3DStyle.Adjust,
                TextAlign = ContentAlignment.MiddleLeft,
                BorderSides = ToolStripStatusLabelBorderSides.All,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                Spring = true
            };
            statusStrip.Items.Add(labelMessage);
            statusStrip.Paint += new PaintEventHandler((sender, e) => e.Graphics.DrawLine(
                SystemPens.Control,
                e.ClipRectangle.X,
                e.ClipRectangle.Y,
                e.ClipRectangle.X + e.ClipRectangle.Width,
                e.ClipRectangle.Y));
        }

        private void EditRemoteConfig(object sender, EventArgs e) {
            try {
                Process.Start(Application.ExecutablePath, Constants.CommandLineSwitchWE);
            } catch (Exception exception) {
                ShowException(exception);
            }
        }

        private async void LoadLogo() {
            using (HttpClient httpClient = new HttpClient()) {
                StringBuilder stringBuilder = new StringBuilder()
                    .Append(Properties.Resources.Website.TrimEnd(Constants.Slash).ToLowerInvariant())
                    .Append(Constants.Slash)
                    .Append(Application.ProductName.ToLowerInvariant())
                    .Append(Constants.Slash)
                    .Append(Constants.LogoFileName);
                try {
                    byte[] bytes = await httpClient.GetByteArrayAsync(stringBuilder.ToString());
                    if (bytes != null && bytes.Length > 0) {
                        using (MemoryStream memoryStream = new MemoryStream(bytes, 0, bytes.Length)) {
                            memoryStream.Write(bytes, 0, bytes.Length);
                            pictureBox.Image = Image.FromStream(memoryStream, true);
                        }
                    }
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                }
            }
        }

        private void OnButtonDeployClick(object sender, EventArgs e) {
            if (!webPatcher.IsSourceDirectoryEmpty() || !webPatcher.IsDestinationDirectoryEmpty()) {
                if (fileSystemWatcher.EnableRaisingEvents) {
                    try {
                        fileSystemWatcher.EnableRaisingEvents = false;
                        Cursor = Cursors.AppStarting;
                        progressBar.Style = ProgressBarStyle.Marquee;
                        labelProgress.Text = string.Empty;
                        webPatcher.ProcessAsync();
                        SetMessage(Properties.Resources.MessagePatching);
                    } catch (Exception exception) {
                        Debug.WriteLine(exception);
                        ErrorLog.WriteLine(exception);
                        SetStatus();
                        try {
                            fileSystemWatcher.EnableRaisingEvents = true;
                        } catch (Exception ex) {
                            Debug.WriteLine(ex);
                            ErrorLog.WriteLine(ex);
                        }
                    }
                }
            } else {
                SetStatus();
                ShowNothingToDo();
            }
        }

        private void OnError(object sender, ErrorEventArgs e) => ShowException(e.GetException());

        private void OnFileSystemWatcher(object sender, FileSystemEventArgs e) => SetStatus();

        private void OnFileSystemWatcherError(object sender, ErrorEventArgs e) => ShowException(e.GetException());

        private void OnFormActivated(object sender, EventArgs e) {
            if (dialog != null) {
                dialog.Activate();
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e) {
            settings.Save();
            webPatcher.Dispose();
        }

        private void OnFormLoad(object sender, EventArgs e) {
            checkBoxChime.Checked = settings.ChimeWhenDone;
            settings.Save();
            SetStatus();
        }

        private void OnPatched(object sender, PatchedEventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler<PatchedEventArgs>(OnPatched), sender, e);
            } else if (e.Count > 0) {
                SetMessage(Properties.Resources.MessageCountingFiles);
                removeCount = e.Count;
                uploadCount = e.Count;
                totalCount = removeCount + uploadCount + 1;
                progressBar.Maximum = (int)totalCount;
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Value = 1;
                labelProgress.Text = new StringBuilder()
                    .Append(progressBar.Value * 100 / progressBar.Maximum)
                    .Append(Constants.Space)
                    .Append(Constants.Percent)
                    .ToString();
                removedCount = 0;
                ftpWebHandler.EmptyRemoteDirectoryAsync();
            } else {
                ShowNothingToDo();
            }
        }

        private void OnRemoteDirectoryEmpty(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler(OnRemoteDirectoryEmpty), sender, e);
            } else {
                if (removedCount < removeCount) {
                    totalCount = removedCount + uploadCount + 1;
                    progressBar.Maximum = 1 + (int)totalCount;
                }
                progressBar.Value = 1 + (int)removedCount;
                labelProgress.Text = new StringBuilder()
                    .Append(progressBar.Value * 100 / progressBar.Maximum)
                    .Append(Constants.Space)
                    .Append(Constants.Percent)
                    .ToString();
                ftpWebHandler.UploadLocalDirectoryAsync();
            }
        }

        private void OnRemoveItem(object sender, RemoteEventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler<RemoteEventArgs>(OnRemoveItem), sender, e);
            } else {
                switch (e.FileType) {
                    case RemoteFileInfo.FileType.File:
                        SetMessage(string.Format(Properties.Resources.MessageDeletingFile, Path.GetFileName(e.ItemPath)));
                        break;
                    case RemoteFileInfo.FileType.Directory:
                        SetMessage(string.Format(Properties.Resources.MessageRemovingDirectory, Path.GetFileName(e.ItemPath)));
                        break;
                    default:
                        return;
                }
                if (++removedCount > removeCount) {
                    removeCount = removedCount;
                    totalCount = removeCount + uploadCount + 1;
                    progressBar.Maximum = (int)totalCount;
                }
                progressBar.Value = 1 + (int)removedCount;
                labelProgress.Text = new StringBuilder()
                    .Append(progressBar.Value * 100 / progressBar.Maximum)
                    .Append(Constants.Space)
                    .Append(Constants.Percent)
                    .ToString();
            }
        }

        private void OnContentUploaded(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler(OnContentUploaded), sender, e);
            } else {
                webPatcher.EmptyDestinationDirectoryAsync();
                progressBar.Value = progressBar.Maximum;
                labelProgress.Text = new StringBuilder()
                    .Append(100)
                    .Append(Constants.Space)
                    .Append(Constants.Percent)
                    .ToString();
                try {
                    fileSystemWatcher.EnableRaisingEvents = true;
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                }
                if (checkBoxChime.Checked) {
                    try {
                        gong.Chime();
                    } catch (Exception exception) {
                        Debug.WriteLine(exception);
                        ErrorLog.WriteLine(exception);
                    }
                }
                settings.ChimeWhenDone = checkBoxChime.Checked;
                SetMessage(Properties.Resources.MessageDone);
                Cursor = Cursors.Default;
            }
        }

        private void OnUploadItem(object sender, RemoteEventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler<RemoteEventArgs>(OnUploadItem), sender, e);
            } else {
                switch (e.FileType) {
                    case RemoteFileInfo.FileType.File:
                        SetMessage(string.Format(Properties.Resources.MessageUploadingFile, Path.GetFileName(e.ItemPath)));
                        break;
                    case RemoteFileInfo.FileType.Directory:
                        SetMessage(string.Format(Properties.Resources.MessageCreatingDirectory, Path.GetFileName(e.ItemPath)));
                        break;
                    default:
                        return;
                }
                if (progressBar.Value < progressBar.Maximum) {
                    progressBar.Value++;
                    labelProgress.Text = new StringBuilder()
                        .Append(progressBar.Value * 100 / progressBar.Maximum)
                        .Append(Constants.Space)
                        .Append(Constants.Percent)
                        .ToString();
                }
            }
        }

        private void OpenHelp(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler(OpenHelp), sender, e);
            } else {
                try {
                    StringBuilder url = new StringBuilder()
                        .Append(Properties.Resources.Website.TrimEnd(Constants.Slash).ToLowerInvariant())
                        .Append(Constants.Slash)
                        .Append(Application.ProductName.ToLowerInvariant())
                        .Append(Constants.Slash);
                    Process.Start(url.ToString());
                } catch (Exception exception) {
                    ShowException(exception);
                }
            }
        }

        private void OpenHelp(object sender, HelpEventArgs e) => OpenHelp(sender, (EventArgs)e);

        private void ParseConfig(string config) {
            bool sectionStrings = false;
            List<string> listStrings = new List<string>();
            StringReader stringReader = new StringReader(config);
            for (string line; (line = stringReader.ReadLine()) != null;) {
                if (sectionStrings) {
                    listStrings.Add(line);
                } else if (line.Trim().Equals(Constants.ConfigStrings)) {
                    sectionStrings = true;
                } else {
                    string[] configLine = line.Split(new char[] { Constants.EqualSign }, 2);
                    if (configLine.Length.Equals(2)) {
                        string value = configLine[1].Trim();
                        switch (configLine[0].Trim()) {
                            case Constants.ConfigSourcePath:
                                webPatcher.SourcePath = value;
                                break;
                            case Constants.ConfigResourcesPath:
                                webPatcher.ResourcesPath = value;
                                break;
                            case Constants.ConfigWebLocalPath:
                                webPatcher.DestinationPath = value;
                                ftpWebHandler.LocalDirectory = value;
                                break;
                            case Constants.ConfigFtpHost:
                                ftpWebHandler.Host = value;
                                break;
                            case Constants.ConfigFtpPort:
                                ushort val;
                                ushort.TryParse(value, out val);
                                ftpWebHandler.Port = val;
                                break;
                            case Constants.ConfigFtpDirectory:
                                ftpWebHandler.RemoteDirectory = value;
                                break;
                            case Constants.ConfigUserName:
                                ftpWebHandler.UserName = value;
                                break;
                            case Constants.ConfigPassword:
                                ftpWebHandler.Password = value;
                                break;
                        }
                    }
                }
            }
            webPatcher.Strings = listStrings.ToArray();
        }

        private void SetMessage(string message) {
            labelMessage.Text = message.EndsWith(Constants.ThreeDots) ? message : message.TrimEnd(Constants.Period) + Constants.Period;
        }

        private void SetStatus() {
            if (webPatcher.IsSourceDirectoryEmpty()) {
                SetMessage(Properties.Resources.MessageNothingToDo);
            } else {
                SetMessage(Properties.Resources.MessageReady);
            }
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;
            labelProgress.Text = string.Empty;
        }

        private void ShowAbout(object sender, EventArgs e) {
            dialog = new AboutForm();
            dialog.HelpRequested += new HelpEventHandler(OpenHelp);
            dialog.ShowDialog(this);
        }

        private void ShowException(Exception exception) => ShowException(exception, null);

        private void ShowException(Exception exception, string statusMessage) {
            Debug.WriteLine(exception);
            ErrorLog.WriteLine(exception);
            StringBuilder title = new StringBuilder()
                .Append(Program.GetTitle())
                .Append(Constants.Space)
                .Append(Constants.EnDash)
                .Append(Constants.Space)
                .Append(Properties.Resources.CaptionError);
            dialog = new MessageForm(this, exception.Message, title.ToString(), MessageForm.Buttons.OK, MessageForm.BoxIcon.Error);
            dialog.HelpRequested += new HelpEventHandler(OpenHelp);
            dialog.ShowDialog(this);
        }

        private void ShowNothingToDo() {
            StringBuilder message = new StringBuilder(Properties.Resources.MessageNothingToDo)
                .Append(Constants.Period);
            dialog = new MessageForm(this, message.ToString(), Properties.Resources.CaptionInformation, MessageForm.Buttons.OK,
                MessageForm.BoxIcon.Information);
            dialog.HelpRequested += new HelpEventHandler(OpenHelp);
            dialog.ShowDialog(this);
        }
    }
}
