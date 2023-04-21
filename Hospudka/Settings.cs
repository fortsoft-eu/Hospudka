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
 * Version 1.0.0.2
 */

using FortSoft.Tools;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Hospudka {

    /// <summary>
    /// This is an implementation of using the PersistentSettings class for
    /// Hospudka.
    /// </summary>
    public sealed class Settings : IDisposable {

        /// <summary>
        /// Fields.
        /// </summary>
        private ConfigHandler configHandler;
        private NumberFormatHandler numberFormatHandler;
        private PersistentSettings persistentSettings;

        /// <summary>
        /// Occurs on successful saving all application settings into the Windows
        /// registry.
        /// </summary>
        public event EventHandler Saved;

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings() {
            numberFormatHandler = new NumberFormatHandler();
            persistentSettings = new PersistentSettings();
            Load();
        }

        /// <summary>
        /// Represents the setting if the application should check for updates.
        /// The default value is false.
        /// </summary>
        public bool CheckForUpdates { get; set; }

        /// <summary>
        /// Represents whether visual styles will be used when rendering
        /// application windows. The default value is false.
        /// </summary>
        public bool DisableThemes { get; set; }

        /// <summary>
        /// Represents the printing setting, whether to use soft margins (larger)
        /// or hard margins (smaller). This setting does not apply to the
        /// embedded Chromium browser. The default value is true.
        /// </summary>
        public bool PrintSoftMargins { get; set; } = true;

        /// <summary>
        /// Represents the setting if the application should inform the user
        /// about available updates in the status bar only. If not, a pop-up
        /// window will appear. The default value is false.
        /// </summary>
        public bool StatusBarNotifOnly { get; set; }

        /// <summary>
        /// Represents the setting of whether decimal prefixes will be used when
        /// displaying data size units. If not, the binary unit prefixes will be
        /// used. The default value is false.
        /// </summary>
        public bool UseDecimalPrefix { get; set; }

        /// <summary>
        /// Represents how the 3D border of the labels will be rendered on the
        /// status strip in the main application window. The default value is
        /// Border3DStyle.Adjust.
        /// </summary>
        public Border3DStyle Border3DStyle { get; set; } = Border3DStyle.Adjust;

        /// <summary>
        /// Last export extension index used. The default value is four.
        /// </summary>
        public int ExtensionFilterIndex { get; set; } = 4;

        /// <summary>
        /// Represents the integer index of the number display format used. The
        /// default value is zero.
        /// </summary>
        public int NumberFormatInt { get; set; }

        /// <summary>
        /// Gets an instance of an NumberFormatComboBox object.
        /// </summary>
        public NumberFormatComboBox NumberFormat => numberFormatHandler.GetNumberFormat(NumberFormatInt);

        /// <summary>
        /// Gets the instance of an NumberFormatHandler object.
        /// </summary>
        public NumberFormatHandler NumberFormatHandler => numberFormatHandler;

        /// <summary>
        /// The config hash string. The default value is null.
        /// </summary>
        public string ConfigHash { get; set; }

        /// <summary>
        /// Last export directory path used. The default value is null.
        /// </summary>
        public string LastExportDirectory { get; set; }

        /// <summary>
        /// Represents the rendering mode of the status strip in the main
        /// application window. The default value is ToolStripRenderMode.System.
        /// </summary>
        public ToolStripRenderMode StripRenderMode { get; set; } = ToolStripRenderMode.System;

        /// <summary>
        /// Loads the software application settings from the Windows registry.
        /// </summary>
        private void Load() => ConfigHash = persistentSettings.Load("ConfigHash", ConfigHash);

        /// <summary>
        /// Saves the software application settings into the Windows registry.
        /// </summary>
        public void Save() {
            persistentSettings.Save("ConfigHash", ConfigHash);
            Saved?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// This setting will not be directly stored in the Windows registry.
        /// </summary>
        public bool RenderWithVisualStyles { get; set; }

        /// <summary>
        /// This setting will not be stored in the Windows registry.
        /// </summary>
        public string Config { get; set; }

        /// <summary>Loads remote configuration file.</summary>
        public void LoadConfig() {
            if (string.IsNullOrEmpty(ConfigHash)) {
                if (SingleInstance.FocusRunning(Application.ExecutablePath, Program.GetTitle())) {
                    Environment.Exit(0);
                } else {
                    ConfigHashForm configHashForm = new ConfigHashForm(this);
                    if (configHashForm.ShowDialog().Equals(DialogResult.OK)) {
                        configHandler = new ConfigHandler(this);
                    } else {
                        Environment.Exit(0);
                    }
                }
            } else {
                try {
                    configHandler = new ConfigHandler(this);
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    StringBuilder title = new StringBuilder()
                        .Append(Program.GetTitle())
                        .Append(Constants.Space)
                        .Append(Constants.EnDash)
                        .Append(Constants.Space)
                        .Append(Properties.Resources.CaptionError);
                    MessageBox.Show(exception.Message, title.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>Saves remote configuration file.</summary>
        public async Task<bool> SaveConfigAsync() {
            string responseString = await configHandler.SaveConfigAsync();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(responseString);
            XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName(Constants.XmlElementStatus);
            string status = string.Empty;
            foreach (XmlElement xmlElement in xmlNodeList) {
                status = xmlElement.InnerText;
            }
            return Constants.StatusOk.Equals(status);
        }

        /// <summary>
        /// Clears the software application values from the Windows registry.
        /// </summary>
        public void Clear() => persistentSettings.Clear();

        /// <summary>Clean up any resources being used.</summary>
        public void Dispose() => persistentSettings.Dispose();

        /// <summary>
        /// Hardware-independent static method for conversion of byte array into
        /// an integer value.
        /// </summary>
        /// <param name="bytes">Byte array.</param>
        /// <returns>An integer value to store in the Windows registry.</returns>
        public static int ByteArrayToInt(byte[] bytes) {
            if (!BitConverter.IsLittleEndian) {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Hardware-independent static method for conversion of an integer value
        /// into a byte array.
        /// </summary>
        /// <param name="val">An integer value stored in the registry.</param>
        public static byte[] IntToByteArray(int val) {
            byte[] bytes = BitConverter.GetBytes(val);
            if (!BitConverter.IsLittleEndian) {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        /// <summary>
        /// Hardware-independent static method for conversion of two ushort
        /// values into an integer value.
        /// </summary>
        /// <param name="values">An array of ushort values.</param>
        /// <returns>An integer value to store in the Windows registry.</returns>
        public static int UShortArrayToInt(ushort[] values) {
            byte[] bytes = new byte[4];
            Array.Copy(
                BitConverter.GetBytes(values[0]),
                0,
                bytes,
                BitConverter.IsLittleEndian ? 0 : 2,
                2);
            Array.Copy(
                BitConverter.GetBytes(values[1]),
                0,
                bytes,
                BitConverter.IsLittleEndian ? 2 : 0,
                2);
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Hardware-independent static method for conversion of an integer value
        /// into two ushort values.
        /// </summary>
        /// <param name="val">An integer value stored in the registry.</param>
        public static ushort[] IntToUShortArray(int val) {
            byte[] bytes = BitConverter.GetBytes(val);
            return new ushort[] {
                BitConverter.ToUInt16(bytes, BitConverter.IsLittleEndian ? 0 : 2),
                BitConverter.ToUInt16(bytes, BitConverter.IsLittleEndian ? 2 : 0)
            };
        }

    }
}
