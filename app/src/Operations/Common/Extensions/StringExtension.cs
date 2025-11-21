using Agora.Operations.Common.Exceptions;
using System;
using System.Linq;
using System.Net.Mail;

namespace Agora.Operations.Common.Extensions
{
    public static class StringExtension
    {
        public static string NormalizeAndValidateCpr(this string cpr)
        {
            var normalizedCpr = new string(cpr.Where(char.IsDigit).ToArray());

            if (normalizedCpr.Length != 10)
            {
                throw new InvalidCprException("Cpr value does not contain 10 digits");
            }

            return normalizedCpr;
        }

        public static string NormalizeAndValidateCvr(this string cvr)
        {
            var normalizedCvr = new string(cvr.Where(char.IsDigit).ToArray());

            if (normalizedCvr.Length != 8)
            {
                throw new InvalidCvrException("Cvr value does not contain 10 digits");
            }

            return normalizedCvr;
        }

        public static string NormalizeAndValidateEmail(this string email)
        {
            var trimmedEmail = email.Trim().ToLowerInvariant();

            try
            {
                var normalizedEmail = new MailAddress(trimmedEmail).Address;

                if (!string.Equals(normalizedEmail, trimmedEmail, StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new InvalidEmailException("Email is invalid");
                }

                return normalizedEmail.ToLowerInvariant();
            }
            catch (FormatException ex)
            {
                throw new InvalidEmailException("Email is not in correct format", ex);
            }
        }
    }
}