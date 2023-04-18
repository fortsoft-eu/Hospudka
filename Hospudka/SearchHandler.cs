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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Hospudka {
    public class SearchHandler {
        private List<Search> searches;
        private string searchFilePath;

        public event EventHandler Deleted;
        public event EventHandler Loaded;
        public event EventHandler Saved;

        public SearchHandler(string searchFileName) {
            searches = new List<Search>();
            searchFilePath = Path.Combine(Path.GetDirectoryName(Application.LocalUserAppDataPath), searchFileName);
        }

        public int MaximumSearches => Constants.MaximumSearches;

        public void Add(Search search) {
            List<Search> searches = new List<Search>();
            searches.Add(search);
            int count = 1;
            foreach (Search s in this.searches) {
                if (++count > Constants.MaximumSearches) {
                    break;
                }
                if (!s.searchString.Equals(search.searchString)) {
                    searches.Add(s);
                }
            }
            this.searches = searches;
        }

        public void Delete() {
            try {
                if (File.Exists(searchFilePath)) {
                    File.Delete(searchFilePath);
                }
                if (!File.Exists(searchFilePath)) {
                    Deleted?.Invoke(this, EventArgs.Empty);
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        public bool EqualsToLast(Search search) {
            if (searches.Count < 1) {
                return false;
            }
            if (search.Equals(searches[0])) {
                return true;
            }
            return false;
        }

        public string[] Get() {
            List<string> searchStrings = new List<string>(searches.Count);
            foreach (Search search in searches) {
                searchStrings.Add(search.searchString);
            }
            return searchStrings.ToArray();
        }

        public Search Get(string searchString) {
            foreach (Search search in searches) {
                if (search.searchString.Equals(searchString)) {
                    return search;
                }
            }
            return new Search();
        }

        public void Load() {
            try {
                if (File.Exists(searchFilePath)) {
                    using (FileStream fileStream = File.OpenRead(searchFilePath)) {
                        Search[] searches = (Search[])new BinaryFormatter().Deserialize(fileStream);
                        this.searches = searches.ToList();
                    }
                    Loaded?.Invoke(this, EventArgs.Empty);
                }
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }

        public void Save() {
            try {
                using (FileStream fileStream = File.Create(searchFilePath)) {
                    new BinaryFormatter().Serialize(fileStream, searches.ToArray());
                }
                Saved?.Invoke(this, EventArgs.Empty);
            } catch (Exception exception) {
                Debug.WriteLine(exception);
                ErrorLog.WriteLine(exception);
            }
        }
    }
}
