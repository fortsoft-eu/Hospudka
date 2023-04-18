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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WebPWrapper;

namespace Hospudka {
    public static class StaticMethods {
        public static bool CheckSelectedUrl(TextBox textBox) {
            if (textBox.SelectionLength <= 3072) {
                string trimmed = textBox.SelectedText.TrimStart();
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

        public static void DumpEnumerable(IEnumerable data) {
            try {
                string filePath = Path.Combine(
                    Application.LocalUserAppDataPath,
                    DateTime.Now.ToString(Constants.DumpFileNameTimeFormat) + Constants.ExtensionTxt);
                using (StreamWriter streamWriter = File.AppendText(filePath)) {
                    StringBuilder stringBuilder = new StringBuilder();
                    int i = 0;
                    foreach (object item in data) {
                        stringBuilder.Append(i++)
                            .Append(Constants.Colon)
                            .AppendLine(item.ToString());
                    }
                    streamWriter.WriteLine(stringBuilder.ToString());
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        public static void DumpString(string str) {
            try {
                string filePath = Path.Combine(
                    Application.LocalUserAppDataPath,
                    DateTime.Now.ToString(Constants.DumpFileNameTimeFormat) + Constants.ExtensionTxt);
                using (StreamWriter streamWriter = File.AppendText(filePath)) {
                    streamWriter.WriteLine(str);
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        public static string EscapeArgument(string argument) {
            argument = Regex.Replace(argument, @"(\\*)" + Constants.QuotationMark, @"$1$1\" + Constants.QuotationMark);
            return Constants.QuotationMark + Regex.Replace(argument, @"(\\+)$", @"$1$1") + Constants.QuotationMark;
        }

        public static void ExportAsImage(Control control, string filePath) {
            using (Bitmap bitmap = new Bitmap(control.Width, control.Height, PixelFormat.Format32bppArgb)) {
                control.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                SaveBitmap(bitmap, filePath);
            }
        }

        public static Size GetNewGraphicsSize(Size graphicSize, Size canvasSize) {
            bool rotate = IsGraphicsRotationNeeded(graphicSize, canvasSize);
            float ratio = 1f;
            float ratioWidth = graphicSize.Width / (float)(rotate ? canvasSize.Height : canvasSize.Width);
            float ratioHeight = graphicSize.Height / (float)(rotate ? canvasSize.Width : canvasSize.Height);
            float ratioMax = Math.Max(ratioWidth, ratioHeight);
            if (ratioMax > ratio) {
                ratio = ratioMax;
            }
            return new Size((int)Math.Floor(graphicSize.Width / ratio), (int)Math.Floor(graphicSize.Height / ratio));
        }

        public static bool IsGraphicsRotationNeeded(Size graphicSize, Size canvasSize) {
            if (graphicSize.Width <= 0 || graphicSize.Height <= 0 || canvasSize.Width <= 0 || canvasSize.Height <= 0) {
                return false;
            }
            if (graphicSize.Width / (float)graphicSize.Height == 1f || canvasSize.Width / (float)canvasSize.Height == 1f) {
                return false;
            }
            if (graphicSize.Width < canvasSize.Width && graphicSize.Height < canvasSize.Height) {
                return false;
            }
            if (graphicSize.Width / (float)graphicSize.Height < 1f && canvasSize.Width / (float)canvasSize.Height < 1f ||
                graphicSize.Width / (float)graphicSize.Height > 1f && canvasSize.Width / (float)canvasSize.Height > 1f) {
                return false;
            }
            return true;
        }

        public static void SaveBitmap(Bitmap bitmap, string finePath) {
            switch (Path.GetExtension(finePath).ToLowerInvariant()) {
                case Constants.ExtensionBmp:
                    bitmap.Save(finePath, ImageFormat.Bmp);
                    break;
                case Constants.ExtensionGif:
                    bitmap.Save(finePath, ImageFormat.Gif);
                    break;
                case Constants.ExtensionJpg:
                    bitmap.Save(finePath, ImageFormat.Jpeg);
                    break;
                case Constants.ExtensionTif:
                    bitmap.Save(finePath, ImageFormat.Tiff);
                    break;
                case Constants.ExtensionWebP:
                    using (WebP webP = new WebP()) {
                        File.WriteAllBytes(finePath, webP.EncodeLossless(bitmap));
                    }
                    break;
                default:
                    bitmap.Save(finePath, ImageFormat.Png);
                    break;
            }
        }

        public static string ShowSize(long length, IFormatProvider provider, bool useDecimalPrefix) {
            double num = length;
            if (num < 1000d) {
                return new StringBuilder()
                    .Append(num.ToString(Constants.ZeroDecimalDigitsFormat, provider))
                    .Append(Constants.Space)
                    .Append(Constants.Byte)
                    .ToString();
            }
            num = num / (useDecimalPrefix ? 1000d : 1024d);
            if (num < 1000d) {
                return new StringBuilder()
                    .Append(num.ToString(Constants.OneDecimalDigitFormat, provider))
                    .Append(Constants.Space)
                    .Append(useDecimalPrefix ? Constants.Kilobyte : Constants.Kibibyte)
                    .ToString();
            }
            num = num / (useDecimalPrefix ? 1000d : 1024d);
            if (num < 1000d) {
                return new StringBuilder()
                    .Append(num.ToString(Constants.OneDecimalDigitFormat, provider))
                    .Append(Constants.Space)
                    .Append(useDecimalPrefix ? Constants.Megabyte : Constants.Mebibyte)
                    .ToString();
            }
            num = num / (useDecimalPrefix ? 1000d : 1024d);
            if (num < 1000d) {
                return new StringBuilder()
                    .Append(num.ToString(Constants.OneDecimalDigitFormat, provider))
                    .Append(Constants.Space)
                    .Append(useDecimalPrefix ? Constants.Gigabyte : Constants.Gibibyte)
                    .ToString();
            }
            num = num / (useDecimalPrefix ? 1000d : 1024d);
            if (num < 1000d) {
                return new StringBuilder()
                    .Append(num.ToString(Constants.OneDecimalDigitFormat, provider))
                    .Append(Constants.Space)
                    .Append(useDecimalPrefix ? Constants.Terabyte : Constants.Tebibyte)
                    .ToString();
            }
            num = num / (useDecimalPrefix ? 1000d : 1024d);
            if (num < 1000d) {
                return new StringBuilder()
                    .Append(num.ToString(Constants.OneDecimalDigitFormat, provider))
                    .Append(Constants.Space)
                    .Append(useDecimalPrefix ? Constants.Petabyte : Constants.Pebibyte)
                    .ToString();
            }
            num = num / (useDecimalPrefix ? 1000d : 1024d);
            if (num < 1000d) {
                return new StringBuilder()
                    .Append(num.ToString(Constants.OneDecimalDigitFormat, provider))
                    .Append(Constants.Space)
                    .Append(useDecimalPrefix ? Constants.Exabyte : Constants.Exbibyte)
                    .ToString();
            }
            num = num / (useDecimalPrefix ? 1000d : 1024d);
            if (num < 1000d) {
                return new StringBuilder()
                    .Append(num.ToString(Constants.OneDecimalDigitFormat, provider))
                    .Append(Constants.Space)
                    .Append(useDecimalPrefix ? Constants.Zettabyte : Constants.Zebibyte)
                    .ToString();
            }
            num = num / (useDecimalPrefix ? 1000d : 1024d);
            if (num < 1000d) {
                return new StringBuilder()
                    .Append(num.ToString(Constants.OneDecimalDigitFormat, provider))
                    .Append(Constants.Space)
                    .Append(useDecimalPrefix ? Constants.Yottabyte : Constants.Yobibyte)
                    .ToString();
            }
            num = num / (useDecimalPrefix ? 1000d : 1024d);
            if (num < 1000d) {
                return new StringBuilder()
                    .Append(num.ToString(Constants.OneDecimalDigitFormat, provider))
                    .Append(Constants.Space)
                    .Append(useDecimalPrefix ? Constants.Ronnabyte : Constants.Robibyte)
                    .ToString();
            }
            num = num / (useDecimalPrefix ? 1000d : 1024d);
            return new StringBuilder()
                .Append(num.ToString(Constants.OneDecimalDigitFormat, provider))
                .Append(Constants.Space)
                .Append(useDecimalPrefix ? Constants.Quettabyte : Constants.Qubibyte)
                .ToString();
        }

        public static List<SearchLine> SplitToLines(string str) {
            List<SearchLine> lines = new List<SearchLine>();
            StringBuilder stringBuilder = new StringBuilder();
            int charIndex = 0, index = 0;
            foreach (char c in str.ToCharArray()) {
                if (c.Equals(Constants.CarriageReturn) || c.Equals(Constants.LineFeed)) {
                    lines.Add(new SearchLine(stringBuilder.ToString(), index));
                    stringBuilder = new StringBuilder();
                } else {
                    if (stringBuilder.Length.Equals(0)) {
                        index = charIndex;
                    }
                    stringBuilder.Append(c);
                }
                charIndex++;
            }
            if (stringBuilder.Length > 0) {
                lines.Add(new SearchLine(stringBuilder.ToString(), charIndex));
            }
            return lines;
        }
    }
}
