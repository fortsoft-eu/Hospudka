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
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FortSoft.Tools {
    public sealed class RandomGenerator : IDisposable {
        private const int byteSize = 0x100;

        private RandomNumberGenerator randomNumberGenerator;

        public RandomGenerator() : this(0, null) { }

        public RandomGenerator(int stringLength) : this(stringLength, null) { }

        public RandomGenerator(int stringLength, char[] characterSet) {
            CharacterSet = characterSet;
            StringLength = stringLength;
            randomNumberGenerator = RandomNumberGenerator.Create();
        }

        public RandomGenerator(int stringLength, bool lowerCase) : this(stringLength, lowerCase, false) { }

        public RandomGenerator(int stringLength, bool lowerCase, bool upperCase) : this(stringLength, lowerCase, upperCase, false) { }

        public RandomGenerator(int stringLength, bool lowerCase, bool upperCase, bool numbers)
            : this(stringLength, lowerCase, upperCase, numbers, false) { }

        public RandomGenerator(int stringLength, bool lowerCase, bool upperCase, bool numbers, bool otherAscii)
            : this(stringLength, lowerCase, upperCase, numbers, otherAscii, null) { }

        public RandomGenerator(int stringLength, bool lowerCase, bool upperCase, bool numbers, bool otherAscii, char[] additionalChars) {
            NewSet(lowerCase, upperCase, numbers, otherAscii, additionalChars);
            StringLength = stringLength;
            randomNumberGenerator = RandomNumberGenerator.Create();
        }

        public char[] CharacterSet { get; set; }

        public int StringLength { get; set; }

        public void Dispose() => randomNumberGenerator.Dispose();

        public string Generate() {
            if (CharacterSet.Length.Equals(0)) {
                throw new ApplicationException("The character set may not be empty.");
            }
            char[] characterSet = new HashSet<char>(CharacterSet).ToArray();
            if (byteSize < characterSet.Length) {
                throw new ApplicationException(string.Format("The character set may contain no more than {0} characters.", byteSize));
            }
            StringBuilder stringBuilder = new StringBuilder();
            byte[] buffer = new byte[128];
            while (stringBuilder.Length < StringLength) {
                randomNumberGenerator.GetBytes(buffer);
                for (int i = 0; i < buffer.Length && stringBuilder.Length < StringLength; ++i) {
                    int outOfRangeStart = byteSize - (byteSize % characterSet.Length);
                    if (outOfRangeStart > buffer[i]) {
                        stringBuilder.Append(characterSet[buffer[i] % characterSet.Length]);
                    }
                }
            }
            return stringBuilder.ToString();

        }

        public void NewSet() => NewSet(true, false);

        public void NewSet(bool lowerCase, bool upperCase) => NewSet(lowerCase, upperCase, false);

        public void NewSet(bool lowerCase, bool upperCase, bool numbers) => NewSet(lowerCase, upperCase, numbers, false);

        public void NewSet(bool lowerCase, bool upperCase, bool numbers, bool otherAscii) {
            NewSet(lowerCase, upperCase, numbers, otherAscii, null);
        }

        public void NewSet(bool lowerCase, bool upperCase, bool numbers, bool otherAscii, char[] additionalChars) {
            List<char> list = new List<char>();
            if (lowerCase) {
                for (char c = 'a'; c <= 'z'; c++) {
                    list.Add(c);
                }
            }
            if (upperCase) {
                for (char c = 'A'; c <= 'Z'; c++) {
                    list.Add(c);
                }
            }
            if (numbers) {
                for (char c = '0'; c <= '9'; c++) {
                    list.Add(c);
                }
            }
            if (otherAscii) {
                for (char c = '!'; c <= '~'; c++) {
                    if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9') {
                        continue;
                    }
                    list.Add(c);
                }
            }
            if (additionalChars != null) {
                foreach (char c in additionalChars) {
                    if (!list.Contains(c)) {
                        list.Add(c);
                    }
                }
            }
            CharacterSet = list.ToArray();
        }
    }
}
