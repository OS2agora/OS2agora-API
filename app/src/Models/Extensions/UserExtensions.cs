using BallerupKommune.Models.Models;
using System.Text.RegularExpressions;

namespace BallerupKommune.Models.Extensions
{
    public static class UserExtensions
    {
        public static string MaskUserIdentifier(this User user)
        {
            return MaskIfContainingCpr(user.PersonalIdentifier);
        }

        public static string MaskCpr(this User user)
        {
            var cpr = user?.Cpr;

            if (cpr != null)
            {
                var maskedCpr = cpr.Substring(0, 6) + "XXXX";
                return maskedCpr;
            }

            return null;
        }

        public static string MaskIfContainingCpr(string identifier)
        {
            string cprPattern = @"^\d{6}\d{4}$";
            string cvrCprPattern = @"^\d{8}-\d{6}\d{4}$";
            string cprCvrPattern = @"^\d{6}\d{4}-\d{8}$";
            string guidPattern = @"^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$";

            if (Regex.IsMatch(identifier, guidPattern))
            {
                return identifier;
            }

            if (Regex.IsMatch(identifier, cprPattern))
            {
                return Regex.Replace(identifier, @"\d{4}$", "xxxx");
            }

            if (Regex.IsMatch(identifier, cvrCprPattern))
            {
                return Regex.Replace(identifier, @"(?<=-\d{6})\d{4}$", "xxxx");
            }

            if (Regex.IsMatch(identifier, cprCvrPattern))
            {
                return Regex.Replace(identifier, @"(?<=^\d{6})\d{4}(?=-)", "xxxx");
            }

            return identifier;
        }
    }
}