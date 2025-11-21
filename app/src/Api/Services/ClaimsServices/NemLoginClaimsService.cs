using Agora.Api.Services.AuthenticationHandlers;
using Agora.Operations.ApplicationOptions;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Agora.Api.Services.ClaimsServices
{
    public class NemLoginClaimsService : BaseClaimsService<NemLoginAuthenticationHandler>
    {
        public NemLoginClaimsService(IOptions<AuthenticationOptions> authOptions) : base(
            authOptions.Value.NemLoginClaimsOverride)
        { }

        public override string GetNameIdentifier()
        {
            var defaultValue = ClaimTypes.NameIdentifier;
            return _claimsOverride?.Name ?? defaultValue;
        }

        public override string GetFullName()
        {
            var defaultValue = "https://data.gov.dk/model/core/eid/fullName";
            return _claimsOverride?.FullName ?? defaultValue;
        }

        public override string GetOrganizationName()
        {
            var defaultValue = "https://data.gov.dk/model/core/eid/professional/orgName";
            return _claimsOverride?.OrganizationName ?? defaultValue;
        }

        public override string GetPidNumberIdentifier()
        {
            var defaultValue = "https://data.gov.dk/model/core/eid/person/pid";
            return _claimsOverride?.PidNumberIdentifier ?? defaultValue;
        }

        public override string GetCprNumberIdentifier()
        {
            var defaultValue = "https://data.gov.dk/model/core/eid/cprNumber";
            return _claimsOverride?.CprNumberIdentifier ?? defaultValue;
        }

        public override string GetCvrNumberIdentifier()
        {
            var defaultValue = "https://data.gov.dk/model/core/eid/professional/cvr";
            return _claimsOverride?.CvrNumberIdentifier ?? defaultValue;
        }

        public override string GetRidNumberIdentifier()
        {
            var defaultValue = "https://data.gov.dk/model/core/eid/professional/rid";
            return _claimsOverride?.RidNumberIdentifier ?? defaultValue;
        }

    }
}