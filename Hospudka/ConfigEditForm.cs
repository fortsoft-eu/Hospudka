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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hospudka {
    public partial class ConfigEditForm : Form {
        private bool cancel, dirty, invertDirection, lastMatch, searching, userClosing;
        private CountDownForm countDownForm;
        private FileExtensionFilter fileExtensionFilter;
        private FindForm findForm;
        private Form dialog;
        private Font printFont;
        private int availableHeight, availableWidth, doEvents, matchIndex, textBoxClicks;
        private List<SearchResult> searchResults;
        private PersistWindowState persistWindowState;
        private Point location;
        private PrintAction printAction;
        private PrintDialog printDialog;
        private PrintDocument printDocument;
        private PrintPreviewDialog printPreviewDialog;
        private PrintType printType;
        private SaveFileDialog saveFileDialog;
        private Search search;
        private Settings settings;
        private Size availableSize;
        private StatusStripHandler statusStripHandler;
        private string stringToPrint;
        private Thread findThread, turnOffThread;
        private System.Windows.Forms.Timer textBoxClicksTimer;
        private UpdateChecker updateChecker;

        public ConfigEditForm(Settings settings) {
            this.settings = settings;

            Icon = Properties.Resources.Config;
            Text = new StringBuilder()
                .Append(Program.GetTitle())
                .Append(Constants.Space)
                .Append(Constants.EnDash)
                .Append(Constants.Space)
                .Append(Properties.Resources.CaptionEditRemoteConfig)
                .ToString();

            textBoxClicksTimer = new System.Windows.Forms.Timer();
            textBoxClicksTimer.Interval = SystemInformation.DoubleClickTime;
            textBoxClicksTimer.Tick += new EventHandler((sender, e) => {
                textBoxClicksTimer.Stop();
                textBoxClicks = 0;
            });

            fileExtensionFilter = new FileExtensionFilter(settings.ExtensionFilterIndex);
            persistWindowState = new PersistWindowState();
            persistWindowState.DetectionOptions = PersistWindowState.WindowDetectionOptions.NoDetection;
            persistWindowState.Parent = this;

            InitializePrintAsync();
            InitializeUpdateCheckerAsync();
            InitializeSaveFileDialogAsync();
            BuildMainMenuAsync();

            InitializeComponent();

            BuildContextMenuAsync();
            InitializeStatusStripHandler();

            textBox.Font = new Font(Constants.MonospaceFontName, 10, FontStyle.Regular, GraphicsUnit.Point, 238);
            textBox.Text = settings.Config;
            textBox.TextChanged += new EventHandler((sender, e) => {
                if (!dirty) {
                    Text += Constants.Space.ToString() + Constants.Asterisk.ToString();
                }
                dirty = true;
            });
        }

        private async void BuildMainMenuAsync() {
            await Task.Run(new Action(() => {
                MainMenu mainMenu = new MainMenu();
                MenuItem menuItemFile = mainMenu.MenuItems.Add(Properties.Resources.MenuItemFile);
                MenuItem menuItemEdit = mainMenu.MenuItems.Add(Properties.Resources.MenuItemEdit);
                MenuItem menuItemOptions = mainMenu.MenuItems.Add(Properties.Resources.MenuItemOptions);
                MenuItem menuItemTools = mainMenu.MenuItems.Add(Properties.Resources.MenuItemTools);
                MenuItem menuItemHelp = mainMenu.MenuItems.Add(Properties.Resources.MenuItemHelp);
                menuItemFile.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemSave,
                    new EventHandler(Save), Shortcut.CtrlS));
                menuItemFile.MenuItems.Add(Constants.Hyphen.ToString());
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
                    new EventHandler((sender, e) => textBox.Undo()), Shortcut.CtrlZ));
                menuItemEdit.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemCut,
                    new EventHandler((sender, e) => textBox.Cut()), Shortcut.CtrlX));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemCopy,
                    new EventHandler(Copy), Shortcut.CtrlC));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemPaste,
                    new EventHandler((sender, e) => textBox.Paste()), Shortcut.CtrlV));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemDelete + Constants.ShortcutDelete,
                    new EventHandler((sender, e) => SendKeys.Send(Constants.SendKeysDelete))));
                menuItemEdit.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemSelectAll,
                    new EventHandler(SelectAll), Shortcut.CtrlA));
                menuItemEdit.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemFind,
                    new EventHandler(Find), Shortcut.CtrlF));
                menuItemEdit.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemFindNext,
                    new EventHandler(FindNext), Shortcut.F3));
                menuItemOptions.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemWordWrap + Constants.ShortcutShiftCtrlL,
                    new EventHandler(ToggleWordWrap)));
                menuItemTools.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemAnalyzeUrl,
                    new EventHandler(AnalyzeUrl)));
                menuItemTools.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemTools.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemEncoderDecoder + Constants.ShortcutShiftCtrlN,
                    new EventHandler(OpenEncoderDecoder)));
                menuItemTools.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemTools.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemTurnOffMonitors,
                    new EventHandler(TurnOffMonitors), Shortcut.F11));
                menuItemTools.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemTools.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemLaunchCalculator,
                    new EventHandler(LaunchCalculator), Shortcut.AltF11));
                menuItemTools.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemLaunchNotepad,
                    new EventHandler(LaunchNotepad), Shortcut.AltF12));
                menuItemTools.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemLaunchWordPad,
                    new EventHandler(LaunchWordPad)));
                menuItemTools.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemLaunchCharMap,
                    new EventHandler(LaunchCharMap)));
                menuItemHelp.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemOnlineHelp,
                    new EventHandler(OpenHelp), Shortcut.F1));
                menuItemHelp.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemCheckForUpdates,
                    new EventHandler(CheckUpdates)));
                menuItemHelp.MenuItems.Add(Constants.Hyphen.ToString());
                menuItemHelp.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemAbout,
                    new EventHandler(ShowAbout)));
                Menu = mainMenu;
                menuItemFile.Popup += new EventHandler((sender, e) => MenuItemFileSetEnabled());
                menuItemEdit.Popup += new EventHandler((sender, e) => MenuItemEditSetEnabled());
                menuItemTools.Popup += new EventHandler((sender, e) =>
                    menuItemTools.MenuItems[0].Enabled = StaticMethods.CheckSelectedUrl(textBox));
                menuItemHelp.Popup += new EventHandler((sender, e) => MenuItemHelpSetEnabled());
            }));
        }

        private async void BuildContextMenuAsync() {
            await Task.Run(new Action(() => {
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemUndo,
                    new EventHandler((sender, e) => textBox.Undo())));
                contextMenu.MenuItems.Add(Constants.Hyphen.ToString());
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemCut,
                    new EventHandler((sender, e) => textBox.Cut())));
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemCopy,
                    new EventHandler(Copy)));
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemPaste,
                    new EventHandler((sender, e) => textBox.Paste())));
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemDelete,
                    new EventHandler((sender, e) => SendKeys.Send(Constants.SendKeysDelete))));
                contextMenu.MenuItems.Add(Constants.Hyphen.ToString());
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemSelectAll,
                    new EventHandler(SelectAll)));
                contextMenu.MenuItems.Add(Constants.Hyphen.ToString());
                contextMenu.MenuItems.Add(new MenuItem(Properties.Resources.MenuItemAnalyzeUrl,
                    new EventHandler(AnalyzeUrl)));
                contextMenu.Popup += new EventHandler((sender, e) => {
                    if (!textBox.Focused) {
                        textBox.Focus();
                    }
                    contextMenu.MenuItems[0].Enabled = textBox.CanUndo;
                    bool enabled = textBox.SelectionLength > 0;
                    contextMenu.MenuItems[2].Enabled = enabled;
                    contextMenu.MenuItems[3].Enabled = enabled;
                    contextMenu.MenuItems[5].Enabled = enabled;
                    contextMenu.MenuItems[7].Enabled = textBox.SelectionLength < textBox.TextLength;
                    try {
                        contextMenu.MenuItems[4].Enabled = Clipboard.ContainsText();
                    } catch (Exception exception) {
                        contextMenu.MenuItems[4].Enabled = true;
                        Debug.WriteLine(exception);
                        ErrorLog.WriteLine(exception);
                    }
                    bool visible = StaticMethods.CheckSelectedUrl(textBox);
                    contextMenu.MenuItems[8].Visible = visible;
                    contextMenu.MenuItems[9].Visible = visible;
                });
                textBox.ContextMenu = contextMenu;
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

        private void MenuItemFileSetEnabled() {
            for (int i = 0; i < 8; i++) {
                Menu.MenuItems[0].MenuItems[i].Enabled = textBox.Enabled;
            }
        }

        private void MenuItemEditSetEnabled() {
            Menu.MenuItems[1].MenuItems[0].Enabled = textBox.Enabled && textBox.CanUndo;
            bool enabled = textBox.Enabled && textBox.SelectionLength > 0;
            Menu.MenuItems[1].MenuItems[2].Enabled = enabled;
            Menu.MenuItems[1].MenuItems[3].Enabled = enabled;
            Menu.MenuItems[1].MenuItems[5].Enabled = enabled;
            Menu.MenuItems[1].MenuItems[7].Enabled = textBox.Enabled && textBox.SelectionLength < textBox.TextLength;
            Menu.MenuItems[1].MenuItems[9].Enabled = textBox.Enabled;
            Menu.MenuItems[1].MenuItems[10].Enabled = textBox.Enabled && !string.IsNullOrEmpty(search.searchString);
            if (textBox.Enabled) {
                try {
                    Menu.MenuItems[1].MenuItems[4].Enabled = Clipboard.ContainsText();
                } catch (Exception exception) {
                    Menu.MenuItems[1].MenuItems[4].Enabled = true;
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                }
            } else {
                Menu.MenuItems[1].MenuItems[4].Enabled = false;
            }
        }

        private void MenuItemHelpSetEnabled() {
            Menu.MenuItems[4].MenuItems[1].Enabled = textBox.Enabled;
            Menu.MenuItems[4].MenuItems[3].Enabled = textBox.Enabled;
        }

        private async void SaveAsync() {
            EnableMenuItems(false);
            try {
                statusStripHandler.SetMessage(
                    StatusStripHandler.StatusMessageType.PersistentB,
                    Properties.Resources.MessageSaving);
                settings.Config = textBox.Text;
                if (await settings.SaveConfigAsync()) {
                    dirty = false;
                    Text = new StringBuilder()
                        .Append(Program.GetTitle())
                        .Append(Constants.Space)
                        .Append(Constants.EnDash)
                        .Append(Constants.Space)
                        .Append(Properties.Resources.CaptionEditRemoteConfig)
                        .ToString();
                    statusStripHandler.SetMessage(
                        StatusStripHandler.StatusMessageType.Temporary,
                        Properties.Resources.MessageSaved);
                } else {
                    userClosing = false;
                    statusStripHandler.SetMessage(
                        StatusStripHandler.StatusMessageType.PersistentB,
                        Properties.Resources.MessageSavingFailed);
                    StringBuilder title = new StringBuilder()
                        .Append(Program.GetTitle())
                        .Append(Constants.Space)
                        .Append(Constants.EnDash)
                        .Append(Constants.Space)
                        .Append(Properties.Resources.CaptionError);
                    dialog = new MessageForm(this, Properties.Resources.MessageSavingFailed, title.ToString(), MessageForm.Buttons.OK,
                        MessageForm.BoxIcon.Error);
                    dialog.HelpRequested += new HelpEventHandler(OpenHelp);
                    dialog.ShowDialog(this);
                    statusStripHandler.SetMessage(
                        StatusStripHandler.StatusMessageType.PersistentB);
                    statusStripHandler.SetMessage(
                        StatusStripHandler.StatusMessageType.Temporary,
                        Properties.Resources.MessageSavingFailed);
                }
            } catch (Exception exception) {
                userClosing = false;
                ShowException(exception, Properties.Resources.MessageSavingFailed);
            } finally {
                EnableMenuItems(true);
                if (userClosing) {
                    Close();
                }
            }
        }

        private void EnableMenuItems(bool enable) {
            textBox.Enabled = enable;
            MenuItemFileSetEnabled();
            MenuItemEditSetEnabled();
            MenuItemHelpSetEnabled();
            if (enable) {
                textBox.Focus();
            }
        }

        private void OnFormActivated(object sender, EventArgs e) {
            if (dialog != null) {
                dialog.Activate();
            }
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e) {
            if (InvokeRequired) {
                Invoke(new FormClosedEventHandler(OnFormClosed), sender, e);
            } else {
                textBox.HideSelection = true;
                statusStripHandler.ClearSearchResult();
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e) {
            if (!textBox.Enabled) {
                userClosing = true;
                e.Cancel = true;
                return;
            }
            if (!Menu.MenuItems[0].MenuItems[5].Enabled && e.CloseReason.Equals(CloseReason.UserClosing)) {
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
            if (dirty) {
                dialog = new MessageForm(this, Properties.Resources.MessageSaveChanges, null, MessageForm.Buttons.YesNoCancel,
                    MessageForm.BoxIcon.Question);
                dialog.HelpRequested += new HelpEventHandler(OpenHelp);
                DialogResult dialogResult = dialog.ShowDialog(this);
                if (dialogResult.Equals(DialogResult.Yes)) {
                    userClosing = true;
                    e.Cancel = true;
                    SaveAsync();
                    return;
                }
                if (dialogResult.Equals(DialogResult.Cancel)) {
                    e.Cancel = true;
                    return;
                }
            }
            settings.Dispose();
            if (findForm != null) {
                CloseFindForm();
            }
            statusStripHandler.Dispose();
            updateChecker.Dispose();
            textBoxClicksTimer.Dispose();
        }

        private void CloseFindForm() {
            findForm.AltCtrlShiftPPressed -= new EventHandler(PrintImage);
            findForm.AltF11Pressed -= new EventHandler(LaunchCalculator);
            findForm.AltF12Pressed -= new EventHandler(LaunchNotepad);
            findForm.CtrlEPressed -= new EventHandler(Export);
            findForm.CtrlPPressed -= new EventHandler(Print);
            findForm.CtrlShiftLPressed -= new EventHandler(ToggleWordWrap);
            findForm.CtrlShiftNPressed -= new EventHandler(OpenEncoderDecoder);
            findForm.CtrlShiftPPressed -= new EventHandler(PrintPreview);
            findForm.CtrlSPressed -= new EventHandler(Save);
            findForm.DownPressed -= new EventHandler(OnDownPressed);
            findForm.EndPressed -= new EventHandler(OnEndPressed);
            findForm.F11Pressed -= new EventHandler(TurnOffMonitors);
            findForm.Find -= new EventHandler<SearchEventArgs>(OnFind);
            findForm.FormClosed -= new FormClosedEventHandler(OnFormClosed);
            findForm.HelpRequested -= new HelpEventHandler(OpenHelp);
            findForm.HomePressed -= new EventHandler(OnHomePressed);
            findForm.PageDownPressed -= new EventHandler(OnPageDownPressed);
            findForm.PageUpPressed -= new EventHandler(OnPageUpPressed);
            findForm.UpPressed -= new EventHandler(OnUpPressed);
            if (findForm.Visible) {
                findForm.SafeClose();
            }
            findForm = null;
        }

        private void OnFormLoad(object sender, EventArgs e) {
            statusStripHandler.SetZoomLevel(0);
            textBox.Select(0, 0);
        }

        private void Save(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler(Save), sender, e);
            } else {
                SaveAsync();
            }
        }

        private void Export(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler(Export), sender, e);
            } else {
                try {
                    saveFileDialog.Filter = fileExtensionFilter.GetFilter();
                    saveFileDialog.FilterIndex = fileExtensionFilter.GetFilterIndex();
                    saveFileDialog.Title = Properties.Resources.CaptionExport;
                    saveFileDialog.FileName = Application.ProductName + Properties.Resources.CaptionExport;
                    if (saveFileDialog.ShowDialog(this).Equals(DialogResult.OK)) {
                        statusStripHandler.SetMessage(
                            StatusStripHandler.StatusMessageType.Temporary,
                            Properties.Resources.MessageExporting);
                        StaticMethods.ExportAsImage(textBox, saveFileDialog.FileName);
                        fileExtensionFilter.SetFilterIndex(saveFileDialog.FilterIndex);
                        settings.ExtensionFilterIndex = saveFileDialog.FilterIndex;
                        settings.LastExportDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                        statusStripHandler.SetMessage(
                            StatusStripHandler.StatusMessageType.Temporary,
                            Properties.Resources.MessageExportFinished);
                    }
                } catch (Exception exception) {
                    ShowException(exception, Properties.Resources.MessageExportFailed);
                }
            }
        }

        private void PrintPreview(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler(PrintPreview), sender, e);
            } else {
                printType = PrintType.Text;
                try {
                    dialog = printPreviewDialog;
                    dialog.WindowState = WindowState.Equals(FormWindowState.Minimized) ? FormWindowState.Normal : WindowState;
                    if (printPreviewDialog.ShowDialog(this).Equals(DialogResult.OK)) {
                        printDocument.Print();
                    }
                } catch (Exception exception) {
                    ShowException(exception, Properties.Resources.MessagePrintingFailed);
                }
            }
        }

        private void Print(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler(Print), sender, e);
            } else {
                printType = PrintType.Text;
                try {
                    printDialog.AllowCurrentPage = false;
                    printDialog.AllowPrintToFile = true;
                    printDialog.AllowSelection = true;
                    printDialog.AllowSomePages = true;
                    printDialog.ShowHelp = true;
                    printDialog.UseEXDialog = true;
                    if (printDialog.ShowDialog(this).Equals(DialogResult.OK)) {
                        printDocument.Print();
                    }
                } catch (Exception exception) {
                    ShowException(exception, Properties.Resources.MessagePrintingFailed);
                }
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
                ShowException(exception, Properties.Resources.MessagePrintingFailed);
            }
        }

        private void PrintImage(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler(PrintImage), sender, e);
            } else {
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
                    ShowException(exception, Properties.Resources.MessagePrintingFailed);
                }
            }
        }

        private void ShowException(Exception exception) => ShowException(exception, null);

        private void ShowException(Exception exception, string statusMessage) {
            Debug.WriteLine(exception);
            ErrorLog.WriteLine(exception);
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
            statusStripHandler.SetMessage(
                StatusStripHandler.StatusMessageType.PersistentB);
            statusStripHandler.SetMessage(
                StatusStripHandler.StatusMessageType.Temporary,
                string.IsNullOrEmpty(statusMessage) ? exception.Message : statusMessage);
        }

        private void BeginPrint(object sender, PrintEventArgs e) {
            printFont = textBox.Font;
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
            for (int i = 0; i < 8; i++) {
                Menu.MenuItems[0].MenuItems[i].Enabled = false;
            }
            Menu.MenuItems[4].MenuItems[1].Enabled = false;
            Menu.MenuItems[4].MenuItems[3].Enabled = false;
            e.Cancel = cancel;
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
                    using (Bitmap bitmap = new Bitmap(textBox.Width, textBox.Height, PixelFormat.Format32bppArgb)) {
                        textBox.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
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
                        stringToPrint = string.Empty;
                        switch (printDocument.PrinterSettings.PrintRange) {
                            case PrintRange.SomePages:
                                stringToMeasure = textBox.Text.Replace(Constants.VerticalTab.ToString(),
                                    string.Empty.PadRight(Constants.VerticalTabNumberOfSpaces)).TrimEnd();
                                StringBuilder stringBuilder = new StringBuilder();
                                for (int n = 1; stringToMeasure.Length > 0 && n <= printDocument.PrinterSettings.ToPage; n++) {
                                    e.Graphics.MeasureString(
                                        stringToMeasure.Length > strLengthToMeasure
                                            ? stringToMeasure.Substring(0, strLengthToMeasure)
                                            : stringToMeasure,
                                        printFont,
                                        availableSize,
                                        StringFormat.GenericTypographic,
                                        out charactersOnPage,
                                        out linesPerPage);

                                    if (n >= printDocument.PrinterSettings.FromPage) {
                                        stringBuilder.Append(stringToMeasure.Substring(0, charactersOnPage));
                                    }
                                    stringToMeasure = stringToMeasure.Substring(charactersOnPage);
                                    if (n % 100 == 0) {
                                        e.Cancel = cancel;
                                        Application.DoEvents();
                                    }
                                }
                                stringToPrint = stringBuilder.ToString();
                                break;
                            case PrintRange.Selection:
                                stringToPrint = textBox.SelectedText.Replace(Constants.VerticalTab.ToString(),
                                    string.Empty.PadRight(Constants.VerticalTabNumberOfSpaces)).TrimEnd();
                                break;
                            default:
                                stringToPrint = textBox.Text.Replace(Constants.VerticalTab.ToString(),
                                    string.Empty.PadRight(Constants.VerticalTabNumberOfSpaces)).TrimEnd();
                                break;
                        }
                    }

                    e.Graphics.MeasureString(
                        stringToPrint.Length > strLengthToMeasure
                            ? stringToPrint.Substring(0, strLengthToMeasure)
                            : stringToPrint,
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
                StatusStripHandler.StatusMessageType.Temporary, e.PrintAction.Equals(PrintAction.PrintToPreview)
                    ? Properties.Resources.MessagePreviewGenerated
                    : Properties.Resources.MessagePrintingFinished);
            Menu.MenuItems[4].MenuItems[1].Enabled = true;
            Menu.MenuItems[4].MenuItems[3].Enabled = true;
            for (int i = 0; i < 8; i++) {
                Menu.MenuItems[0].MenuItems[i].Enabled = true;
            }
            Cursor = Cursors.Default;
        }

        private void OnUpdateCheckerStateChanged(object sender, UpdateCheckEventArgs e) {
            statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.PersistentB, e.Message);
            if (dialog == null || !dialog.Visible) {
                dialog = e.Dialog;
            }
        }

        private void Copy(object sender, EventArgs e) {
            textBox.Copy();
            if (textBox.SelectionLength > 0) {
                statusStripHandler.SetMessage(
                    StatusStripHandler.StatusMessageType.Temporary, Properties.Resources.MessageCopiedToClipboard);
            }
        }

        private void SelectAll(object sender, EventArgs e) {
            textBox.SelectAll();
            statusStripHandler.SetMessage(
                StatusStripHandler.StatusMessageType.Temporary);
        }

        private void AnalyzeUrl(object sender, EventArgs e) {
            if (StaticMethods.CheckSelectedUrl(textBox)) {
                try {
                    StringBuilder arguments = new StringBuilder()
                        .Append(Constants.CommandLineSwitchWD)
                        .Append(Constants.Space)
                        .Append(StaticMethods.EscapeArgument(textBox.SelectedText.TrimStart()));
                    Process.Start(Application.ExecutablePath, arguments.ToString());
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                    statusStripHandler.SetMessage(StatusStripHandler.StatusMessageType.PersistentB, exception.Message);
                }
            }
        }

        private void Find(object sender, EventArgs e) {
            if (findThread != null && findThread.IsAlive) {
                try {
                    findForm.SafeSelect();
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                }
            } else {
                if (searching) {
                    textBox.HideSelection = false;
                } else {
                    search.searchString = null;
                }
                findThread = new Thread(new ThreadStart(Find));
                findThread.Start();
            }
        }

        private void Find() {
            findForm = new ConfigFindForm(search);
            findForm.AltCtrlShiftPPressed += new EventHandler(PrintImage);
            findForm.AltF11Pressed += new EventHandler(LaunchCalculator);
            findForm.AltF12Pressed += new EventHandler(LaunchNotepad);
            findForm.CtrlEPressed += new EventHandler(Export);
            findForm.CtrlPPressed += new EventHandler(Print);
            findForm.CtrlShiftLPressed += new EventHandler(ToggleWordWrap);
            findForm.CtrlShiftNPressed += new EventHandler(OpenEncoderDecoder);
            findForm.CtrlShiftPPressed += new EventHandler(PrintPreview);
            findForm.CtrlSPressed += new EventHandler(Save);
            findForm.DownPressed += new EventHandler(OnDownPressed);
            findForm.EndPressed += new EventHandler(OnEndPressed);
            findForm.F11Pressed += new EventHandler(TurnOffMonitors);
            findForm.Find += new EventHandler<SearchEventArgs>(OnFind);
            findForm.FormClosed += new FormClosedEventHandler(OnFormClosed);
            findForm.HelpRequested += new HelpEventHandler(OpenHelp);
            findForm.HomePressed += new EventHandler(OnHomePressed);
            findForm.PageDownPressed += new EventHandler(OnPageDownPressed);
            findForm.PageUpPressed += new EventHandler(OnPageUpPressed);
            findForm.UpPressed += new EventHandler(OnUpPressed);
            findForm.ShowDialog();
        }

        private void OnHomePressed(object sender, EventArgs e) => SendKeys.Send(Constants.SendKeysHome);

        private void OnEndPressed(object sender, EventArgs e) => SendKeys.Send(Constants.SendKeysEnd);

        private void OnPageUpPressed(object sender, EventArgs e) => SendKeys.Send(Constants.SendKeysPgUp);

        private void OnPageDownPressed(object sender, EventArgs e) => SendKeys.Send(Constants.SendKeysPgDn);

        private void OnUpPressed(object sender, EventArgs e) => SendKeys.Send(Constants.SendKeysUp);

        private void OnDownPressed(object sender, EventArgs e) => SendKeys.Send(Constants.SendKeysDown);

        private void OnFind(object sender, SearchEventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler<SearchEventArgs>(OnFind), sender, e);
            } else {
                if (!string.IsNullOrEmpty(e.Search.searchString)) {
                    textBox.HideSelection = false;
                    search = e.Search;
                    Find(search);
                }
                persistWindowState.SetVisible(e.Handle);
            }
        }

        private void FindNext(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(search.searchString)) {
                searching = true;
                Find(search);
            }
        }

        private void FindPrevious(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(search.searchString)) {
                searching = true;
                Find(search);
            }
        }

        private void Find(Search search) {
            searchResults = new List<SearchResult>();
            string haystack = textBox.Text;
            if (search.regularExpression) {
                Regex regex = new Regex(search.searchString, (search.caseSensitive
                    ? RegexOptions.None
                    : RegexOptions.IgnoreCase) | RegexOptions.CultureInvariant | RegexOptions.Multiline);
                foreach (Match match in regex.Matches(haystack)) {
                    searchResults.Add(new SearchResult(match.Index, match.Length));
                }
            } else if (search.startsWith || search.endsWith) {
                foreach (SearchLine searchLine in StaticMethods.SplitToLines(haystack)) {
                    if (search.startsWith && searchLine.line.StartsWith(search.searchString, search.caseSensitive
                            ? StringComparison.Ordinal
                            : StringComparison.OrdinalIgnoreCase)) {

                        searchResults.Add(new SearchResult(searchLine.index, search.searchString.Length));
                    }
                    if (search.endsWith && searchLine.line.EndsWith(search.searchString, search.caseSensitive
                            ? StringComparison.Ordinal
                            : StringComparison.OrdinalIgnoreCase)) {

                        searchResults.Add(new SearchResult(
                            searchLine.index + searchLine.line.Length - search.searchString.Length,
                            search.searchString.Length));
                    }
                }
            } else {
                for (int i = 0;
                    (i = haystack.IndexOf(search.searchString, i, search.caseSensitive
                        ? StringComparison.Ordinal
                        : StringComparison.OrdinalIgnoreCase)) > -1;
                    i += search.searchString.Length) {

                    searchResults.Add(new SearchResult(i, search.searchString.Length));
                }
            }
            if (searchResults.Count > 0) {
                if (search.backward && !invertDirection || !search.backward && invertDirection) {
                    if (--matchIndex < 0) {
                        matchIndex = searchResults.Count - 1;
                    }
                    while (!lastMatch && textBox.SelectionStart < searchResults[matchIndex].index) {
                        if (--matchIndex < 0) {
                            matchIndex = searchResults.Count - 1;
                            break;
                        }
                    }
                } else {
                    if (++matchIndex >= searchResults.Count) {
                        matchIndex = 0;
                    }
                    while (!lastMatch && textBox.SelectionStart > searchResults[matchIndex].index) {
                        if (++matchIndex >= searchResults.Count) {
                            matchIndex = 0;
                            break;
                        }
                    }
                }
                textBox.Select(searchResults[matchIndex].index, searchResults[matchIndex].length);
                textBox.ScrollToCaret();
            }
            lastMatch = false;
            statusStripHandler.SetSearchResult(searchResults.Count, matchIndex + 1);
            if (matchIndex.Equals(0) || matchIndex.Equals(searchResults.Count - 1)) {
                lastMatch = true;
            }
        }

        private void ToggleWordWrap(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler(ToggleWordWrap), sender, e);
            } else {
                Cursor = Cursors.WaitCursor;
                try {
                    textBox.WordWrap = !textBox.WordWrap;
                    Menu.MenuItems[2].MenuItems[0].Checked = textBox.WordWrap;
                } catch (OutOfMemoryException exception) {
                    ShowException(exception, Properties.Resources.MessageOutOfMemoryError);
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                } finally {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void OpenEncoderDecoder(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler(OpenEncoderDecoder), sender, e);
            } else {
                try {
                    Process.Start(Application.ExecutablePath, Constants.CommandLineSwitchWD);
                } catch (Exception exception) {
                    ShowException(exception);
                }
            }
        }

        private void TurnOffMonitors(object sender, EventArgs e) {
            if (turnOffThread != null && turnOffThread.IsAlive) {
                try {
                    countDownForm.SafeSelect();
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                }
            } else {
                turnOffThread = new Thread(new ThreadStart(TurnOffMonitors));
                turnOffThread.Start();
            }
        }

        private void TurnOffMonitors() {
            countDownForm = new CountDownForm();
            countDownForm.HelpButtonClicked += new CancelEventHandler((sender, e) => {
                countDownForm.Close();
                OpenHelp(sender, e);
            });
            countDownForm.HelpRequested += new HelpEventHandler((sender, hlpevent) => {
                countDownForm.Close();
                OpenHelp(sender, hlpevent);
            });
            countDownForm.ShowDialog();
        }

        private void LaunchCalculator(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler(LaunchCalculator), sender, e);
            } else {
                try {
                    Process.Start(Path.Combine(Environment.SystemDirectory, Constants.CalcFileName));
                } catch (Exception exception) {
                    ShowException(exception);
                }
            }
        }

        private void LaunchNotepad(object sender, EventArgs e) {
            if (InvokeRequired) {
                Invoke(new EventHandler(LaunchNotepad), sender, e);
            } else {
                try {
                    Process.Start(Path.Combine(Environment.SystemDirectory, Constants.NotepadFileName));
                } catch (Exception exception) {
                    ShowException(exception);
                }
            }
        }

        private void LaunchWordPad(object sender, EventArgs e) {
            try {
                Process.Start(Path.Combine(Environment.SystemDirectory, Constants.WordPadFileName));
            } catch (Exception exception) {
                ShowException(exception);
            }
        }

        private void LaunchCharMap(object sender, EventArgs e) {
            try {
                Process.Start(Path.Combine(Environment.SystemDirectory, Constants.CharMapFileName));
            } catch (Exception exception) {
                ShowException(exception);
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
                    ShowException(exception);
                }
            }
        }

        private void OpenHelp(object sender, HelpEventArgs e) => OpenHelp(sender, (EventArgs)e);

        private void OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Alt && e.Shift && e.Control && e.KeyCode.Equals(Keys.P)) {
                if (Menu.MenuItems[0].MenuItems[7].Enabled) {
                    e.SuppressKeyPress = true;
                    PrintImage(sender, e);
                }
            } else if (e.Shift && e.Control && e.KeyCode.Equals(Keys.L)) {
                e.SuppressKeyPress = true;
                ToggleWordWrap(sender, e);
            } else if (e.Shift && e.Control && e.KeyCode.Equals(Keys.N)) {
                e.SuppressKeyPress = true;
                OpenEncoderDecoder(sender, e);
            } else if (e.Shift && e.Control && e.KeyCode.Equals(Keys.P)) {
                if (Menu.MenuItems[0].MenuItems[4].Enabled) {
                    e.SuppressKeyPress = true;
                    PrintPreview(sender, e);
                }
            } else if (e.Control && e.KeyCode.Equals(Keys.A)) {
                e.SuppressKeyPress = true;
                SelectAll(sender, e);
            } else if (e.Control && e.KeyCode.Equals(Keys.C)) {
                e.SuppressKeyPress = true;
                Copy(sender, e);
            } else if (e.KeyCode.Equals(Keys.ShiftKey)) {
                invertDirection = true;
            } else if (e.Shift && e.KeyCode.Equals(Keys.F3)) {
                e.SuppressKeyPress = true;
                invertDirection = true;
                FindPrevious(sender, e);
            } else if (e.KeyCode.Equals(Keys.F3)) {
                e.SuppressKeyPress = true;
                invertDirection = false;
                FindNext(sender, e);
            } else if (e.KeyCode.Equals(Keys.Escape)) {
                e.SuppressKeyPress = true;
                if (findForm != null) {
                    CloseFindForm();
                } else if (searching) {
                    statusStripHandler.ClearSearchResult();
                    searching = false;
                }
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

        private void OnKeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode.Equals(Keys.ShiftKey)) {
                invertDirection = false;
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

        private enum PrintType {
            Text,
            Image
        }
    }
}
