using System.Collections.Generic;
using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;

namespace Agora.Api.Models.DTOs
{
    public class FieldTemplateDto : BaseDto<FieldTemplateDto.FieldTemplateAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>
        {
            new DtoRelationship("field"),
            new DtoRelationship("hearingType")
        };

        public class FieldTemplateAttributeDto : BaseAttributeDto
        {
            private string _name;
            private string _text;

            public string Name
            {
                get => _name;
                set { _name = value; PropertyUpdated(); }
            }

            public string Text
            {
                get => _text;
                set { _text = value; PropertyUpdated(); }
            }
        }
    }
}