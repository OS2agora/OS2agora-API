using System.Collections.Generic;
using BallerupKommune.Api.Models.Common;
using BallerupKommune.Api.Models.JsonApi;

namespace BallerupKommune.Api.Models.DTOs
{
    public class UserCapacityDto : BaseDto<UserCapacityDto.UserCapacityAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();

        public class UserCapacityAttributeDto : BaseAttributeDto
        {
            private Enums.UserCapacity _userCapacity;

            public Enums.UserCapacity UserCapacity
            {
                get => _userCapacity;
                set { _userCapacity = value; PropertyUpdated(); }
            }
        }
    }
}