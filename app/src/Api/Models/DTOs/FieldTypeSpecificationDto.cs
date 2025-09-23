using System.Collections.Generic;
using BallerupKommune.Api.Models.Common;
using BallerupKommune.Api.Models.JsonApi;

namespace BallerupKommune.Api.Models.DTOs
{
    public class FieldTypeSpecificationDto : BaseDto<FieldTypeSpecificationDto.FieldTypeSpecificationAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>
        {
            new DtoRelationship("fieldType"),
            new DtoRelationship("contentType")
        };

        public class FieldTypeSpecificationAttributeDto : BaseAttributeDto
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