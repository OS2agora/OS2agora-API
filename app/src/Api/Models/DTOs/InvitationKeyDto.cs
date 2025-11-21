using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;
using System.Collections.Generic;

namespace Agora.Api.Models.DTOs
{
    public class InvitationKeyDto : BaseDto<InvitationKeyDto.InvitationKeyAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>
        {
            new DtoRelationship("invitationGroup")
        };

        public class InvitationKeyAttributeDto : BaseAttributeDto
        {
            private string _cvr;
            private string _cpr;
            private string _email;

            public string Cvr
            {
                get => _cvr;
                set { _cvr = value; PropertyUpdated(); }
            }

            public string Cpr
            {
                get => _cpr;
                set { _cpr = value; PropertyUpdated(); }
            }

            public string Email
            {
                get => _email;
                set { _email = value; PropertyUpdated(); }
            }
        }
    }
}