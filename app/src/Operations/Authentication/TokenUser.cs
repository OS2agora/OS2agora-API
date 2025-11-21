using System.Collections.Generic;
using System.Security.Claims;
using Agora.Operations.Common.Enums;

namespace Agora.Operations.Authentication
{
    public class TokenUser
    {
        public AuthenticationMethod AuthMethod { get; set; } = AuthenticationMethod.NONE;
        public string ApplicationUserId { get; set; }
        public string Cvr { get; set; } = null;
        public string Cpr { get; set; } = null;
        public string Pid { get; set; } = null;
        public string Email { get; set; } = null;
        public string PersonalIdentifier { get; set; } = null;
        public string Name { get; set; } = null;
        public string EmployeeDisplayName { get; set; } = null;
        public string CompanyName { get; set; } = null;
        public List<Claim> PossibleRoles { get; set; } = new List<Claim>();
        public List<Claim> AdditionalClaims { get; set; } = new List<Claim>();

    }
}