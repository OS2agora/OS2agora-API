using System.Collections.Generic;
using BallerupKommune.Api.Models.Common;
using BallerupKommune.Api.Models.JsonApi;

namespace BallerupKommune.Api.Models.DTOs
{
    public class HearingTemplateDto : BaseDto<HearingTemplateDto.HearingTemplateAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();

        public class HearingTemplateAttributeDto : BaseAttributeDto
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