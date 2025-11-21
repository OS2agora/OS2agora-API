using System.Collections.Generic;
using System.Linq;

namespace Agora.Api.Mappings
{
    public static class IncludesMapper
    {
        /// <summary>
        /// Splits supplied string into substrings seperated by <c>','</c>. If the supplied string is <c>null</c>, then
        /// an empty list of substrings is returned. 
        /// </summary>
        /// <param name="includes">The string to split.</param>
        /// <returns>A list of substrings.</returns>
        public static List<string> MapToIncludeList(this string includes)
        {
            return includes?.Split(',').ToList() ?? new List<string>();
        }
    }
}