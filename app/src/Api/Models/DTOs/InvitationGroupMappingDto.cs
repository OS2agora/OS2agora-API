using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;
using System.Collections.Generic;

namespace Agora.Api.Models.DTOs
{
    public class InvitationGroupMappingDto : BaseDto<InvitationGroupMappingDto.InvitationGroupMappingAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>
        {
            new DtoRelationship("invitationGroup"),
            new DtoRelationship("hearingType")
        };

        public class InvitationGroupMappingAttributeDto : BaseAttributeDto
        {
        }
    }
}