using System.Collections.Generic;
using BallerupKommune.Api.Models.Common;
using BallerupKommune.Api.Models.JsonApi;

namespace BallerupKommune.Api.Models.DTOs
{
    public class HearingRoleDto : BaseDto<HearingRoleDto.HearingRoleAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();

        public class HearingRoleAttributeDto : BaseAttributeDto
        {
            private Enums.HearingRole _hearingRole;
            private string _name;

            public Enums.HearingRole HearingRole
            {
                get => _hearingRole;
                set { _hearingRole = value; PropertyUpdated(); }
            }

            public string Name
            {
                get => _name;
                set { _name = value; PropertyUpdated(); }
            }
        }
    }
}