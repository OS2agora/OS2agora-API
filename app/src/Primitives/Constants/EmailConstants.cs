using System.Text.RegularExpressions;

namespace BallerupKommune.Primitives.Constants
{
    public static class ValidationRegex
    {
        public static string EmailRegex = @"^\S+@\S+\.\S{2,4}$";

        public static string PhoneNumberRegex = @"^\(\+45\) \d{4}-\d{4}$";
    }
}