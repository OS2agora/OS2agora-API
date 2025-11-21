using System.Collections.Generic;
using Agora.Models.Enums;

namespace Agora.Operations.ApplicationOptions
{
    public class AuthenticationOptions
    {
        public const string Authentication = "Authentication";

        public string InternalApiKey { get; set; }
        public string PublicApiKey { get; set; }

        public List<UserCapacity> InternalAuthentication { get; set; }
        public List<UserCapacity> PublicAuthentication { get; set; }

        public string CitizenAuthentication { get; set; }
        public string CompanyAuthentication { get; set; }
        public string EmployeeAuthentication { get; set; }

        public string UnknownAuthentication { get; set; }

        public ClaimsOverride CodeFlowClaimsOverride { get; set; }
        public ClaimsOverride EntraIdClaimsOverride { get; set; }
        public ClaimsOverride NemLoginClaimsOverride { get; set; }

        public Roles Roles { get; set; }

        /// <summary>
        /// Possible values for Internal- Public- and Unknown- Authentication options
        /// </summary>
        public static class AuthenticationMethods
        {
            public const string CodeFlow = "CodeFlow";
            public const string NemLogin = "NemLogin";
            public const string EntraId = "EntraId";
        }
    }

    public class Roles
    {
        public string Admin { get; set; }
        public string HearingCreator { get; set; }
    }

    public class ClaimsOverride
    {
        public string NameIdentifier { get; set; } = null;
        public string FullName {get; set;} = null;
        public string Name {get; set;} = null;
        public string FirstName {get; set;} = null;
        public string LastName {get; set;} = null;
        public string DisplayName {get; set;} = null;
        public string Email {get; set;} = null;
        public string OrganizationName { get; set; } = null;

        public string PidNumberIdentifier {get; set;} = null;
        public string CprNumberIdentifier {get; set;} = null;
        public string CvrNumberIdentifier {get; set;} = null;
        public string RidNumberIdentifier {get; set;} = null;
        public string CompanyNameIdentifier {get; set;} = null;

        public string Roles {get; set;} = null;

        public string Subject {get; set;} = null;
        public string UniqueIdentifier {get; set;} = null;
        public string TokenUsage {get; set;} = null;
        public string JwtId {get; set;} = null;
        public string Scope {get; set;} = null;
        public string Audience {get; set;} = null;
        public string Azp {get; set;} = null;
        public string IssuedAt {get; set;} = null;
        public string NotBefore {get; set;} = null;
        public string ExpirationTime {get; set;} = null;
        public string Issuer {get; set;} = null;
    }
}