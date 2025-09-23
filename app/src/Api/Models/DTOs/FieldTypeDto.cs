using System.Collections.Generic;
using BallerupKommune.Api.Models.Common;
using BallerupKommune.Api.Models.JsonApi;

namespace BallerupKommune.Api.Models.DTOs
{
    public class FieldTypeDto : BaseDto<FieldTypeDto.FieldTypeAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();

        public class FieldTypeAttributeDto : BaseAttributeDto
        {
            private Enums.FieldType _fieldType;

            public Enums.FieldType FieldType
            {
                get => _fieldType;
                set { _fieldType = value; PropertyUpdated(); }
            }
        }
    }
}