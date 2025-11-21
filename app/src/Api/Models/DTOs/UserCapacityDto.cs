using System.Collections.Generic;
using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;

namespace Agora.Api.Models.DTOs
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