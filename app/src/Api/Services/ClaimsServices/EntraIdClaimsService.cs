using Agora.Api.Services.AuthenticationHandlers;
using Agora.Operations.ApplicationOptions;
using Microsoft.Extensions.Options;

namespace Agora.Api.Services.ClaimsServices
{
    public class EntraIdClaimsService : BaseClaimsService<EntraIdAuthenticationHandler>
    {

        public EntraIdClaimsService(IOptions<AuthenticationOptions> authOptions) : base(authOptions.Value.EntraIdClaimsOverride) { }

        public override string GetFullName()
        {
            var defaultValue = "name";
            return _claimsOverride?.FullName ?? defaultValue;
        }

        public override string GetDisplayName()
        {
            var defaultValue = "preferred_username";
            return _claimsOverride?.DisplayName ?? defaultValue;
        }

        public override string GetUniqueIdentifier()
        {
            var defaultValue = "oid";
            return _claimsOverride?.NameIdentifier ?? defaultValue;
        }

        public override string GetRoles()
        {
            var defaultValue = "roles";
            return _claimsOverride?.Roles ?? defaultValue;
        }

        public override string GetEmail()
        {
            var defaultValue = "email";
            return _claimsOverride?.Email ?? defaultValue;
        }
    }
}