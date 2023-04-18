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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hospudka {
    public partial class EncDecForm : Form {
        private bool cancel, topMost, undone;
        private FileExtensionFilter fileExtensionFilter;
        private Font printFont;
        private Form dialog;
        private int availableHeight, availableWidth, doEvents, textBoxClicks;
        private PersistWindowState persistWindowState;
        private Point location;
        private PrintAction printAction;
        private PrintDialog printDialog;
        private PrintDocument printDocument;
        private PrintPreviewDialog printPreviewDialog;
        private PrintType printType;
        private SaveFileDialog saveFileDialog;
        private Settings settings;
        private Size availableSize;
        private StatusStripHandler statusStripHandler;
        private string stringToPrint, undoneInput;
        private TextBox textBoxEntered;
        private Timer textBoxClicksTimer;
        private UpdateChecker updateChecker;

        public EncDecForm(Settings settings) : this(settings, null) { }

        public EncDecForm(Settings settings, string str) {
            this.settings = settings;

            Icon = Properties.Resources.Binary;
            Text = new StringBuilder()
                .Append(Program.GetTitle())
                .Append(Constants.Space)
                .Append(Constants.EnDash)
                .Append(Constants.Space)
                .Append(Properties.Resources.CaptionEncoderDecoder)
                .ToString();

            textBoxClicksTimer = new Timer();
            textBoxClicksTimer.Interval = SystemInformation.DoubleClickTime;
            textBoxClicksTimer.Tick += new EventHandler((sender, e) => {
                textBoxClicksTimer.Stop();
                textBoxClicks = 0;
            });

            fileExtensionFilter = new FileExtensionFilter(settings.ExtensionFilterIndex);
            persistWindowState = new PersistWindowState();
            persistWindowState.DetectionOptions = PersistWindowState.WindowDetectionOptions.TitleEquals;
            persistWindowState.Parent = this;

            InitializePrintAsync();
            InitializeUpdateCheckerAsync();
            InitializeSaveFileDialogAsync();
            BuildMainMenuAsync();

            InitializeComponent();

            BuildContextMenuAsync();
            InitializeStatusStripHandler();

            textBoxInput.Text = str;
            textBoxInput.Font = new Font(Constants.MonospaceFontName, 10, FontStyle.Regular, GraphicsUnit.Point, 238);
            textBoxOutput.Font = new Font(Constants.MonospaceFontName, 10, FontStyle.Regular, GraphicsUnit.Point, 238);

            Menu.MenuItems[2].MenuItems[2].Checked = textBoxInput.WordWrap;
            EnableControls();
        }

        private async void BuildMainMenuAsync() {
            await Task.Run(new Action(() => {
                MainMenu mainMenu = new MainMenu();
                MenuItem menuItemFile = mainMenu.MenuItems.Add(Properties.Resources.MenuItemFile);
                MenuItem menuItemEdit = mainMenu.MenuItems.Add(Properties.Resources.MenuItemEdit);
                MenuItem menuItemOptions = mainMenu.MenuItems.Add(Properties.Resources.MenuItemOptions);
                MenuItem menuItemHelp = mainMenu.MenuItems.Add(Properties.Resources.MenuItemHelp);
                menuItemFile.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemExport,
                    new EventHandler(Export), Shortcut.CtrlE));
                menuItemFile.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemFile.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemPrintPreview + Constants.ShortcutShiftCtrlP,
                    new EventHandler(PrintPreview)));
                menuItemFile.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemPrint,
                    new EventHandler(Print), Shortcut.CtrlP));
                menuItemFile.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemPrintImagePreview,
                    new EventHandler(PrintImagePreview)));
                menuItemFile.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemPrintImage + Constants.ShortcutAltShiftCtrlP,
                    new EventHandler(PrintImage)));
                menuItemFile.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemFile.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemExit,
                    new EventHandler((sender, e) => Close()), Shortcut.AltF4));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemUndo,
                    new EventHandler(Undo), Shortcut.CtrlZ));
                menuItemEdit.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemCut,
                    new EventHandler(Cut), Shortcut.CtrlX));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemCopy,
                    new EventHandler(Copy), Shortcut.CtrlC));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemPaste,
                    new EventHandler(Paste), Shortcut.CtrlV));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemDelete + Constants.ShortcutDelete,
                    new EventHandler(Delete)));
                menuItemEdit.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemSelectAll,
                    new EventHandler(SelectAll), Shortcut.CtrlA));
                menuItemEdit.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemEncodeRFC1738,
                    new EventHandler(EncodeRFC1738), Shortcut.F5));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemEncodeRFC3986,
                    new EventHandler(EncodeRFC3986), Shortcut.F6));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemEncodeBase64,
                    new EventHandler(EncodeBase64), Shortcut.F7));
                menuItemEdit.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemParseUrl,
                    new EventHandler(ParseUrl), Shortcut.F8));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemDecodeUrl,
                    new EventHandler(DecodeUrl), Shortcut.F9));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemDecodeBase64,
                    new EventHandler(DecodeBase64), Shortcut.F10));
                menuItemEdit.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemASCIIFull,
                    new EventHandler(ConvertToASCIIFull), Shortcut.F11));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemASCIISafePath,
                    new EventHandler(ConvertToASCIISafePath), Shortcut.AltF11));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemASCIISafeFileName,
                    new EventHandler(ConvertToASCIISafeFileName), Shortcut.AltF12));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemASCIIAlphanumeric,
                    new EventHandler(ConvertToASCIIAlphanumeric), Shortcut.F12));
                menuItemEdit.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemClearInput,
                    new EventHandler(ClearInput), Shortcut.CtrlI));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemFlip,
                    new EventHandler(Flip), Shortcut.CtrlU));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemClearOutput,
                    new EventHandler(ClearOutput), Shortcut.CtrlO));
                menuItemEdit.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemFocusInput,
                    new EventHandler(FocusInput), Shortcut.F2));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemFocusOutput,
                    new EventHandler(FocusOutput), Shortcut.F3));
                menuItemOptions.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemAlwaysOnTop + Constants.ShortcutShiftCtrlT,
                    new EventHandler(ToggleTopMost)));
                menuItemOptions.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemOptions.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemWordWrap + Constants.ShortcutShiftCtrlL,
                    new EventHandler(ToggleWordWrap)));
                menuItemHelp.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemOnlineHelp,
                    new EventHandler(OpenHelp), Shortcut.F1));
                menuItemHelp.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemCheckForUpdates,
                    new EventHandler(CheckUpdates)));
                menuItemHelp.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemHelp.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemAbout,
                    new EventHandler(ShowAbout)));
                Menu = mainMenu;
                menuItemEdit.Popup += new EventHandler((sender, e) => {
                    TextBox textBox = GetTextBoxEntered();
                    if (textBox == null) {
                        Menu.MenuItems[1].MenuItems[0].Enabled = false;
                        Menu.MenuItems[1].MenuItems[2].Enabled = false;
                        Menu.MenuItems[1].MenuItems[3].Enabled = false;
                        Menu.MenuItems[1].MenuItems[4].Enabled = false;
                        Menu.MenuItems[1].MenuItems[5].Enabled = false;
                        Menu.MenuItems[1].MenuItems[7].Enabled = false;
                    } else {
                        Menu.MenuItems[1].MenuItems[0].Enabled = textBox.CanUndo || !string.IsNullOrEmpty(undoneInput) || undone;
                        bool enabled = textBox.SelectionLength > 0;
                        Menu.MenuItems[1].MenuItems[2].Enabled = !textBox.ReadOnly && enabled;
                        Menu.MenuItems[1].MenuItems[3].Enabled = enabled;
                        Menu.MenuItems[1].MenuItems[5].Enabled = !textBox.ReadOnly && enabled;
                        Menu.MenuItems[1].MenuItems[7].Enabled = textBox.SelectionLength < textBox.TextLength;
                        try {
                            Menu.MenuItems[1].MenuItems[4].Enabled = !textBox.ReadOnly && Clipboard.ContainsText();
                        } catch (Exception exception) {
                            Menu.MenuItems[1].MenuItems[4].Enabled = true;
                            Debug.WriteLine(exception);
                            ErrorLog.WriteLine(exception);
                        }
                    }
                });
            }));
        }

        private async void BuildContextMenuAsync() {
            await Task.Run(new Action(() => {
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemUndo,
                    new EventHandler(Undo)));
                contextMenu.MenuItems.Add(Constants.Hyphen.ToString());
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemCut,
                    new EventHandler((sender, e) => textBoxInput.Cut())));
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemCopy,
                    new EventHandler(Copy)));
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemPaste,
                    new EventHandler((sender, e) => textBoxInput.Paste())));
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemDelete,
                    new EventHandler((sender, e) => SendKeys.Send(Constants.SendKeysDelete))));
                contextMenu.MenuItems.Add(Constants.Hyphen.ToString());
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemSelectAll,
                    new EventHandler(SelectAll)));
                contextMenu.Popup += new EventHandler((sender, e) => {
                    if (!textBoxInput.Focused) {
                        textBoxInput.Focus();
                    }
                    contextMenu.MenuItems[0].Enabled = textBoxInput.CanUndo || !string.IsNullOrEmpty(undoneInput) || undone;
                    bool enabled = textBoxInput.SelectionLength > 0;
                    contextMenu.MenuItems[2].Enabled = enabled;
                    contextMenu.MenuItems[3].Enabled = enabled;
                    contextMenu.MenuItems[5].Enabled = enabled;
                    contextMenu.MenuItems[7].Enabled = textBoxInput.SelectionLength < textBoxInput.TextLength;
                    try {
                        contextMenu.MenuItems[4].Enabled = Clipboard.ContainsText();
                    } catch (Exception exception) {
                        contextMenu.MenuItems[4].Enabled = true;
                        Debug.WriteLine(exception);
                        ErrorLog.WriteLine(exception);
                    }
                });
                textBoxInput.ContextMenu = contextMenu;
            })).
            ContinueWith(new Action<Task>(task => {
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemCopy,
                    new EventHandler(Copy)));
                contextMenu.MenuItems.Add(Constants.Hyphen.ToString());
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemSelectAll,
                    new EventHandler(SelectAll)));
                contextMenu.Popup += new EventHandler((sender, e) => {
                    TextBox textBox = (TextBox)contextMenu.SourceControl;
                    if (!textBox.Focused) {
                        textBox.Focus();
                    }
                    contextMenu.MenuItems[0].Enabled = textBox.SelectionLength > 0;
                    contextMenu.MenuItems[2].Enabled = textBox.SelectionLength < textBox.TextLength;
                });
                textBoxOutput.ContextMenu = contextMenu;
            }));
        }

        private async void InitializePrintAsync() {
            await Task.Run(new Action(() => {
                printDialog = new PrintDialog();
                printDocument = new PrintDocument();
                printDocument.DocumentName = new StringBuilder()
                    .Append(Program.GetTitle())
                    .Append(Constants.Space)
                    .Append(Properties.Resources.CaptionPrintOutput)
                    .ToString();
                printDocument.BeginPrint += new PrintEventHandler(BeginPrint);
                printDocument.PrintPage += new PrintPageEventHandler(PrintPage);
                printDocument.EndPrint += new PrintEventHandler(EndPrint);
                printDialog.Document = printDocument;
                printAction = PrintAction.PrintToPrinter;
                printPreviewDialog = new PrintPreviewDialog() {
                    ShowIcon = false,
                    UseAntiAlias = true,
                    Document = printDocument
                };
                printPreviewDialog.WindowState = FormWindowState.Normal;
                printPreviewDialog.StartPosition = FormStartPosition.WindowsDefaultBounds;
                printPreviewDialog.HelpRequested += new HelpEventHandler(OpenHelp);
            }));
        }

        private async void InitializeSaveFileDialogAsync() {
            await Task.Run(new Action(() => {
                string initialDirectory = string.IsNullOrEmpty(settings.LastExportDirectory)
                    ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    : settings.LastExportDirectory;
                saveFileDialog = new SaveFileDialog() {
                    AddExtension = true,
                    CheckPathExists = true,
                    InitialDirectory = initialDirectory,
                    OverwritePrompt = true,
                    ValidateNames = true
                };
                saveFileDialog.HelpRequest += new EventHandler(OpenHelp);
            }));
        }

        private void InitializeStatusStripHandler() {
            statusStripHandler = new StatusStripHandler(statusStrip, StatusStripHandler.DisplayMode.Basic, settings);
        }

        private async void InitializeUpdateCheckerAsync() {
            await Task.Run(new Action(() => {
                updateChecker = new UpdateChecker(settings);
                updateChecker.Parent = this;
                updateChecker.StateChanged += new EventHandler<UpdateCheckEventArgs>(OnUpdateCheckerStateChanged);
                updateChecker.Help += new HelpEventHandler(OpenHelp);
            }));
        }

        private void EncodeRFC1738(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            textBoxOutput.Clear();
            try {
                textBoxOutput.Text = WebUtility.UrlEncode(textBoxInput.Text);
                statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.Temporary);
            } catch (Exception exception) {
                ShowException(exception, true);
            } finally {
                Cursor = Cursors.Default;
            }
        }

        private void EncodeRFC3986(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            textBoxOutput.Clear();
            try {
                textBoxOutput.Text = Uri.EscapeDataString(textBoxInput.Text);
                statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.Temporary);
            } catch (Exception exception) {
                ShowException(exception, true);
            } finally {
                Cursor = Cursors.Default;
            }
        }

        private void EncodeBase64(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            textBoxOutput.Clear();
            try {
                textBoxOutput.Text = Convert.ToBase64String(Encoding.UTF8.GetBytes(textBoxInput.Text));
                statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.Temporary);
            } catch (Exception exception) {
                ShowException(exception, true);
            } finally {
                Cursor = Cursors.Default;
            }
        }

        private void ParseUrl(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            textBoxOutput.Clear();
            try {
                textBoxOutput.Text = ParseUrl(textBoxInput.Text.TrimStart(), UriFormat.UriEscaped);
                statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.Temporary);
            } catch (Exception exception) {
                ShowException(exception, false);
            } finally {
                Cursor = Cursors.Default;
            }
        }

        private void DecodeUrl(object sender, EventArgs e) {
            textBoxOutput.Clear();
            Cursor = Cursors.WaitCursor;
            try {
                textBoxOutput.Text = CheckUrl(textBoxInput)
                    ? ParseUrl(textBoxInput.Text.TrimStart(), UriFormat.Unescaped)
                    : WebUtility.UrlDecode(textBoxInput.Text);
                statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.Temporary);
            } catch (Exception exception) {
                ShowException(exception, false);
            } finally {
                Cursor = Cursors.Default;
            }
        }

        private void DecodeBase64(object sender, EventArgs e) {
            textBoxOutput.Clear();
            Cursor = Cursors.WaitCursor;
            try {
                textBoxOutput.Text = Encoding.UTF8.GetString(Convert.FromBase64String(textBoxInput.Text));
                statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.Temporary);
            } catch (Exception exception) {
                ShowException(exception, false);
            } finally {
                Cursor = Cursors.Default;
            }
        }

        private void ConvertToASCIIFull(object sender, EventArgs e) {
            textBoxOutput.Clear();
            Cursor = Cursors.WaitCursor;
            try {
                textBoxOutput.Text = ASCII.Convert(textBoxInput.Text, Encoding.UTF8, ASCII.ConversionOptions.Full);
            } catch (Exception exception) {
                ShowException(exception, true);
            } finally {
                Cursor = Cursors.Default;
            }
        }

        private void ConvertToASCIISafePath(object sender, EventArgs e) {
            textBoxOutput.Clear();
            Cursor = Cursors.WaitCursor;
            try {
                textBoxOutput.Text = ASCII.Convert(textBoxInput.Text, Encoding.UTF8, ASCII.ConversionOptions.SafePath);
            } catch (Exception exception) {
                ShowException(exception, true);
            } finally {
                Cursor = Cursors.Default;
            }
        }

        private void ConvertToASCIISafeFileName(object sender, EventArgs e) {
            textBoxOutput.Clear();
            Cursor = Cursors.WaitCursor;
            try {
                textBoxOutput.Text = ASCII.Convert(textBoxInput.Text, Encoding.UTF8, ASCII.ConversionOptions.SafeFileName);
            } catch (Exception exception) {
                ShowException(exception, true);
            } finally {
                Cursor = Cursors.Default;
            }
        }

        private void ConvertToASCIIAlphanumeric(object sender, EventArgs e) {
            textBoxOutput.Clear();
            Cursor = Cursors.WaitCursor;
            try {
                textBoxOutput.Text = ASCII.Convert(textBoxInput.Text, Encoding.UTF8, ASCII.ConversionOptions.Alphanumeric);
            } catch (Exception exception) {
                ShowException(exception, true);
            } finally {
                Cursor = Cursors.Default;
            }
        }

        private TextBox GetTextBoxEntered() {
            if (textBoxInput.ContainsFocus) {
                return textBoxInput;
            }
            if (textBoxOutput.ContainsFocus) {
                return textBoxOutput;
            } else {
                return textBoxEntered;
            }
        }

        private void OnEnter(object sender, EventArgs e) => textBoxEntered = (TextBox)sender;

        private void OnFormActivated(object sender, EventArgs e) {
            if (dialog != null) {
                dialog.Activate();
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e) {
            if (!Menu.MenuItems[0].MenuItems[3].Enabled && e.CloseReason.Equals(CloseReason.UserClosing)) {
                dialog = new MessageForm(this, Properties.Resources.MessageQuestionCancelPrinting, Properties.Resources.CaptionQuestion,
                    MessageForm.Buttons.YesNo, MessageForm.BoxIcon.Question, MessageForm.DefaultButton.Button2);
                dialog.HelpRequested += new HelpEventHandler(OpenHelp);
                if (dialog.ShowDialog(this).Equals(DialogResult.Yes)) {
                    cancel = true;
                } else {
                    e.Cancel = true;
                    return;
                }
            }
            settings.Dispose();
            statusStripHandler.Dispose();
            updateChecker.Dispose();
            textBoxClicksTimer.Dispose();
        }

        private void OnFormLoad(object sender, EventArgs e) => statusStripHandler.SetZoomLevel(0);

        private void Export(object sender, EventArgs e) {
            try {
                saveFileDialog.Filter = fileExtensionFilter.GetFilter();
                saveFileDialog.FilterIndex = fileExtensionFilter.GetFilterIndex();
                saveFileDialog.Title = Properties.Resources.CaptionExport;
                saveFileDialog.FileName = Application.ProductName + Properties.Resources.CaptionExport;
                if (saveFileDialog.ShowDialog(this).Equals(DialogResult.OK)) {
                    statusStripHandler.SetMessage(
                        StatusStripHandler.StatusMessageType.Temporary,
                        Properties.Resources.MessageExporting);
                    StaticMethods.ExportAsImage(splitContainer, saveFileDialog.FileName);
                    fileExtensionFilter.SetFilterIndex(saveFileDialog.FilterIndex);
                    settings.ExtensionFilterIndex = saveFileDialog.FilterIndex;
                    settings.LastExportDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                    statusStripHandler.SetMessage(
                        StatusStripHandler.StatusMessageType.Temporary,
                        Properties.Resources.MessageExportFinished);
                }
            } catch (Exception exception) {
                ShowException(exception, Properties.Resources.MessageExportFailed, true);
            }
        }

        private void PrintPreview(object sender, EventArgs e) {
            printType = PrintType.Text;
            try {
                dialog = printPreviewDialog;
                dialog.WindowState = WindowState.Equals(FormWindowState.Minimized) ? FormWindowState.Normal : WindowState;
                if (printPreviewDialog.ShowDialog(this).Equals(DialogResult.OK)) {
                    printDocument.Print();
                }
            } catch (Exception exception) {
                ShowException(exception, Properties.Resources.MessagePrintingFailed, true);
            }
        }

        private void Print(object sender, EventArgs e) {
            printType = PrintType.Text;
            try {
                printDialog.AllowCurrentPage = true;
                printDialog.AllowPrintToFile = true;
                printDialog.AllowSelection = true;
                printDialog.AllowSomePages = true;
                printDialog.ShowHelp = true;
                printDialog.UseEXDialog = true;
                if (printDialog.ShowDialog(this).Equals(DialogResult.OK)) {
                    printDocument.Print();
                }
            } catch (Exception exception) {
                ShowException(exception, Properties.Resources.MessagePrintingFailed, true);
            }
        }

        private void PrintImagePreview(object sender, EventArgs e) {
            printType = PrintType.Image;
            try {
                dialog = printPreviewDialog;
                dialog.WindowState = WindowState.Equals(FormWindowState.Minimized) ? FormWindowState.Normal : WindowState;
                if (printPreviewDialog.ShowDialog(this).Equals(DialogResult.OK)) {
                    printDocument.Print();
                }
            } catch (Exception exception) {
                ShowException(exception, Properties.Resources.MessagePrintingFailed, true);
            }
        }

        private void PrintImage(object sender, EventArgs e) {
            printType = PrintType.Image;
            try {
                printDialog.AllowCurrentPage = false;
                printDialog.AllowPrintToFile = true;
                printDialog.AllowSelection = false;
                printDialog.AllowSomePages = false;
                printDialog.ShowHelp = true;
                printDialog.UseEXDialog = true;
                if (printDialog.ShowDialog(this).Equals(DialogResult.OK)) {
                    printDocument.Print();
                }
            } catch (Exception exception) {
                ShowException(exception, Properties.Resources.MessagePrintingFailed, true);
            }
        }

        private void ShowException(Exception exception, bool log) => ShowException(exception, null, log);

        private void ShowException(Exception exception, string statusMessage, bool log) {
            Debug.WriteLine(exception);
            if (log) {
                ErrorLog.WriteLine(exception);
            }
            statusStripHandler.SetMessage(
                StatusStripHandler.StatusMessageType.PersistentB,
                string.IsNullOrEmpty(statusMessage) ? exception.Message : statusMessage);
            StringBuilder title = new StringBuilder()
                .Append(Program.GetTitle())
                .Append(Constants.Space)
                .Append(Constants.EnDash)
                .Append(Constants.Space)
                .Append(Properties.Resources.CaptionError);
            dialog = new MessageForm(this, exception.Message, title.ToString(), MessageForm.Buttons.OK, MessageForm.BoxIcon.Error);
            dialog.HelpRequested += new HelpEventHandler(OpenHelp);
            dialog.ShowDialog(this);
            statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.PersistentB);
            statusStripHandler.SetMessage(
                StatusStripHandler.StatusMessageType.Temporary,
                string.IsNullOrEmpty(statusMessage) ? exception.Message : statusMessage);
        }

        private void BeginPrint(object sender, PrintEventArgs e) {
            try {
                printFont = textBoxInput.Font;
                availableWidth = 0;
                availableHeight = 0;
                availableSize = Size.Empty;
                doEvents = 0;
                stringToPrint = null;
                printAction = e.PrintAction;
                printDocument.OriginAtMargins = settings.PrintSoftMargins;
                statusStripHandler.SetMessage(
                    StatusStripHandler.StatusMessageType.Temporary, e.PrintAction.Equals(PrintAction.PrintToPreview)
                        ? Properties.Resources.MessageGeneratingPreview
                        : Properties.Resources.MessagePrinting);
                Cursor = Cursors.WaitCursor;
                for (int i = 0; i < 6; i++) {
                    Menu.MenuItems[0].MenuItems[i].Enabled = false;
                }
                Menu.MenuItems[3].MenuItems[1].Enabled = false;
                Menu.MenuItems[3].MenuItems[3].Enabled = false;
                Menu.MenuItems[2].MenuItems[0].Enabled = false;
                topMost = TopMost;
                if (TopMost) {
                    TopMost = false;
                    Menu.MenuItems[2].MenuItems[0].Checked = false;
                }
                e.Cancel = cancel;
            } catch (Exception exception) {
                ShowException(exception, Properties.Resources.MessagePrintingFailed, true);
            }
        }

        private void PrintPage(object sender, PrintPageEventArgs e) {
            int charactersOnPage = 0;
            int linesPerPage = 0;
            if (availableWidth.Equals(0)) {
                availableWidth = (int)Math.Floor(printDocument.OriginAtMargins
                    ? e.MarginBounds.Width
                    : e.PageSettings.Landscape ? e.PageSettings.PrintableArea.Height : e.PageSettings.PrintableArea.Width);
            }
            if (availableHeight.Equals(0)) {
                availableHeight = (int)Math.Floor(printDocument.OriginAtMargins
                    ? e.MarginBounds.Height
                    : e.PageSettings.Landscape ? e.PageSettings.PrintableArea.Width : e.PageSettings.PrintableArea.Height);
            }
            if (availableSize.Equals(Size.Empty)) {
                availableSize = new Size(availableWidth, availableHeight);
            }
            if (printAction.Equals(PrintAction.PrintToPreview)) {
                e.Graphics.TranslateTransform(e.PageSettings.PrintableArea.X, e.PageSettings.PrintableArea.Y);
            }
            int strLengthToMeasure = availableWidth * availableHeight / 50;
            string stringToMeasure = string.Empty;
            e.Cancel = cancel;
            switch (printType) {
                case PrintType.Image:
                    using (Bitmap bitmap = new Bitmap(splitContainer.Width, splitContainer.Height, PixelFormat.Format32bppArgb)) {
                        splitContainer.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                        bool rotate = StaticMethods.IsGraphicsRotationNeeded(bitmap.Size, availableSize);
                        if (rotate) {
                            e.Graphics.RotateTransform(90, MatrixOrder.Prepend);
                        }
                        Size size = StaticMethods.GetNewGraphicsSize(bitmap.Size, availableSize);
                        e.Graphics.DrawImage(bitmap, 0, rotate ? -availableWidth : 0, size.Width, size.Height);
                        e.HasMorePages = false;
                    }
                    break;
                default:
                    if (stringToPrint == null) {
                        string header = Constants.PrintOutputInput + Constants.Colon;
                        string separator = string.Empty.PadRight(header.Length, Constants.EqualSign);
                        StringBuilder stringBuilder = new StringBuilder()
                            .AppendLine(separator)
                            .AppendLine(header)
                            .AppendLine(separator);
                        string body = textBoxInput.Text.TrimEnd();
                        if (!string.IsNullOrEmpty(body)) {
                            stringBuilder.AppendLine(body);
                        }
                        header = Constants.PrintOutputOutput + Constants.Colon;
                        separator = string.Empty.PadRight(header.Length, Constants.EqualSign);
                        stringBuilder.AppendLine()
                            .AppendLine(separator)
                            .AppendLine(header)
                            .AppendLine(separator);
                        body = textBoxOutput.Text.TrimEnd();
                        if (!string.IsNullOrEmpty(body)) {
                            stringBuilder.AppendLine(body);
                        }
                        stringToPrint = stringBuilder.ToString();
                    }

                    e.Graphics.MeasureString(
                        stringToPrint.Length > strLengthToMeasure ? stringToPrint.Substring(0, strLengthToMeasure) : stringToPrint,
                        printFont,
                        availableSize,
                        StringFormat.GenericTypographic,
                        out charactersOnPage,
                        out linesPerPage);

                    e.Graphics.DrawString(
                        stringToPrint.Substring(0, charactersOnPage),
                        printFont,
                        Brushes.Black,
                        new Rectangle(Point.Empty, availableSize),
                        StringFormat.GenericTypographic);

                    stringToPrint = stringToPrint.Substring(charactersOnPage);
                    e.HasMorePages = stringToPrint.Length > 0;
                    if (++doEvents % 50 == 0) {
                        Application.DoEvents();
                    }
                    break;
            }
            e.Cancel = cancel;
        }

        private void EndPrint(object sender, PrintEventArgs e) {
            statusStripHandler.SetMessage(
                StatusStripHandler.StatusMessageType.Temporary,
                e.PrintAction.Equals(PrintAction.PrintToPreview)
                    ? Properties.Resources.MessagePreviewGenerated
                    : Properties.Resources.MessagePrintingFinished);
            try {
                Menu.MenuItems[3].MenuItems[1].Enabled = true;
                Menu.MenuItems[3].MenuItems[3].Enabled = true;
                for (int i = 0; i < 6; i++) {
                    Menu.MenuItems[0].MenuItems[i].Enabled = true;
                }
                if (topMost) {
                    TopMost = true;
                    Menu.MenuItems[2].MenuItems[0].Checked = true;
                }
                Menu.MenuItems[2].MenuItems[0].Enabled = true;
                Cursor = Cursors.Default;
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        private void EnableControls() {
            bool enabled = textBoxInput.TextLength > 0;
            buttonRFC1738.Enabled = enabled;
            buttonRFC3986.Enabled = enabled;
            buttonEncodeBase64.Enabled = enabled;
            buttonParseUrl.Enabled = enabled;
            buttonDecodeUrl.Enabled = enabled;
            buttonDecodeBase64.Enabled = enabled;
            buttonFull.Enabled = enabled;
            buttonSafePath.Enabled = enabled;
            buttonSafeFileName.Enabled = enabled;
            buttonAlphanumeric.Enabled = enabled;
            buttonClearInput.Enabled = enabled;
            buttonClearOutput.Enabled = textBoxOutput.TextLength > 0;
            buttonFlip.Enabled = enabled || buttonClearOutput.Enabled;
            Menu.MenuItems[1].MenuItems[9].Enabled = enabled;
            Menu.MenuItems[1].MenuItems[10].Enabled = enabled;
            Menu.MenuItems[1].MenuItems[11].Enabled = enabled;
            Menu.MenuItems[1].MenuItems[13].Enabled = enabled;
            Menu.MenuItems[1].MenuItems[14].Enabled = enabled;
            Menu.MenuItems[1].MenuItems[15].Enabled = enabled;
            Menu.MenuItems[1].MenuItems[17].Enabled = enabled;
            Menu.MenuItems[1].MenuItems[18].Enabled = enabled;
            Menu.MenuItems[1].MenuItems[19].Enabled = enabled;
            Menu.MenuItems[1].MenuItems[20].Enabled = enabled;
            Menu.MenuItems[1].MenuItems[22].Enabled = enabled;
            Menu.MenuItems[1].MenuItems[24].Enabled = buttonClearOutput.Enabled;
            Menu.MenuItems[1].MenuItems[23].Enabled = enabled || buttonClearOutput.Enabled;
        }

        private void Undo(object sender, EventArgs e) {
            if (textBoxInput.CanUndo) {
                textBoxInput.Undo();
            } else if (!textBoxInput.ReadOnly) {
                if (!string.IsNullOrEmpty(undoneInput)) {
                    textBoxInput.Text = undoneInput;
                    textBoxInput.SelectAll();
                    undone = true;
                } else if (undone) {
                    if (textBoxInput.TextLength > 0) {
                        string str = textBoxInput.Text;
                        textBoxInput.Clear();
                        undoneInput = str;
                    }
                    undone = false;
                }
            }
        }

        private void Cut(object sender, EventArgs e) {
            TextBox textBox = GetTextBoxEntered();
            if (textBox != null) {
                textBox.Cut();
            }
        }

        private void Copy(object sender, EventArgs e) {
            TextBox textBox = GetTextBoxEntered();
            if (textBox != null) {
                textBox.Copy();
                if (textBox.SelectionLength > 0) {
                    statusStripHandler.SetMessage(
                        StatusStripHandler.StatusMessageType.Temporary,
                        Properties.Resources.MessageCopiedToClipboard);
                }
            }
        }

        private void Paste(object sender, EventArgs e) {
            TextBox textBox = GetTextBoxEntered();
            if (textBox != null) {
                textBox.Paste();
            }
        }

        private void Delete(object sender, EventArgs e) {
            if (GetTextBoxEntered() != null) {
                SendKeys.Send(Constants.SendKeysDelete);
            }
        }

        private void SelectAll(object sender, EventArgs e) {
            TextBox textBox = GetTextBoxEntered();
            if (textBox != null) {
                textBox.SelectAll();
                statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.Temporary);
            }
        }

        private void ClearInput(object sender, EventArgs e) {
            if (textBoxInput.TextLength > 0) {
                string str = textBoxInput.Text;
                textBoxInput.Clear();
                undoneInput = str;
                textBoxInput.Focus();
                statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.Temporary);
            }
        }

        private void Flip(object sender, EventArgs e) {
            if (textBoxInput.TextLength > 0 || textBoxOutput.TextLength > 0) {
                TextBox textBox = GetTextBoxEntered() ?? textBoxInput;
                string str = textBoxInput.Text;
                textBoxInput.Text = textBoxOutput.Text;
                textBoxOutput.Text = str;
                textBox.Focus();
                textBoxInput.SelectAll();
                statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.Temporary);
            }
        }

        private void ClearOutput(object sender, EventArgs e) {
            if (textBoxOutput.TextLength > 0) {
                textBoxOutput.Clear();
                textBoxOutput.Focus();
                statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.Temporary);
            }
        }

        private void FocusInput(object sender, EventArgs e) {
            textBoxInput.Focus();
            textBoxInput.SelectAll();
        }

        private void FocusOutput(object sender, EventArgs e) {
            textBoxOutput.Focus();
            textBoxOutput.SelectAll();
        }

        private void ToggleTopMost(object sender, EventArgs e) {
            Menu.MenuItems[2].MenuItems[0].Checked = !Menu.MenuItems[2].MenuItems[0].Checked;
            TopMost = Menu.MenuItems[2].MenuItems[0].Checked;
            if (!TopMost) {
                SendToBack();
            }
        }

        private void ToggleWordWrap(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            try {
                textBoxInput.WordWrap = !textBoxInput.WordWrap;
                textBoxOutput.WordWrap = textBoxInput.WordWrap;
                Menu.MenuItems[2].MenuItems[2].Checked = textBoxInput.WordWrap;
            } catch (OutOfMemoryException exception) {
                ShowException(exception, Properties.Resources.MessageOutOfMemoryError, true);
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            } finally {
                Cursor = Cursors.Default;
            }
        }

        private void CheckUpdates(object sender, EventArgs e) => updateChecker.Check(UpdateChecker.CheckType.User);

        private void ShowAbout(object sender, EventArgs e) {
            dialog = new AboutForm();
            dialog.HelpRequested += new HelpEventHandler(OpenHelp);
            dialog.ShowDialog(this);
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
                    ShowException(exception, true);
                }
            }
        }

        private void OpenHelp(object sender, HelpEventArgs e) => OpenHelp(sender, (EventArgs)e);

        private void OnInputTextChanged(object sender, EventArgs e) {
            undoneInput = null;
            EnableControls();
        }

        private void OnOutputTextChanged(object sender, EventArgs e) => EnableControls();

        private void OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Alt && e.Shift && e.Control && e.KeyCode.Equals(Keys.P)) {
                if (Menu.MenuItems[0].MenuItems[7].Enabled) {
                    e.SuppressKeyPress = true;
                    PrintImage(sender, e);
                }
            } else if (e.Shift && e.Control && e.KeyCode.Equals(Keys.P)) {
                if (Menu.MenuItems[0].MenuItems[5].Enabled) {
                    e.SuppressKeyPress = true;
                    PrintPreview(sender, e);
                }
            } else if (e.Control && e.KeyCode.Equals(Keys.A)) {
                e.SuppressKeyPress = true;
                SelectAll(sender, e);
            } else if (e.Control && e.KeyCode.Equals(Keys.C)) {
                e.SuppressKeyPress = true;
                Copy(sender, e);
            } else if (e.Control && e.KeyCode.Equals(Keys.Z)) {
                e.SuppressKeyPress = true;
                Undo(sender, e);
            } else {
                statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.Temporary);
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e) {
            TextBox textBox = (TextBox)sender;
            if (IsKeyLocked(Keys.Insert)
                    && !char.IsControl(e.KeyChar)
                    && !textBox.ReadOnly
                    && textBox.SelectionLength.Equals(0)
                    && textBox.SelectionStart < textBox.TextLength) {

                int selectionStart = textBox.SelectionStart;
                StringBuilder stringBuilder = new StringBuilder(textBox.Text);
                stringBuilder[textBox.SelectionStart] = e.KeyChar;
                e.Handled = true;
                textBox.Text = stringBuilder.ToString();
                textBox.SelectionStart = selectionStart + 1;
            }
        }

        /// <summary>
        /// Modified version of OnMouseDown common method. Do not use anywhere else.
        /// </summary>
        private void OnMouseDown(object sender, MouseEventArgs e) {
            statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.Temporary);
            if (!e.Button.Equals(MouseButtons.Left)) {
                textBoxClicks = 0;
                return;
            }
            TextBox textBox = (TextBox)sender;
            textBoxClicksTimer.Stop();
            if (textBox.SelectionLength > 0) {
                textBoxClicks = 2;
            } else if (textBoxClicks.Equals(0) || Math.Abs(e.X - location.X) < 2 && Math.Abs(e.Y - location.Y) < 2) {
                textBoxClicks++;
            } else {
                textBoxClicks = 0;
            }
            location = e.Location;
            if (textBoxClicks.Equals(3)) {
                textBoxClicks = 0;
                NativeMethods.MouseEvent(
                    Constants.MOUSEEVENTF_LEFTUP,
                    Convert.ToUInt32(Cursor.Position.X),
                    Convert.ToUInt32(Cursor.Position.Y),
                    0,
                    0);
                Application.DoEvents();
                if (textBox.Multiline) {
                    char[] chars = textBox.Text.ToCharArray();
                    int selectionEnd = Math.Min(
                        Array.IndexOf(chars, Constants.CarriageReturn, textBox.SelectionStart),
                        Array.IndexOf(chars, Constants.LineFeed, textBox.SelectionStart));
                    if (selectionEnd < 0) {
                        selectionEnd = textBox.TextLength;
                    }
                    selectionEnd = Math.Max(textBox.SelectionStart + textBox.SelectionLength, selectionEnd);
                    int selectionStart = Math.Min(textBox.SelectionStart, selectionEnd);
                    while (--selectionStart > 0
                        && !chars[selectionStart].Equals(Constants.LineFeed)
                        && !chars[selectionStart].Equals(Constants.CarriageReturn)) { }
                    textBox.Select(selectionStart, selectionEnd - selectionStart);
                } else {
                    textBox.SelectAll();
                }
                textBox.Focus();
            } else {
                textBoxClicksTimer.Start();
            }
        }

        private void OnUpdateCheckerStateChanged(object sender, UpdateCheckEventArgs e) {
            statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.PersistentB, e.Message);
            if (dialog == null || !dialog.Visible) {
                dialog = e.Dialog;
            }
        }

        public static bool CheckUrl(TextBox textBox) {
            if (textBox.TextLength <= 3072) {
                string trimmed = textBox.Text.TrimStart();
                if (trimmed.Length <= 2048) {
                    try {
                        new Uri(trimmed);
                        return true;
                    } catch (Exception exception) {
                        Debug.WriteLine(exception);
                    }
                }
            }
            return false;
        }

        private static string Format(List<NameValuePair> list, char glue) {
            int maxNameLength = 0;
            foreach (NameValuePair nameValuePair in list) {
                maxNameLength = Math.Max(nameValuePair.name.Length, maxNameLength);
            }
            maxNameLength += glue.Equals(Constants.Colon) ? 2 : 1;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (NameValuePair nameValuePair in list) {
                if (glue.Equals(Constants.Colon)) {
                    stringBuilder.Append((nameValuePair.name + glue).PadRight(maxNameLength));
                } else {
                    stringBuilder.Append(nameValuePair.name.PadRight(maxNameLength));
                    stringBuilder.Append(glue);
                    stringBuilder.Append(Constants.Space);
                }
                stringBuilder.AppendLine(nameValuePair.value);
            }
            return stringBuilder.ToString();
        }

        private static List<NameValuePair> ParseQueryString(string queryString) {
            List<NameValuePair> list = new List<NameValuePair>();
            foreach (string item in queryString.Split(Constants.Ampersand)) {
                string[] nameValue = item.Split(new char[] { Constants.EqualSign }, 2);
                list.Add(new NameValuePair(nameValue[0], nameValue.Length > 1 ? nameValue[1] : string.Empty));
            }
            return list;
        }

        private static string ParseUrl(string url, UriFormat uriFormat) {
            Uri uri = new Uri(url);
            List<NameValuePair> list = new List<NameValuePair>();
            if (!string.IsNullOrEmpty(uri.Scheme)) {
                list.Add(new NameValuePair(Constants.UriScheme, uri.GetComponents(UriComponents.Scheme, uriFormat)));
            }
            if (!string.IsNullOrEmpty(uri.UserInfo)) {
                string[] userInfo = uri
                    .GetComponents(UriComponents.UserInfo, uriFormat)
                    .Split(new char[] { Constants.Colon }, 2);
                if (userInfo.Length > 1) {
                    list.Add(new NameValuePair(Constants.UriUserName, userInfo[0]));
                    list.Add(new NameValuePair(Constants.UriPassword, userInfo[1]));
                } else {
                    list.Add(new NameValuePair(Constants.UriUserInfo, userInfo[0]));
                }
            }
            if (!string.IsNullOrEmpty(uri.Host)) {
                list.Add(new NameValuePair(Constants.UriHost, uri.GetComponents(UriComponents.Host, uriFormat)));
            }
            if (uri.Port >= 0) {
                list.Add(new NameValuePair(Constants.UriPort, uri.GetComponents(UriComponents.StrongPort, uriFormat)));
            }
            StringBuilder stringBuilder;
            if (!string.IsNullOrEmpty(uri.AbsolutePath)) {
                string path = uri.GetComponents(UriComponents.Path, uriFormat);
                stringBuilder = new StringBuilder();
                if (string.IsNullOrEmpty(path)) {
                    stringBuilder.Append(uri.AbsolutePath);
                } else {
                    char[] absolutePath = uri.AbsolutePath.ToCharArray();
                    char[] pathCharArray = path.ToCharArray();
                    if (!absolutePath[0].Equals(pathCharArray[0]) && (absolutePath[0].Equals(Path.DirectorySeparatorChar)
                            || absolutePath[0].Equals(Path.AltDirectorySeparatorChar))) {

                        stringBuilder.Append(absolutePath[0]);
                    }
                    stringBuilder.Append(path);
                }
                list.Add(new NameValuePair(Constants.UriPath, stringBuilder.ToString()));
            }
            if (!string.IsNullOrEmpty(uri.Query)) {
                list.Add(new NameValuePair(Constants.UriQuery, uri.GetComponents(UriComponents.Query, uriFormat)));
            }
            if (!string.IsNullOrEmpty(uri.Fragment)) {
                list.Add(new NameValuePair(Constants.UriFragment, uri.GetComponents(UriComponents.Fragment, uriFormat)));
            }
            stringBuilder = new StringBuilder(Format(list, Constants.Colon));
            if (!string.IsNullOrEmpty(uri.Query)) {
                stringBuilder.AppendLine()
                    .Append(Constants.UriVariables)
                    .Append(Constants.Colon)
                    .AppendLine()
                    .Append(Format(ParseQueryString(uri.GetComponents(UriComponents.Query, uriFormat)), Constants.EqualSign));
            }
            return stringBuilder.ToString();
        }

        private enum PrintType {
            Text,
            Image
        }
    }
}
