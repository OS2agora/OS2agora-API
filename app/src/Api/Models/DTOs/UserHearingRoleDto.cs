using System.Collections.Generic;
using BallerupKommune.Api.Models.Common;
using BallerupKommune.Api.Models.JsonApi;

namespace BallerupKommune.Api.Models.DTOs
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