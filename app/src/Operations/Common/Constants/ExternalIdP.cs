using System.Security.Claims;

namespace BallerupKommune.Operations.Common.Constants
{
    public static class ExternalIdP
    {
        /// <summary>
        /// These are all the Claims that can be in the JWT token from the external IdP
        /// Not all are important, but they are created here for visibility
        /// </summary>
        public static class Claims
        {
            // NemId Login - Citizen
            public const string NameIdentifier = ClaimTypes.NameIdentifier;
            public const string FullName = "urn:oid:2.5.4.3";
            public const string PidNumberIdentifier = "dk:gov:saml:attribute:PidNumberIdentifier";
            public const string CprNumberIdentifier = "dk:gov:saml:attribute:CprNumberIdentifier";
            public const string Name = ClaimTypes.Name;

            // NemId Login - Employee - Have all the Claims from above
            public const string FirstName = "urn:Fornavn";
            public const string LastName = "urn:Efternavn";
            public const string DisplayName = "urn:Brugernavn";
            public const string Email = "urn:Email"; // Not everyone have an e-mail registered

            // AD FS - Employee - Have all the Claims from above minus the PidNumberIdentifier and FullName
            public const string Roles = "group";


            // NemId Login - Company - Also have NameIdentifier, Name and FullName
            public const string CvrNumberIdentifier = "dk:gov:saml:attribute:CvrNumberIdentifier";
            public const string RidNumberIdentifier = "dk:gov:saml:attribute:RidNumberIdentifier";
            public const string CompanyNameIdentifier = "dk:gov:saml:attribute:NameIdentifier";


            // Standard Claims from JWT
            public const string Subject = "sub";
            public const string UniqueIdentifier = "uiId";
            public const string TokenUsage = "token_usage";
            public const string JwtId = "jti";
            public const string Scope = "scope";
            public const string Audience = "aud";
            public const string Azp = "azp";
            public const string IssuedAt = "iat";
            public const string NotBefore = "nbf";
            public const string ExpirationTime = "exp";
            public const string Issuer = "iss";
        }

        /// <summary>
        /// These are the 'roles' which can be in the Claim 'group'
        /// </summary>
        public static class Groups
        {
            public static string Admin = "agora_admin";
            public static string CanCreateHearing = "agora_ejer";
        }
    }
}