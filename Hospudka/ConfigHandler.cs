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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hospudka {
    public class ConfigHandler {
        private Settings settings;

        public ConfigHandler(Settings settings) {
            this.settings = settings;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            GetConfig();
        }

        private string Decrypt(string str, string password) {
            byte[] bytes = Convert.FromBase64String(str);
            SymmetricAlgorithm symmetricAlgorithm = Aes.Create();
            symmetricAlgorithm.Key = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
            symmetricAlgorithm.IV = new byte[16];
            using (MemoryStream memoryStream = new MemoryStream()) {
                using (CryptoStream cryptoStream = new CryptoStream(
                        memoryStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write)) {

                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                    bytes = memoryStream.ToArray();
                    int length = bytes.Length - 16;
                    if (length < 0) {
                        throw new ApplicationException(Properties.Resources.MessageMissingHashError);
                    }
                    byte[] hash = new byte[16];
                    Array.Copy(bytes, length, hash, 0, 16);
                    if (!hash.SequenceEqual(MD5.Create().ComputeHash(bytes, 0, length))) {
                        throw new ApplicationException(Properties.Resources.MessageInvalidHashError);
                    }
                    return Encoding.UTF8.GetString(bytes, 0, length);
                }
            }
        }

        private static string Encrypt(string str, string password) {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] hash = MD5.Create().ComputeHash(bytes);
            SymmetricAlgorithm symmetricAlgorithm = Aes.Create();
            symmetricAlgorithm.Key = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
            symmetricAlgorithm.IV = new byte[16];
            using (MemoryStream memoryStream = new MemoryStream()) {
                using (CryptoStream cryptoStream = new CryptoStream(
                        memoryStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write)) {

                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.Write(hash, 0, hash.Length);
                    cryptoStream.FlushFinalBlock();
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        private void GetConfig() {
            using (HttpClient httpClient = new HttpClient()) {
                StringBuilder stringBuilder = new StringBuilder()
                    .Append(Properties.Resources.Website.TrimEnd(Constants.Slash).ToLowerInvariant())
                    .Append(Constants.Slash)
                    .Append(Application.ProductName.ToLowerInvariant())
                    .Append(Constants.Slash)
                    .Append(Constants.RemoteApiScriptName)
                    .Append(Constants.QuestionMark)
                    .Append(Constants.RemoteVariableNameGet)
                    .Append(Constants.EqualSign)
                    .Append(Constants.RemoteApplicationConfig);
                string config = httpClient.GetStringAsync(stringBuilder.ToString()).GetAwaiter().GetResult();
                settings.Config = Decrypt(config, settings.ConfigHash);
            }
        }

        public async Task<string> SaveConfigAsync() {
            using (HttpClient httpClient = new HttpClient()) {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add(Constants.RemoteApplicationConfig, Encrypt(settings.Config, settings.ConfigHash));
                FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(dictionary);
                StringBuilder stringBuilder = new StringBuilder()
                    .Append(Properties.Resources.Website.TrimEnd(Constants.Slash).ToLowerInvariant())
                    .Append(Constants.Slash)
                    .Append(Application.ProductName.ToLowerInvariant())
                    .Append(Constants.Slash)
                    .Append(Constants.RemoteApiScriptName)
                    .Append(Constants.QuestionMark)
                    .Append(Constants.RemoteVariableNameSet)
                    .Append(Constants.EqualSign)
                    .Append(Constants.RemoteApplicationConfig);
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(stringBuilder.ToString(), formUrlEncodedContent);
                return await httpResponseMessage.Content.ReadAsStringAsync();
            }
        }
    }
}
