using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PdfPlayGround
{
    public static class FileUtil
    {
        public static string ConvertToValidFilename(string str)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return Regex.Replace(str, invalidRegStr, "_");
        }
    }

    public static class FileUtilExtension
    {
        public static string ToValidFilename(this string str) => FileUtil.ConvertToValidFilename(str);
    }
}
