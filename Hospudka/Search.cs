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
 * Version 1.0.0.1
 */

using System;
using System.Runtime.Serialization;

namespace Hospudka {

    [Serializable]
    public struct Search {
        public string searchString;
        public bool caseSensitive;
        public bool backward;
        public bool startsWith;
        public bool endsWith;
        public bool regularExpression;

        public Search(
                string searchString,
                bool caseSensitive,
                bool backward,
                bool startsWith,
                bool endsWith,
                bool regularExpression) {

            this.searchString = searchString;
            this.caseSensitive = caseSensitive;
            this.backward = backward;
            this.startsWith = startsWith;
            this.endsWith = endsWith;
            this.regularExpression = regularExpression;
        }

        private Search(SerializationInfo info, StreamingContext ctxt) {
            searchString = (string)info.GetValue("SearchString", typeof(string));
            caseSensitive = (bool)info.GetValue("CaseSensitive", typeof(bool));
            backward = (bool)info.GetValue("Backward", typeof(bool));
            startsWith = (bool)info.GetValue("StartsWith", typeof(bool));
            endsWith = (bool)info.GetValue("EndsWith", typeof(bool));
            regularExpression = (bool)info.GetValue("RegularExpression", typeof(bool));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
            info.AddValue("SearchString", searchString);
            info.AddValue("CaseSensitive", caseSensitive);
            info.AddValue("Backward", backward);
            info.AddValue("StartsWith", startsWith);
            info.AddValue("EndsWith", endsWith);
            info.AddValue("RegularExpression", regularExpression);
        }

        public bool Equals(Search search) {
            return search.searchString == searchString
                && search.caseSensitive.Equals(caseSensitive)
                && search.backward.Equals(backward)
                && search.regularExpression.Equals(regularExpression)
                && search.startsWith.Equals(startsWith)
                && search.endsWith.Equals(endsWith);
        }
    }
}
