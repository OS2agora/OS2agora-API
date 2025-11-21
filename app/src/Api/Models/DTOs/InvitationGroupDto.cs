using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;
using System.Collections.Generic;

namespace Agora.Api.Models.DTOs
{
    public class InvitationGroupDto : BaseDto<InvitationGroupDto.InvitationGroupAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();

        public class InvitationGroupAttributeDto : BaseAttributeDto
        {
            private string _name;

            public string Name
            {
                get => _name;
                set { _name = value; PropertyUpdated(); }
            }
        }
    }
}