using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MD.Translations
{
    internal static class Extensions
    {
        public static bool IsNullOrEmpty(this string src) => String.IsNullOrEmpty(src);
        public static byte[] ToByteArray(this string str) => Encoding.UTF8.GetBytes(str);
        public static string MaxLength(this string source, int maxLength)
        {
            if (String.IsNullOrEmpty(source)
                || source.Length < maxLength) return source;
            return source.Substring(0, maxLength);
        }

        public static string ToUrlPrefix(this string source, int maxLength = 120)
        {
            var str = source?.Replace(' ', '-').ReplaceDiacritics() ?? "";
            str = Regex.Replace(str, "[^a-zA-Z0-9-]", "");
            str = Regex.Replace(str, "[-]+", "-").Trim('-');
            return (str.Length > 0 ? str.MaxLength(maxLength - 1) + "-" : "");
        }

        public static string SearchNormalize(this string source)
        {
            string sourceInFormD = source.Normalize(NormalizationForm.FormKD).ToLower();

            var output = new StringBuilder();
            foreach (char c in sourceInFormD)
            {
                if (c >= 48 && c <= 57) { output.Append(c); continue; } // 0-9 - 30-39
                if (c >= 97 && c <= 122) { output.Append(c); continue; } // 0-9 - 61-7a
            }

            return output.ToString();
        }
        public static string ReplaceDiacritics(this string source)
        {
            string sourceInFormD = source.Normalize(NormalizationForm.FormD);

            var output = new StringBuilder();
            foreach (char c in sourceInFormD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                    output.Append(c);
            }

            return (output.ToString().Normalize(NormalizationForm.FormC));
        }

        public static Uri GetAbsoluteUri(this HttpRequest request)
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Port = request.Host.Port.GetValueOrDefault(80),
                Path = request.Path.ToString(),
                Query = request.QueryString.ToString()
            };
            return uriBuilder.Uri;
        }

        public static string RemoveNonAlphaNumChar(this string src, bool keepSpaces = false) => keepSpaces ? src.RemoveInPattern("[^a-zA-Z0-9 ]") : src.RemoveInPattern("[^a-zA-Z0-9]");
        /// <summary>
        /// Warning: space only at the end of the pattern
        /// </summary>
        public static string RemoveInPattern(this string src, string pattern = "[^a-zA-Z0-9- ]")
        {
            if (src == null) return null;
            Regex rgx = new Regex(pattern);
            return rgx.Replace(src, "");
        }

    }
}
