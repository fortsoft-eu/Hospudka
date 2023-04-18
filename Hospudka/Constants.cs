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

namespace Hospudka {

    /// <summary>
    /// Constants used in many places in the application.
    /// </summary>
    public static class Constants {

        /// <summary>
        /// Maximum number of previous searches in the combobox.
        /// </summary>
        public const int MaximumSearches = 30;

        /// <summary>
        /// Maximum internal width in pixels for calculations of the ProgressBar
        /// in the StatusStrip regardless of displayed size.
        /// </summary>
        public const int StripProgressBarInternalMax = 360;

        /// <summary>
        /// The time interval in milliseconds for smooth ProgressBar shift in the
        /// StatusStrip.
        /// </summary>
        public const int StripProgressBarInterval = 30;

        /// <summary>
        /// Width limit in pixels for displaying the ProgressBar in the
        /// StatusStrip.
        /// </summary>
        public const int StripProgressBarVLimit = 1200;

        /// <summary>
        /// Ratio for displaying the ProgressBar in the StatusStrip according to
        /// its width.
        /// </summary>
        public const int StripProgressBarWRatio = 18;

        /// <summary>
        /// Width limit in pixels for displaying the browser cache size label in
        /// the StatusStrip.
        /// </summary>
        public const int StripStatusLblCacheVLimit = 1000;

        /// <summary>
        /// Width limit in pixels for displaying the reduced browser cache size
        /// label in the StatusStrip.
        /// </summary>
        public const int StripStatusLblCacheVLimitReduced = 600;

        /// <summary>
        /// Width limit in pixels for displaying the search results label in the
        /// StatusStrip.
        /// </summary>
        public const int StripStatusLblSearchResVLimit = 800;

        /// <summary>
        /// Width limit in pixels for displaying the URL label ProgressBar in the
        /// StatusStrip.
        /// </summary>
        public const int StripStatusLblUrlVLimit = 600;

        /// <summary>
        /// The time interval in seconds after which the monitors will be turned
        /// off after entering the command to turn off the monitors.
        /// </summary>
        public const int TurnOffTheMonitorsInterval = 5;

        /// <summary>
        /// Tabs in the log files will be replaced with the following number of
        /// spaces for displaying in the log viewer window and when printing.
        /// </summary>
        public const int VerticalTabNumberOfSpaces = 3;

        /// <summary>
        /// Windows API constants.
        /// </summary>
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MONITORPOWER = 0xF170;
        public const int SC_SCREENSAVE = 0xF140;
        public const int SC_TASKLIST = 0xF130;
        public const int WM_CLEAR = 0x0303;
        public const int WM_COPY = 0x0301;
        public const int WM_CUT = 0x0300;
        public const int WM_HSCROLL = 0x114;
        public const int WM_PASTE = 0x0302;
        public const int WM_SYSCOMMAND = 0x112;

        /// <summary>
        /// Characters used in many places in the application code.
        /// </summary>
        public const char Ampersand = '&';
        public const char Asterisk = '*';
        public const char CarriageReturn = '\r';
        public const char ClosingCurly = '}';
        public const char Colon = ':';
        public const char Comma = ',';
        public const char Ellipsis = '…';
        public const char EmDash = '—';
        public const char EnDash = '–';
        public const char EqualSign = '=';
        public const char GreaterThan = '>';
        public const char Hyphen = '-';
        public const char LineFeed = '\n';
        public const char MinusSign = '−';
        public const char OpeningBracket = '[';
        public const char OpeningCurly = '{';
        public const char Percent = '%';
        public const char Period = '.';
        public const char QuestionMark = '?';
        public const char QuotationMark = '"';
        public const char Semicolon = ';';
        public const char Slash = '/';
        public const char Space = ' ';
        public const char Underscore = '_';
        public const char VerticalBar = '|';
        public const char VerticalTab = '\t';
        public const char Zero = '0';

