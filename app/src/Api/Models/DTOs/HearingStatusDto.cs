using System.Collections.Generic;
using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;

namespace Agora.Api.Models.DTOs
{
    public class HearingStatusDto : BaseDto<HearingStatusDto.HearingStatusAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();

        public class HearingStatusAttributeDto : BaseAttributeDto
        {
            private Enums.HearingStatus _hearingStatus;
            private string _name;

            public Enums.HearingStatus HearingStatus
            {
                get => _hearingStatus;
                set { _hearingStatus = value; PropertyUpdated(); }
            }

            public string Name
            {
                get => _name;
                set { _name = value; PropertyUpdated(); }
            }
        }
    }
}