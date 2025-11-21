using Agora.Models.Models.Multiparts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Agora.Operations.Models.Comments.Commands
{
    public static class CommentCommandUtils
    {
        // Formatting filename due limitation to SBSYS and DataScanner implementations
        public static string FormatFileName(string fileName)
        {
            var DateTimeFormat = "yyyyMMddTHH-mm";
            var dateTimeString = DateTime.Now.ToString(DateTimeFormat);
            var formattedFileName = RemoveDiacritics(fileName
                .Replace("æ", "ae")
                .Replace("ø", "oe")
                .Replace("å", "aa"));

            return $"{dateTimeString}_{formattedFileName}";
        }

        // Removes accents from string (e.g. ö, ó, ò, ô and so forth)
        private static string RemoveDiacritics(string s)
        {
            string normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
