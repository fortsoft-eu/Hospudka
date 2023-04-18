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
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hospudka {
    public class FtpWebHandler {
        string parentDirectory;

        public event ErrorEventHandler Error;
        public event EventHandler ContentUploaded;
        public event EventHandler RemoteDirectoryEmpty;
        public event EventHandler<RemoteEventArgs> RemoveItem;
        public event EventHandler<RemoteEventArgs> UploadItem;

        public FtpWebHandler() {
            parentDirectory = new StringBuilder()
                .Append(Constants.Period)
                .Append(Constants.Period)
                .ToString();
        }

        public string Host { get; set; }

        public ushort Port { get; set; } = 21;

        public string RemoteDirectory { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string LocalDirectory { get; set; }

        public string GetUrl() {
            UriBuilder uriBuilder = new UriBuilder() {
                Scheme = Constants.SchemeFtp,
                Host = Host,
                Port = Port,
                Path = RemoteDirectory
            };
            return uriBuilder.Uri.AbsoluteUri;
        }

        private void EmptyRecursive(string remotePath) {
            foreach (RemoteFileInfo remoteFileInfo in GetList(remotePath)) {
                if (remoteFileInfo.Name.Equals(Constants.Period.ToString()) || remoteFileInfo.Name.Equals(parentDirectory)) {
                    continue;
                }
                string path = new StringBuilder()
                    .Append(remotePath)
                    .Append(Constants.Slash)
                    .Append(remoteFileInfo.Name)
                    .ToString();
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;
                request.Credentials = new NetworkCredential(UserName, Password);
                switch (remoteFileInfo.Type) {
                    case RemoteFileInfo.FileType.File:
                        request.Method = WebRequestMethods.Ftp.DeleteFile;
                        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) {
                            using (Stream stream = response.GetResponseStream()) {
                                RemoveItem?.Invoke(this, new RemoteEventArgs(remoteFileInfo.Type, path));
                            }
                        }
                        break;
                    case RemoteFileInfo.FileType.Directory:
                        EmptyRecursive(path);
                        request.Method = WebRequestMethods.Ftp.RemoveDirectory;
                        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) {
                            using (Stream stream = response.GetResponseStream()) {
                                RemoveItem?.Invoke(this, new RemoteEventArgs(remoteFileInfo.Type, path));
                            }
                        }
                        break;
                }
            }
        }

        public async void EmptyRemoteDirectoryAsync() {
            if (string.IsNullOrWhiteSpace(RemoteDirectory)) {
                throw new WebException();
            }
            await Task.Run(new Action(() => {
                try {
                    EmptyRecursive(GetUrl());
                    RemoteDirectoryEmpty?.Invoke(this, EventArgs.Empty);
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                    Error?.Invoke(this, new ErrorEventArgs(exception));
                }
            }));
        }

        private RemoteFileInfo[] GetList(string directoryPath) {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(directoryPath);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;
            request.Credentials = new NetworkCredential(UserName, Password);
            List<RemoteFileInfo> list = new List<RemoteFileInfo>();
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) {
                using (Stream stream = response.GetResponseStream()) {
                    using (StreamReader streamReader = new StreamReader(stream)) {
                        for (string line; (line = streamReader.ReadLine()) != null;) {
                            try {
                                list.Add(new RemoteFileInfo(line));
                            } catch (Exception exception) {
                                Debug.WriteLine(exception);
                                ErrorLog.WriteLine(exception);
                            }
                        }
                    }
                }
            }
            return list.ToArray();
        }

        private void UploadRecursive(string localPath, string remotePath, bool isFile = false, bool noRoot = false) {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remotePath);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;
            request.Credentials = new NetworkCredential(UserName, Password);
            if (isFile) {
                request.Method = WebRequestMethods.Ftp.UploadFile;
                using (FileStream fileStream = File.Open(localPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    using (Stream stream = request.GetRequestStream()) {
                        fileStream.CopyTo(stream);
                        UploadItem?.Invoke(this, new RemoteEventArgs(RemoteFileInfo.FileType.File, remotePath));
                    }
                }
            } else {
                if (noRoot) {
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) {
                        using (Stream stream = response.GetResponseStream()) {
                            UploadItem?.Invoke(this, new RemoteEventArgs(RemoteFileInfo.FileType.Directory, remotePath));
                        }
                    }
                }
                foreach (string directoryPath in Directory.GetDirectories(localPath)) {
                    StringBuilder stringBuilder = new StringBuilder()
                        .Append(remotePath)
                        .Append(Constants.Slash)
                        .Append(Path.GetFileName(directoryPath));
                    UploadRecursive(directoryPath, stringBuilder.ToString(), false, true);
                }
                foreach (string filePath in Directory.GetFiles(localPath)) {
                    StringBuilder stringBuilder = new StringBuilder()
                        .Append(remotePath)
                        .Append(Constants.Slash)
                        .Append(Path.GetFileName(filePath));
                    UploadRecursive(filePath, stringBuilder.ToString(), true);
                }
            }
        }

        public async void UploadLocalDirectoryAsync() {
            if (string.IsNullOrWhiteSpace(RemoteDirectory)) {
                throw new WebException();
            }
            await Task.Run(new Action(() => {
                try {
                    UploadRecursive(LocalDirectory, GetUrl());
                    ContentUploaded?.Invoke(this, EventArgs.Empty);
                } catch (Exception exception) {
                    Debug.WriteLine(exception);
                    ErrorLog.WriteLine(exception);
                    Error?.Invoke(this, new ErrorEventArgs(exception));
                }
            }));
        }
    }
}
