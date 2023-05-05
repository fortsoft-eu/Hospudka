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
 * Version 1.1.1.0
 */

using FortSoft.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hospudka {
    public sealed class WebPatcher : IDisposable {
        private Dictionary<string, string> htmlFileNameSubstitutions;
        private long itemCount;
        private List<string> createdDirectories;
        private RandomGenerator randomGenerator;
        private string trailingSlash, trailingSlashLeadingSpace;

        public event EventHandler Counting;
        public event ErrorEventHandler Error;
        public event EventHandler<PatchedEventArgs> Patched;

        public WebPatcher() {
            randomGenerator = new RandomGenerator(8, true, true, true);
            trailingSlash = new StringBuilder()
                .Append(Constants.Slash)
                .Append(Constants.GreaterThan)
                .ToString();
            trailingSlashLeadingSpace = new StringBuilder()
                .Append(Constants.Space)
                .Append(Constants.Slash)
                .Append(Constants.GreaterThan)
                .ToString();
        }

        public string DestinationPath { get; set; }

        public string ResourcesPath { get; set; }

        public string SourcePath { get; set; }

        public string[] Strings { get; set; }

        private void CopyRecursive(string sourceDirPath, string targetDirPath) {
            Regex regex = new Regex(Strings[20], RegexOptions.IgnoreCase);
            foreach (string item in Directory.GetFiles(sourceDirPath)) {
                string dirPath = Path.GetDirectoryName(item);
                if (!createdDirectories.Contains(targetDirPath)) {
                    Directory.CreateDirectory(targetDirPath);
                }
                string newFileName = Path.GetFileName(item);
                if (newFileName.Equals(Strings[0])) {
                    newFileName = Strings[1];
                } else if (newFileName.EndsWith(Constants.ExtensionHtml)) {
                    newFileName = Path.GetFileNameWithoutExtension(newFileName).ToLowerInvariant() + Constants.ExtensionHtml;
                }
                string newFilePath = Path.Combine(targetDirPath, newFileName);
                if (item.EndsWith(Constants.ExtensionHtml)) {
                    File.WriteAllText(newFilePath, PatchHtml(File.ReadAllText(item), newFileName));
                } else if (item.EndsWith(Constants.ExtensionCss)) {
                    string content = Regex.Replace(File.ReadAllText(item), Constants.CssCommentsPattern, string.Empty,
                        RegexOptions.Singleline);
                    foreach (KeyValuePair<string, string> keyValuePair in htmlFileNameSubstitutions) {
                        content = content.Replace(keyValuePair.Key, keyValuePair.Value);
                    }
                    if (regex.IsMatch(Path.GetFileName(item))) {
                        File.WriteAllText(newFilePath, content
                            .Replace(
                                new StringBuilder()
                                    .Append(Constants.OpeningCurly)
                                    .Append(Strings[25])
                                    .Append(Constants.ClosingCurly)
                                    .ToString(),
                                new StringBuilder()
                                    .Append(Constants.OpeningCurly)
                                    .Append(Strings[21])
                                    .Append(Constants.ClosingCurly)
                                    .ToString())
                            .Replace(
                                new StringBuilder()
                                    .Append(Constants.Semicolon)
                                    .Append(Strings[25])
                                    .Append(Constants.ClosingCurly)
                                    .ToString(),
                                new StringBuilder()
                                    .Append(Constants.Semicolon)
                                    .Append(Strings[21])
                                    .Append(Constants.ClosingCurly)
                                    .ToString()));
                    } else if (item.EndsWith(Strings[4])) {
                        File.WriteAllText(newFilePath, content
                            .Replace(Strings[28], Strings[32])
                            .Replace(Strings[29], Strings[33])
                            .Replace(Strings[30], Strings[34])
                            .Replace(
                                new StringBuilder()
                                    .Append(Constants.Semicolon)
                                    .Append(Strings[24])
                                    .Append(Constants.ClosingCurly)
                                    .ToString(),
                                new StringBuilder()
                                    .Append(Constants.Semicolon)
                                    .Append(Strings[21])
                                    .Append(Constants.ClosingCurly)
                                    .ToString())
                            .Replace(
                                new StringBuilder()
                                    .Append(Constants.OpeningCurly)
                                    .Append(Strings[24])
                                    .Append(Constants.Semicolon)
                                    .ToString(),
                                new StringBuilder()
                                    .Append(Constants.OpeningCurly)
                                    .Append(Strings[21])
                                    .Append(Constants.Semicolon)
                                    .ToString())
                            .Replace(
                                new StringBuilder()
                                    .Append(Constants.OpeningCurly)
                                    .Append(Strings[25])
                                    .Append(Constants.ClosingCurly)
                                    .ToString(),
                                new StringBuilder()
                                    .Append(Constants.OpeningCurly)
                                    .Append(Strings[21])
                                    .Append(Constants.ClosingCurly)
                                    .ToString())
                            .Replace(
                                new StringBuilder()
                                    .Append(Constants.Semicolon)
                                    .Append(Strings[25])
                                    .Append(Constants.Semicolon)
                                    .ToString(),
                                new StringBuilder()
                                    .Append(Constants.Semicolon)
                                    .Append(Strings[21])
                                    .Append(Constants.Semicolon)
                                    .ToString())
                            .Replace(
                                new StringBuilder()
                                    .Append(Constants.Semicolon)
                                    .Append(Strings[25])
                                    .Append(Constants.ClosingCurly)
                                    .ToString(),
                                new StringBuilder()
                                    .Append(Constants.Semicolon)
                                    .Append(Strings[21])
                                    .Append(Constants.ClosingCurly)
                                    .ToString()));
                    } else if (item.EndsWith(Strings[3])) {
                        File.WriteAllText(newFilePath, content
                            .Replace(Strings[31], Strings[35])
                            .Replace(
                                new StringBuilder()
                                    .Append(Constants.Semicolon)
                                    .Append(Strings[26])
                                    .Append(Constants.ClosingCurly)
                                    .ToString(),
                                new StringBuilder()
                                    .Append(Constants.Semicolon)
                                    .Append(Strings[22])
                                    .Append(Constants.ClosingCurly)
                                    .ToString()));
                    } else if (item.EndsWith(Strings[2])) {
                        File.WriteAllText(newFilePath, content
                            .Replace(Strings[31], Strings[35])
                            .Replace(
                                new StringBuilder()
                                    .Append(Constants.Space)
                                    .Append(Constants.Space)
                                    .Append(Strings[27])
                                    .Append(Constants.Semicolon)
                                    .ToString(),
                                new StringBuilder()
                                    .Append(Constants.Space)
                                    .Append(Constants.Space)
                                    .Append(Strings[23])
                                    .Append(Constants.Semicolon)
                                    .ToString()));
                    } else if (item.EndsWith(Strings[5])) {
                        File.WriteAllText(newFilePath, content
                            .Replace(
                                new StringBuilder()
                                    .Append(Constants.Semicolon)
                                    .Append(Strings[24])
                                    .Append(Constants.Semicolon)
                                    .ToString(),
                                new StringBuilder()
                                    .Append(Constants.Semicolon)
                                    .Append(Strings[21])
                                    .Append(Constants.Semicolon)
                                    .ToString())
                            .Replace(
                                new StringBuilder()
                                    .Append(Constants.Semicolon)
                                    .Append(Strings[24])
                                    .Append(Constants.ClosingCurly)
                                    .ToString(),
                                new StringBuilder()
                                    .Append(Constants.Semicolon)
                                    .Append(Strings[21])
                                    .Append(Constants.ClosingCurly)
                                    .ToString())
                            .Replace(
                                new StringBuilder()
                                    .Append(Constants.OpeningCurly)
                                    .Append(Strings[24])
                                    .Append(Constants.Semicolon)
                                    .ToString(),
                                new StringBuilder()
                                    .Append(Constants.OpeningCurly)
                                    .Append(Strings[21])
                                    .Append(Constants.Semicolon)
                                    .ToString()));
                    } else if (item.EndsWith(Strings[6]) || item.EndsWith(Strings[7])) {
                        File.WriteAllText(newFilePath, content);
                    } else {
                        if (File.Exists(newFilePath)) {
                            File.Delete(newFilePath);
                        }
                        File.Copy(item, newFilePath);
                    }
                } else if (item.EndsWith(Constants.ExtensionJs) || item.EndsWith(Constants.ExtensionJson)) {
                    string content = Regex.Replace(File.ReadAllText(item), Constants.JSCommentsPattern, string.Empty,
                        RegexOptions.Singleline);
                    foreach (KeyValuePair<string, string> keyValuePair in htmlFileNameSubstitutions) {
                        content = content.Replace(keyValuePair.Key, keyValuePair.Value);
                    }
                    File.WriteAllText(newFilePath, content);
                } else {
                    if (sourceDirPath.EndsWith(Path.Combine(Strings[10], Strings[9]))) {
                        if (newFileName.Equals(Strings[8])) {
                            if (File.Exists(newFilePath)) {
                                File.Delete(newFilePath);
                            }
                            File.Copy(item, newFilePath);
                        }
                    } else {
                        if (File.Exists(newFilePath)) {
                            File.Delete(newFilePath);
                        }
                        File.Copy(item, newFilePath);
                    }
                }
            }
            foreach (string item in Directory.GetDirectories(sourceDirPath)) {
                CopyRecursive(item, Path.Combine(targetDirPath, Path.GetFileName(item)));
            }
        }

        private long CountItems() {
            Counting?.Invoke(this, EventArgs.Empty);
            itemCount = 0;
            CountItemsRecursive(DestinationPath);
            return itemCount;
        }

        private void CountItemsRecursive(string directoryPath) {
            string[] directories = Directory.GetDirectories(directoryPath);
            itemCount += Directory.GetFiles(directoryPath).LongLength;
            itemCount += directories.LongLength;
            foreach (string directory in directories) {
                CountItemsRecursive(directory);
            }
        }

        public void Dispose() => randomGenerator.Dispose();

        private void EmptyDestinationDirectory() => EmptyDirectory(DestinationPath);

        public async void EmptyDestinationDirectoryAsync() {
            await Task.Run(new Action(() => {
                try {
                    EmptyDirectory(DestinationPath);
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                    Error?.Invoke(this, new ErrorEventArgs(exception));
                }
            }));
        }

        public async void EmptySourceDirectoryAsync() {
            await Task.Run(new Action(() => {
                try {
                    EmptyDirectory(SourcePath);
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                    Error?.Invoke(this, new ErrorEventArgs(exception));
                }
            }));
        }

        private void GetHtmlFileNameSubstitutions(string dirPath) {
            foreach (string item in Directory.GetFiles(dirPath)) {
                string fileName = Path.GetFileName(item);
                if (fileName.Equals(Strings[0])) {
                    htmlFileNameSubstitutions.Add(fileName, Strings[1]);
                } else if (Regex.IsMatch(fileName, Constants.SubstitutionPattern, RegexOptions.IgnoreCase)) {
                    StringBuilder substitution = new StringBuilder()
                        .Append(Path.GetFileNameWithoutExtension(item).ToLowerInvariant())
                        .Append(Constants.ExtensionHtml);
                    htmlFileNameSubstitutions.Add(fileName, substitution.ToString());
                }
            }
            foreach (string item in Directory.GetDirectories(dirPath)) {
                GetHtmlFileNameSubstitutions(item);
            }
        }

        public bool IsDestinationDirectoryEmpty() => IsDirectoryEmpty(DestinationPath);

        public bool IsSourceDirectoryEmpty() => IsDirectoryEmpty(SourcePath);

        private void Patch() {
            createdDirectories = new List<string>();
            htmlFileNameSubstitutions = new Dictionary<string, string>();
            GetHtmlFileNameSubstitutions(SourcePath);
            CopyRecursive(SourcePath, DestinationPath);
            CopyRecursive(ResourcesPath, DestinationPath);
        }

        private string PatchHtml(string html, string fileName) {
            string baseUrl = Strings[14];
            string description = new StringBuilder()
                 .Append(Strings[15])
                 .Append(Strings[16])
                 .Append(Strings[17])
                 .Append(Strings[18])
                 .Append(Strings[19])
                 .ToString();
            string siteName = Strings[13];
            string sToken = randomGenerator.Generate();

            DateTime dateTime = DateTime.UtcNow;
            Regex commentsRegex = new Regex(Constants.HtmlCommentsPattern);
            Regex appendRegex = new Regex(Constants.AppendPattern);

            bool skip = false;
            bool main = false;
            bool context = false;
            bool parttag = false;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> item in htmlFileNameSubstitutions) {
                html = html.Replace(item.Key, item.Value);
            }

            StringReader stringReader = new StringReader(html.Replace(trailingSlashLeadingSpace, Constants.GreaterThan.ToString())
                .Replace(trailingSlash, Constants.GreaterThan.ToString())
                .Replace(Strings[36], Constants.HtmlTagStyle)
                .Replace(
                    new StringBuilder()
                        .Append(Constants.GreaterThan)
                        .Append(Constants.HtmlTagEH4)
                        .ToString(),
                    new StringBuilder()
                        .Append(Constants.GreaterThan)
                        .Append(Constants.HtmlEntityNbsp)
                        .Append(Constants.HtmlTagEH4)
                        .ToString()));
            int counterButton = 0;
            for (string line; (line = stringReader.ReadLine()) != null;) {
                if (parttag && line.Trim().Equals(Constants.GreaterThan.ToString())) {
                    stringBuilder.Append(Constants.GreaterThan)
                        .AppendLine();
                    parttag = false;
                    continue;
                }
                if (line.Contains(Strings[37])) {
                    stringBuilder.AppendLine(Strings[38]);
                    continue;
                }
                if (line.Contains(Constants.HtmlTag)) {
                    stringBuilder.AppendLine(Strings[38]);
                    context = true;
                    continue;
                }
                if (line.Contains(Strings[39])) {
                    if (!context) {
                        stringBuilder.AppendFormat(Strings[40], dateTime.ToString(Constants.RFC1123DateTimeFormat))
                            .AppendLine()
                            .AppendFormat(Strings[41],
                                StartOfWeek(dateTime, DayOfWeek.Monday).AddDays(7).ToString(Constants.RFC1123DateTimeFormat))
                            .AppendLine();
                    }
                    stringBuilder.AppendLine(Strings[42]);
                    continue;
                }
                if (line.Contains(Strings[43])) {
                    stringBuilder.AppendFormat(Strings[44], siteName, description)
                        .AppendLine();
                    continue;
                }
                if (line.Contains(Strings[45])) {
                    stringBuilder.Append(Strings[46])
                        .AppendLine(Strings[47])
                        .AppendLine(Strings[48])
                        .AppendFormat(Strings[49], baseUrl)
                        .AppendLine()
                        .AppendFormat(Strings[50], baseUrl)
                        .AppendLine()
                        .AppendLine(Strings[51])
                        .AppendLine(Strings[52])
                        .AppendLine(Strings[53])
                        .AppendLine(Strings[54])
                        .AppendFormat(Strings[55], FormatMetaRevisedDateTime(dateTime.ToLocalTime()))
                        .AppendLine()
                        .AppendLine(Strings[56])
                        .AppendFormat(Strings[57], siteName, dateTime.ToLocalTime().Year)
                        .AppendLine()
                        .AppendLine(Strings[58]);
                    continue;
                }
                if (line.Contains(Strings[59])) {
                    stringBuilder.AppendLine(Strings[60])
                        .AppendLine(Strings[61]);
                    continue;
                }
                if (line.Contains(Strings[62])) {
                    stringBuilder.AppendLine(Strings[63]);
                    continue;
                }
                if (line.Contains(Strings[64])) {
                    stringBuilder.AppendLine(Strings[65])
                        .AppendLine(Strings[66]);
                    continue;
                }
                if (line.Contains(Constants.HtmlTagLinkStart) && line.Contains(Constants.HtmlStylesheet)) {
                    stringBuilder
                        .AppendLine(Regex.Replace(line, Strings[105], string.Format(Strings[109], sToken), RegexOptions.IgnoreCase));
                    continue;
                }
                if (line.Contains(Strings[67])) {
                    stringBuilder.AppendLine(Strings[68])
                        .AppendLine(Strings[69]);
                    continue;
                }
                if (line.Contains(Strings[70])) {
                    stringBuilder
                        .AppendLine(Regex.Replace(line, Strings[106], string.Format(Strings[110], siteName), RegexOptions.IgnoreCase));
                    continue;
                }
                if (line.Contains(Strings[71])) {
                    stringBuilder.AppendFormat(Strings[72], siteName, description)
                        .AppendLine()
                        .AppendFormat(Strings[73], baseUrl + fileName)
                        .AppendLine()
                        .AppendLine(Strings[74])
                        .AppendFormat(Strings[75],
                            dateTime.ToLocalTime().ToString(Constants.CreatedOnDateTimeFormat, CultureInfo.InvariantCulture))
                        .AppendLine()
                        .AppendLine(Strings[76])
                        .AppendLine(Strings[77])
                        .AppendFormat(Strings[78], baseUrl)
                        .AppendLine();
                    continue;
                }
                if (line.Contains(Strings[79])) {
                    stringBuilder
                        .AppendLine(Regex.Replace(line, Strings[106], string.Format(Strings[110], siteName), RegexOptions.IgnoreCase));
                    continue;
                }
                if (line.Contains(Strings[80])) {
                    stringBuilder.AppendLine(Strings[81])
                        .AppendFormat(Strings[82], baseUrl + fileName)
                        .AppendLine();
                    continue;
                }
                if (line.Contains(Strings[83])) {
                    stringBuilder.AppendFormat(Strings[84], siteName, description)
                        .AppendLine()
                        .AppendFormat(Strings[85], baseUrl)
                        .AppendLine()
                        .AppendFormat(Strings[86], baseUrl)
                        .AppendLine()
                        .AppendLine(Strings[87])
                        .AppendLine(Strings[88])
                        .AppendLine(Strings[89])
                        .AppendLine(Strings[90]);
                    continue;
                }
                if (string.IsNullOrWhiteSpace(line)
                        || line.Contains(Strings[11])
                        || line.Contains(Strings[12])
                        || line.Trim().StartsWith(Constants.Slash.ToString() + Constants.Slash.ToString())
                        || line.Contains(Constants.HtmlTagP + Constants.HtmlTagEP)) {

                    continue;
                }
                if (line.Contains(Strings[91])) {
                    stringBuilder.AppendLine(Strings[92]);
                    continue;
                }
                if (line.Contains(Constants.HtmlTagHrStart)) {
                    stringBuilder.AppendLine(Constants.HtmlTagHr);
                    continue;
                }
                if (line.Contains(Strings[93])) {
                    stringBuilder.AppendFormat(Strings[94], counterButton++ % 2 == 0
                            ? Properties.Resources.HtmlContentShowMenu
                            : Properties.Resources.HtmlContentHideMenu)
                        .AppendLine();
                    continue;
                }
                if (line.Contains(Strings[95])) {
                    stringBuilder
                        .AppendLine(Regex.Replace(line, Strings[107], string.Format(Strings[110], siteName), RegexOptions.IgnoreCase));
                    continue;
                } else if (line.Contains(Constants.HtmlTagTitle)) {
                    stringBuilder
                        .AppendLine(Regex.Replace(line, Strings[108], string.Format(Strings[110], siteName), RegexOptions.IgnoreCase));
                    continue;
                }
                if (line.Contains(Strings[96])) {
                    stringBuilder.AppendLine(line.Replace(Strings[97], Strings[98]).Replace(Strings[99], Strings[100]));
                    continue;
                }
                if (line.Contains(Strings[101])) {
                    main = true;
                } else if (line.Contains(Strings[102])
                        || line.Contains(Constants.HtmlCommentStart + Constants.OpeningBracket.ToString())) {

                    skip = true;
                }
                if (main && line.Equals(Constants.HtmlTagEDiv.ToString().PadLeft(18))) {
                    if (!skip) {
                        stringBuilder.AppendLine(Constants.HtmlTagHr);
                    }
                    main = false;
                }
                if (!skip) {
                    if (line.Contains(Strings[103])) {
                        stringBuilder.AppendFormat(Strings[104], siteName)
                            .AppendLine();
                    }
                    line = commentsRegex.Replace(line, string.Empty);
                    if (!string.IsNullOrWhiteSpace(line)) {
                        if (parttag || appendRegex.IsMatch(line)) {
                            stringBuilder.Append((parttag ? Constants.Space.ToString() : string.Empty.PadRight(8)) + line.Trim());
                            parttag = true;
                        } else {
                            stringBuilder.AppendLine(line.TrimEnd());
                        }
                    }
                }
                if (line.Contains(Constants.HtmlTagELi) || line.Contains(Constants.HtmlCommentEnd)) {
                    skip = false;
                }
            }
            return stringBuilder.ToString();
        }

        public async void ProcessAsync() {
            await Task.Run(new Action(() => {
                try {
                    if (!IsSourceDirectoryEmpty()) {
                        EmptyDestinationDirectory();
                        Patch();
                        EmptySourceDirectoryAsync();
                    }
                    Patched?.Invoke(this, new PatchedEventArgs(CountItems()));
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                    Error?.Invoke(this, new ErrorEventArgs(exception));
                }
            }));
        }

        private static void EmptyDirectory(string directoryPath) {
            foreach (string item in Directory.GetDirectories(directoryPath)) {
                Directory.Delete(item, true);
            }
            foreach (string item in Directory.GetFiles(directoryPath)) {
                File.Delete(item);
            }
        }

        private static string FormatMetaRevisedDateTime(DateTime dateTime) {
            StringBuilder stringBuilder = new StringBuilder()
                .Append(dateTime.ToString(Constants.RevisedDateTimeFormat1, CultureInfo.InvariantCulture));
            if (new[] { 11, 12, 13 }.Contains(dateTime.Day)) {
                stringBuilder.Append(Constants.SuffixTh);
            } else if (dateTime.Day % 10 == 1) {
                stringBuilder.Append(Constants.SuffixSt);
            } else if (dateTime.Day % 10 == 2) {
                stringBuilder.Append(Constants.SuffixNd);
            } else if (dateTime.Day % 10 == 3) {
                stringBuilder.Append(Constants.SuffixRd);
            } else {
                stringBuilder.Append(Constants.SuffixTh);
            }
            stringBuilder.Append(dateTime.ToString(Constants.RevisedDateTimeFormat2, CultureInfo.InvariantCulture).ToLowerInvariant());
            return stringBuilder.ToString();
        }

        private static bool IsDirectoryEmpty(string directoryPath) {
            return Directory.GetFiles(directoryPath).Length.Equals(0) && Directory.GetDirectories(directoryPath).Length.Equals(0);
        }

        private static DateTime StartOfWeek(DateTime dateTime, DayOfWeek startOfWeek) {
            int diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
            return dateTime.AddDays(-diff).Date;
        }
    }
}
