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
 * Version 1.1.0.0
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace Hospudka {

    /// <summary>
    /// Implements standard gong sound.
    /// </summary>
    public class Gong {

        /// <summary>
        /// The name of the destination MP3 file where the gong sound will be
        /// saved and made available to Windows Media Player.
        /// </summary>
        private const string gongFileName = "Gong.mp3";

        /// <summary>
        /// Field with the full path to the extracted MP3 file with the gong
        /// sound.
        /// </summary>
        private string gongFilePath;

        /// <summary>
        /// Field with Windows Media Player object.
        /// </summary>
        private WindowsMediaPlayer windowsMediaPlayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Gong"/> class.
        /// </summary>
        public Gong() {
            ExtractGongAsync();

            gongFilePath = Path.Combine(Path.GetDirectoryName(Application.LocalUserAppDataPath), gongFileName);

            windowsMediaPlayer = new WindowsMediaPlayer();
        }

        /// <summary>
        /// Extracts the gong sound in the MP3 file imported in the resources
        /// called 'Gong' and saves it to the application data folder as an MP3
        /// file.
        /// </summary>
        private static async void ExtractGongAsync() {
            await Task.Run(new Action(() => {
                try {
                    string gongFilePath = Path.Combine(Path.GetDirectoryName(Application.LocalUserAppDataPath), gongFileName);
                    if (!File.Exists(gongFilePath)) {
                        File.WriteAllBytes(gongFilePath, Properties.Resources.Gong);
                    }
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                }
            }));
        }

        /// <summary>Performs chime.</summary>
        public void Chime() => windowsMediaPlayer.URL = gongFilePath;
    }
}
