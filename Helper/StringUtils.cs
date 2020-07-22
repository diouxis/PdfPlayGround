using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PdfPlayGround.Helper
{
    public static class StringUtils
    {
        public static string ExtractByRegexPattern(string input, string pattern, RegexOptions options = RegexOptions.None)
        {
            Match extractMatch = Regex.Match(input, pattern, options);
            if (extractMatch.Success && extractMatch.Groups.Count > 1)
            {
                return extractMatch.Groups[1].Value;
            }
            return null;
        }

        public static string HtmlEncode(object obj)
        {
            string text = System.Web.HttpUtility.HtmlEncode(obj);
            text = Regex.Replace(text, @"^\s", "&nbsp;", RegexOptions.Multiline);
            return text.Replace(Environment.NewLine, "<br />");
        }

        public static bool IsValidPersonName(string str)
        {
            string invalidCharPattern = @"[^\s0-9A-Za-zÀ-ÖØ-öø-ÿ\.\'\-\&]";
            return !Regex.IsMatch(str, invalidCharPattern);
        }

        public static bool IsValidPhoneNumber(string str)
        {
            string invalidCharPattern = @"[^\s0-9+,\-\(\)]";
            return !Regex.IsMatch(str, invalidCharPattern);
        }

        public static bool IsValidEmail(string str)
        {
            try
            {
                foreach(var email in str.Split(','))
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    if (addr.Address != email.Trim()) { return false; }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public static class StringUtilsExtensions
    {
        public static string ToBase64(this string str)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string FromBase64(this string strBase64)
        {
            var base64EncodedBytes = Convert.FromBase64String(strBase64);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string ExtractByRegexPattern(this string str, string pattern, RegexOptions options = RegexOptions.None) => StringUtils.ExtractByRegexPattern(str, pattern, options);

    }
}
