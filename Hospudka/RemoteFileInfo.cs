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
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FortSoft.Tools {

    public class RemoteFileInfo {

        /// <summary>
        /// Constants.
        /// </summary>
        private const char Hyphen = '-';
        private const char LowerCaseD = 'd';
        private const char LowerCaseL = 'l';
        private const char Space = ' ';

        public RemoteFileInfo(string lsOutputLine) {
            StringBuilder stringBuilder = new StringBuilder();
            List<string> list = new List<string>();
            char[] charArray = lsOutputLine.ToCharArray();
            for (int i = 0, j = 0; i < charArray.Length; i++) {
                if (charArray[i].Equals(Space)) {
                    if (stringBuilder.Length > 0) {
                        list.Add(stringBuilder.ToString());
                        stringBuilder = new StringBuilder();
                    }
                } else if ((j++).Equals(0)) {
                    switch (charArray[i]) {
                        case Hyphen:
                            Type = FileType.File;
                            break;
                        case LowerCaseD:
                            Type = FileType.Directory;
                            break;
                        case LowerCaseL:
                            Type = FileType.SoftLink;
                            break;
                        default:
                            throw new FormatException();
                    }
                } else {
                    stringBuilder.Append(charArray[i]);
                }
            }
            if (stringBuilder.Length > 0) {
                list.Add(stringBuilder.ToString());
            }
            if (list.Count.Equals(0)) {
                throw new FormatException();
            }
            Name = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            if (list.Count.Equals(0)) {
                throw new FormatException();
            }
            string time = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            if (list.Count.Equals(0)) {
                throw new FormatException();
            }
            stringBuilder = new StringBuilder();
            for (int i = 0; i < list.Count; i++) {
                switch (i) {
                    case 0:
                        Permissions = list[i];
                        break;
                    case 1:
                        uint hardLinkCount;
                        if (uint.TryParse(list[i], out hardLinkCount)) {
                            HardLinkCount = hardLinkCount;
                        }
                        break;
                    case 2:
                        Owner = list[i];
                        break;
                    case 3:
                        Group = list[i];
                        break;
                    case 4:
                        uint fileSize;
                        if (uint.TryParse(list[i], out fileSize)) {
                            FileSize = fileSize;
                        }
                        break;
                    default:
                        stringBuilder.Append(list[i])
                            .Append(Space);
                        break;
                }
            }
            if (stringBuilder.Length.Equals(0)) {
                throw new FormatException();
            }
            stringBuilder.Append(time);
            string[] formats = new string[] {
                "MMM d H:mm",
                "MMM d H:mm:ss",
                "MMM d yyyy",
                "MMM d yyyy H:mm",
                "MMM d yyyy H:mm:ss",
                "MMM d yyyy",
                "d MMM yyyy",
                "d MMM yyyy H:mm",
                "d MMM yyyy H:mm:ss",
                "M/d/yyyy H:mm",
                "M/d/yyyy H:mm:ss",
                "d/M/yyyy H:mm",
                "d/M/yyyy H:mm:ss",
                "d.M.yyyy H:mm",
                "d.M.yyyy H:mm:ss",
                "M.d.yyyy H:mm",
                "M.d.yyyy H:mm:ss",
                "d-M-yyyy H:mm",
                "d-M-yyyy H:mm:ss",
                "M-d-yyyy H:mm",
                "M-d-yyyy H:mm:ss",
                "yyyy-M-d H:mm",
                "yyyy-M-d H:mm:ss"
            };
            DateTime dateTime;
            if (DateTime.TryParseExact(stringBuilder.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowInnerWhite,
                    out dateTime)) {

                LastModified = dateTime;
            }
        }

        public DateTime LastModified { get; private set; }

        public FileType Type { get; private set; }

        public string Group { get; private set; }

        public string Name { get; private set; }

        public string Owner { get; private set; }

        public string Permissions { get; private set; }

        public uint FileSize { get; private set; }

        public uint HardLinkCount { get; private set; }

        public override string ToString() {
            return new StringBuilder()
                .Append("Type: ")
                .Append(Type)
                .AppendLine()
                .Append("Permissions: ")
                .AppendLine(Permissions)
                .Append("HardLinkCount: ")
                .Append(HardLinkCount)
                .AppendLine()
                .Append("Owner: ")
                .AppendLine(Owner)
                .Append("Group: ")
                .AppendLine(Group)
                .Append("FileSize: ")
                .Append(FileSize)
                .AppendLine()
                .Append("LastModified: ")
                .AppendLine(LastModified.ToString("yyyy-MM-dd HH:mm:ss"))
                .Append("Name: ")
                .Append(Name)
                .ToString();
        }

        public enum FileType {
            File,
            Directory,
            SoftLink
        }
    }
}