        /// <summary>
        /// Strings used in many places in the application code.
        /// </summary>
        public const string AppendPattern = "^\\s+<div\\s+.*\".*\"\\s?$";
        public const string BlankPageUri = "about:blank";
        public const string CalcFileName = "calc.exe";
        public const string CharMapFileName = "charmap.exe";
        public const string CommandLineSwitchUD = "-d";
        public const string CommandLineSwitchUE = "-e";
        public const string CommandLineSwitchWD = "/d";
        public const string CommandLineSwitchWE = "/e";
        public const string ConfigFtpDirectory = "FTP_PATH";
        public const string ConfigFtpHost = "FTP_HOST";
        public const string ConfigFtpPort = "FTP_PORT";
        public const string ConfigPassword = "PASSWORD";
        public const string ConfigResourcesPath = "RES_PATH";
        public const string ConfigSearchFileName = "ConfigSearch.dat";
        public const string ConfigSourcePath = "SRC_PATH";
        public const string ConfigStrings = "STRINGS";
        public const string ConfigUserName = "USERNAME";
        public const string ConfigWebLocalPath = "WEB_PATH";
        public const string CreatedOnDateTimeFormat = "MMMM dd, yyyy";
        public const string CssCommentsPattern = "/\\*.*\\*/";
        public const string DumpFileNameTimeFormat = "yyyy-MM-dd_HHmmss_fff";
        public const string ErrorLogEmptyString = "[Empty String]";
        public const string ErrorLogErrorMessage = "ERROR MESSAGE";
        public const string ErrorLogFileName = "Error.log";
        public const string ErrorLogNull = "[null]";
        public const string ErrorLogTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        public const string ErrorLogWhiteSpace = "[White Space]";
        public const string ExtBrowsChromeExecRelPath = "Google\\Chrome\\Application\\chrome.exe";
        public const string ExtBrowsChromeLnkFileName = "Google Chrome.lnk";
        public const string ExtBrowsFirefoxExecRelPath = "Mozilla Firefox\\Firefox.exe";
        public const string ExtBrowsFirefoxLnkFileName = "Firefox.lnk";
        public const string ExtensionBmp = ".bmp";
        public const string ExtensionCss = ".css";
        public const string ExtensionFilterBmp = "Windows Bitmap BMP (*.bmp)|*.bmp";
        public const string ExtensionFilterGif = "CompuServe GIF 89a (*.gif)|*.gif";
        public const string ExtensionFilterJpg = "JPEG File Interchange Format (*.jpg)|*.jpg";
        public const string ExtensionFilterPng = "Portable Network Graphics PNG (*.png)|*.png";
        public const string ExtensionFilterTif = "Tagged Image File Format TIFF (*.tif)|*.tif";
        public const string ExtensionFilterWebP = "Google WebP (*.webp)|*.webp";
        public const string ExtensionGif = ".gif";
        public const string ExtensionHtml = ".html";
        public const string ExtensionJpg = ".jpg";
        public const string ExtensionJs = ".js";
        public const string ExtensionJson = ".json";
        public const string ExtensionPng = ".png";
        public const string ExtensionTif = ".tif";
        public const string ExtensionTxt = ".txt";
        public const string ExtensionWebP = ".webp";
        public const string HtmlCommentEnd = "-->";
        public const string HtmlCommentsPattern = "\\s*<!--.*-->\\s*$";
        public const string HtmlCommentStart = "<!--";
        public const string HtmlEntityNbsp = "&nbsp;";
        public const string HtmlFileExtension = "*.html";
        public const string HtmlStylesheet = "stylesheet";
        public const string HtmlTag = "<html>";
        public const string HtmlTagEDiv = "</div>";
        public const string HtmlTagEH4 = "</h4>";
        public const string HtmlTagELi = "</li>";
        public const string HtmlTagEP = "</p>";
        public const string HtmlTagHr = "<hr>";
        public const string HtmlTagHrStart = "<hr ";
        public const string HtmlTagLi = "<li>";
        public const string HtmlTagLinkStart = "<link ";
        public const string HtmlTagP = "<p>";
        public const string HtmlTagStyle = "<style>";
        public const string HtmlTagTitle = "<title>";
        public const string IetfLanguageTagEnUs = "en-US";
        public const string JSCommentsPattern = "\\s*/\\*.*\\*/\\s+";
        public const string LibWebPX64FileName = "libwebp_x64.dll";
        public const string LibWebPX86FileName = "libwebp_x86.dll";
        public const string LogoFileName = "logo.png";
        public const string MonospaceFontName = "Consolas";
        public const string Muted = "Muted";
        public const string NotepadFileName = "notepad.exe";
        public const string NumberFormatSystem = "[system]";
        public const string OneDecimalDigitFormat = "f1";
        public const string PrintOutputInput = "Input";
        public const string PrintOutputOutput = "Output";
        public const string RemoteApiScriptName = "api.php";
        public const string RemoteApplicationConfig = "ApplicationConfig";
        public const string RemoteClientRemoteAddress = "ClientRemoteAddress";
        public const string RemoteProductLatestVersion = "ProductLatestVersion";
        public const string RemoteVariableNameGet = "get";
        public const string RemoteVariableNameSet = "set";
        public const string RevisedDateTimeFormat1 = "dddd, MMMM dd";
        public const string RevisedDateTimeFormat2 = ", yyyy, h:mm tt";
        public const string RFC1123DateTimeFormat = "r";
        public const string SchemeFtp = "ftp";
        public const string SchemeHttps = "https";
        public const string SendKeysDelete = "{DELETE}";
        public const string SendKeysDown = "{DOWN}";
        public const string SendKeysEnd = "{END}";
        public const string SendKeysHome = "{HOME}";
        public const string SendKeysPgDn = "{PGDN}";
        public const string SendKeysPgUp = "{PGUP}";
        public const string SendKeysTab = "{TAB}";
        public const string SendKeysUp = "{UP}";
        public const string ShortcutAltShiftCtrlP = "\tAlt+Shift+Ctrl+P";
        public const string ShortcutDelete = "\tDelete";
        public const string ShortcutShiftCtrlL = "\tShift+Ctrl+L";
        public const string ShortcutShiftCtrlN = "\tShift+Ctrl+N";
        public const string ShortcutShiftCtrlP = "\tShift+Ctrl+P";
        public const string ShortcutShiftCtrlT = "\tShift+Ctrl+T";
        public const string StatusOk = "Ok";
        public const string StripSearchFormat = "{0}/{1} {2}";
        public const string StripSearchMatches = "matches";
        public const string StripSearchMatchesShort1 = "match";
        public const string StripSearchMatchesShort2 = "m.";
        public const string StripSearchNotFound = "String not found";
        public const string SubstitutionPattern = "^[a-z].*\\.html";
        public const string SuffixNd = "nd";
        public const string SuffixRd = "rd";
        public const string SuffixSt = "st";
        public const string SuffixTh = "th";
        public const string ThreeDots = "...";
        public const string UnitPrefixBinary = "Binary";
        public const string UnitPrefixDecimal = "Decimal";
        public const string UriFragment = "Fragment";
        public const string UriHost = "Host";
        public const string UriPassword = "Password";
        public const string UriPath = "Path";
        public const string UriPort = "Port";
        public const string UriQuery = "Query String";
        public const string UriScheme = "Scheme";
        public const string UriUserInfo = "User Info";
        public const string UriUserName = "User Name";
        public const string UriVariables = "Variables";
        public const string VersionRegexPattern = "^\\d+\\.\\d+\\.\\d+\\.\\d$";
        public const string WordPadFileName = "write.exe";
        public const string XmlElementRemoteAddress = "RemoteAddress";
        public const string XmlElementStatus = "Status";
        public const string XmlElementVersion = "Version";
        public const string ZeroDecimalDigitsFormat = "f0";

        /// <summary>
        /// Data size units with their binary and decimal prefixes.
        /// </summary>
        public const string Byte = "B";
        public const string Kibibyte = "KiB";
        public const string Kilobyte = "kB";
        public const string Mebibyte = "MiB";
        public const string Megabyte = "MB";
        public const string Gibibyte = "GiB";
        public const string Gigabyte = "GB";
        public const string Tebibyte = "TiB";
        public const string Terabyte = "TB";
        public const string Pebibyte = "PiB";
        public const string Petabyte = "PB";
        public const string Exbibyte = "EiB";
        public const string Exabyte = "EB";
        public const string Zebibyte = "ZiB";
        public const string Zettabyte = "ZB";
        public const string Yobibyte = "YiB";
        public const string Yottabyte = "YB";
        public const string Robibyte = "RiB";
        public const string Ronnabyte = "RB";
        public const string Qubibyte = "QiB";
        public const string Quettabyte = "QB";
    }
}
