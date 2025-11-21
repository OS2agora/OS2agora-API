using System.Collections.Generic;
using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;

namespace Agora.Api.Models.DTOs
{
    public class HearingTypeDto : BaseDto<HearingTypeDto.HearingTypeAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>
        {
            new DtoRelationship("hearingTemplate")
        };

        public class HearingTypeAttributeDto : BaseAttributeDto
        {
            private string _name;
            private bool _isActive;
            private bool _isInternalHearing;

            public string Name
            {
                get => _name;
                set { _name = value; PropertyUpdated(); }
            }

            public bool IsActive
            {
                get => _isActive;
                set { _isActive = value; PropertyUpdated(); }
            }

            public bool IsInternalHearing
            {
                get => _isInternalHearing;
                set { _isInternalHearing = value; PropertyUpdated(); }
            }
        }
    }
}