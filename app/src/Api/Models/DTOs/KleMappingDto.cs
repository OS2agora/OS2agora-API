using System.Collections.Generic;
using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;

namespace Agora.Api.Models.DTOs
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