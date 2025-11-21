using Agora.Api.Services.Interfaces;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Enums;
using Microsoft.Extensions.Options;
using System;

namespace Agora.Api.Services
{
    public class BaseClaimsService<T> : IClaimsService<T>
    {
        protected readonly ClaimsOverride _claimsOverride;

        protected BaseClaimsService(ClaimsOverride claimsOverride)
        {
            _claimsOverride = claimsOverride;
        }

        public virtual string GetNameIdentifier()
        {
            throw new NotImplementedException();
        }

        public virtual string GetFullName()
        {
            throw new NotImplementedException();
        }

        public virtual string GetName()
        {
            throw new NotImplementedException();
        }

        public virtual string GetFirstName()
        {
            throw new NotImplementedException();
        }

        public virtual string GetLastName()
        {
            throw new NotImplementedException();
        }

        public virtual string GetDisplayName()
        {
            throw new NotImplementedException();
        }

        public virtual string GetEmail()
        {
            throw new NotImplementedException();
        }

        public virtual string GetOrganizationName()
        {
            throw new NotImplementedException();
        }

        public virtual string GetPidNumberIdentifier()
        {
            throw new NotImplementedException();
        }

        public virtual string GetCprNumberIdentifier()
        {
            throw new NotImplementedException();
        }

        public virtual string GetCvrNumberIdentifier()
        {
            throw new NotImplementedException();
        }

        public virtual string GetRidNumberIdentifier()
        {
            throw new NotImplementedException();
        }

        public virtual string GetCompanyNameIdentifier()
        {
            throw new NotImplementedException();
        }

        public virtual string GetRoles()
        {
            throw new NotImplementedException();
        }

        public virtual string GetSubject()
        {
            throw new NotImplementedException();
        }

        public virtual string GetUniqueIdentifier()
        {
            throw new NotImplementedException();
        }

        public virtual string GetTokenUsage()
        {
            throw new NotImplementedException();
        }

        public virtual string GetJwtId()
        {
            throw new NotImplementedException();
        }

        public virtual string GetScope()
        {
            throw new NotImplementedException();
        }

        public virtual string GetAudience()
        {
            throw new NotImplementedException();
        }

        public virtual string GetAzp()
        {
            throw new NotImplementedException();
        }

        public virtual string GetIssuedAt()
        {
            throw new NotImplementedException();
        }

        public virtual string GetNotBefore()
        {
            throw new NotImplementedException();
        }

        public virtual string GetExpirationTime()
        {
            throw new NotImplementedException();
        }

        public virtual string GetIssuer()
        {
            throw new NotImplementedException();
        }
    }
}