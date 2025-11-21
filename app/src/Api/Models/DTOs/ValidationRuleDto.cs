using System.Collections.Generic;
using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;

namespace Agora.Api.Models.DTOs
{
    public class ValidationRuleDto : BaseDto<ValidationRuleDto.ValidationRuleAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();

        public class ValidationRuleAttributeDto : BaseAttributeDto { }
    }
}