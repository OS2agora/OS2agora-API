using System;
using System.Collections.Generic;
using System.Linq;
using Agora.Primitives.Constants;

namespace Agora.Primitives.Logic
{
    public static class MunicipalityProfile
    {
        public static string GetMunicipalityProfile() =>
            System.Environment.GetEnvironmentVariable(MunicipalityProfileEnvironmentVariables.MunicipalityProfileEnvironmentVariable);

        public static bool IsCopenhagenMunicipalityProfile() =>
            CheckMunicipalityProfileMatch(MunicipalityProfileEnvironmentVariables.KobenhavnValue);

        public static bool IsBallerupMunicipalityProfile() =>
            CheckMunicipalityProfileMatch(MunicipalityProfileEnvironmentVariables.BallerupValue);

        public static bool IsNovatarisMunicipalityProfile() =>
            CheckMunicipalityProfileMatch(MunicipalityProfileEnvironmentVariables.NovatarisValue);

        // Use OS2 if specified explicitly and as a fallback if no other municipality is specified
        public static bool IsOS2MunicipalityProfile()
        {
            if (CheckMunicipalityProfileMatch(MunicipalityProfileEnvironmentVariables.OS2Value))
            {
                return true;
            }

            var allMunicipalityProfiles = new List<bool>()
            {
                IsCopenhagenMunicipalityProfile(),
                IsBallerupMunicipalityProfile(),
                IsNovatarisMunicipalityProfile()
            };

            // If all are false, use novataris profile
            return allMunicipalityProfiles.All(x => !x);
        }

        private static bool CheckMunicipalityProfileMatch(string municipality)
        {
            var municipalityProfile = GetMunicipalityProfile();
            return municipalityProfile != null &&
                   municipalityProfile.Equals(municipality, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}