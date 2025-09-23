using System.Collections.Generic;
using BallerupKommune.Api.Models.Common;
using BallerupKommune.Api.Models.JsonApi;

namespace BallerupKommune.Api.Models.DTOs
{
    public class KleMappingDto : BaseDto<KleMappingDto.KleMappingAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>
        {
            new DtoRelationship("kleHierarchy"),
            new DtoRelationship("hearingType")
        };

        public class KleMappingAttributeDto : BaseAttributeDto
        {
        }
    }
}