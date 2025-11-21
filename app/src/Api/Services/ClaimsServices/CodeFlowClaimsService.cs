using Agora.Api.Services.AuthenticationHandlers;
using Agora.Operations.ApplicationOptions;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Agora.Api.Services.ClaimsServices
{
    public class CodeFlowClaimsService : BaseClaimsService<CodeFlowAuthenticationHandler>
    {
        public CodeFlowClaimsService(IOptions<AuthenticationOptions> authOptions) : base(
            authOptions.Value.CodeFlowClaimsOverride){ }

        public override string GetNameIdentifier()
        {
            var defaultValue = ClaimTypes.NameIdentifier;
            return _claimsOverride?.NameIdentifier ?? defaultValue;
        }

        public override string GetFullName()
        {
            var defaultValue = "urn:oid:2.5.4.3";
            return _claimsOverride?.FullName ?? defaultValue;
        }

        public override string GetName()
        {
            var defaultValue = ClaimTypes.Name;
            return _claimsOverride?.Name ?? defaultValue;
        }

        public override string GetFirstName()
        {
            var defaultValue = "urn:Fornavn";
            return _claimsOverride?.FirstName ?? defaultValue;
        }

        public override string GetLastName()
        {
            var defaultValue = "urn:Efternavn";
            return _claimsOverride?.LastName ?? defaultValue;
        }

        public override string GetDisplayName()
        {
            var defaultValue = "urn:Brugernavn";
            return _claimsOverride?.DisplayName ?? defaultValue;
        }

        public override string GetEmail()
        {
            var defaultValue = "urn:Email";
            return _claimsOverride?.Email ?? defaultValue;
        }

        public override string GetPidNumberIdentifier()
        {
            var defaultValue = "dk:gov:saml:attribute:PidNumberIdentifier";
            return _claimsOverride?.PidNumberIdentifier ?? defaultValue;
        }

        public override string GetCprNumberIdentifier()
        {
            var defaultValue = "dk:gov:saml:attribute:CprNumberIdentifier";
            return _claimsOverride?.CprNumberIdentifier ?? defaultValue;
        }

        public override string GetCvrNumberIdentifier()
        {
            var defaultValue = "dk:gov:saml:attribute:CvrNumberIdentifier";
            return _claimsOverride?.CvrNumberIdentifier ?? defaultValue;
        }

        public override string GetRidNumberIdentifier()
        {
            var defaultValue = "dk:gov:saml:attribute:RidNumberIdentifier";
            return _claimsOverride?.RidNumberIdentifier ?? defaultValue;
        }

        public override string GetCompanyNameIdentifier()
        {
            var defaultValue = "dk:gov:saml:attribute:NameIdentifier";
            return _claimsOverride?.OrganizationName ?? defaultValue;
        }

        public override string GetRoles()
        {
            var defaultValue = "group";
            return _claimsOverride?.Roles ?? defaultValue;
        }

        public override string GetSubject()
        {
            var defaultValue = "sub";
            return _claimsOverride?.Subject ?? defaultValue;
        }

        public override string GetUniqueIdentifier()
        {
            var defaultValue = "uiId";
            return _claimsOverride?.UniqueIdentifier ?? defaultValue;
        }

        public override string GetTokenUsage()
        {
            var defaultValue = "token_usage";
            return _claimsOverride?.TokenUsage ?? defaultValue;
        }

        public override string GetJwtId()
        {
            var defaultValue = "jti";
            return _claimsOverride?.JwtId ?? defaultValue;
        }

        public override string GetScope()
        {
            var defaultValue = "scope";
            return _claimsOverride?.Scope ?? defaultValue;
        }

        public override string GetAudience()
        {
            var defaultValue = "aud";
            return _claimsOverride?.Audience ?? defaultValue;
        }

        public override string GetAzp()
        {
            var defaultValue = "azp";
            return _claimsOverride?.Azp ?? defaultValue;
        }

        public override string GetIssuedAt()
        {
            var defaultValue = "iat";
            return _claimsOverride?.IssuedAt ?? defaultValue;
        }

        public override string GetNotBefore()
        {
            var defaultValue = "nbf";
            return _claimsOverride?.NotBefore ?? defaultValue;
        }

        public override string GetExpirationTime()
        {
            var defaultValue = "exp";
            return _claimsOverride?.ExpirationTime ?? defaultValue;
        }

        public override string GetIssuer()
        {
            var defaultValue = "iss";
            return _claimsOverride?.Issuer ?? defaultValue;
        }
    }
}