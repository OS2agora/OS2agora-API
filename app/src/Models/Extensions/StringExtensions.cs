using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace BallerupKommune.Models.Extensions
{
    public static class StringExtensions
    {
        public static string ToLowerCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value) || char.IsLower(value, 0))
            {
                return value;
            }

            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        public static string ToTitelCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var textInfo = new CultureInfo("da-DK", false).TextInfo;
            return textInfo.ToTitleCase(value);
        }

        public static string RemoveAllNonNumbers(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var strippedString = Regex.Replace(value, @"[^0-9]", "");
            return strippedString;
        }

        public static string UrlEncode(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return HttpUtility.UrlEncode(value);
            }

            return string.Empty;
        }
    }
}