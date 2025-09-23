using BallerupKommune.Api.Models.Common;
using BallerupKommune.Api.Models.JsonApi;
using System.Collections.Generic;

namespace BallerupKommune.Api.Models.DTOs
{
    public class GlobalContentTypeDto : BaseDto<GlobalContentTypeDto.GlobalContentAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();

        public class GlobalContentAttributeDto : BaseAttributeDto
        {
        }
    }
}