using System.Collections.Generic;
using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;

namespace Agora.Api.Models.DTOs
{
    public class UserHearingRoleDto : BaseDto<UserHearingRoleDto.UserHearingRoleAttributeDto>
    {

        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>
        {
            new DtoRelationship("user"),
            new DtoRelationship("hearing")
        };

        public class UserHearingRoleAttributeDto : BaseAttributeDto
        {
        }
    }
}