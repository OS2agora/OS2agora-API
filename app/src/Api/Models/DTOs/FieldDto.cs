using System.Collections.Generic;
using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;

namespace Agora.Api.Models.DTOs
{
    public class FieldDto : BaseDto<FieldDto.FieldAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>
        {
            new DtoRelationship("hearingTemplate"),
            new DtoRelationship("validationRule"),
            new DtoRelationship("fieldType")
        };

        public class FieldAttributeDto : BaseAttributeDto
        {
            private int _displayOrder;
            private string _name;
            private bool _showOnList;

            public int DisplayOrder
            {
                get => _displayOrder;
                set { _displayOrder = value; PropertyUpdated(); }
            }

            public string Name
            {
                get => _name;
                set { _name = value; PropertyUpdated(); }
            }

            public bool ShowOnList
            {
                get => _showOnList;
                set { _showOnList = value; PropertyUpdated(); }
            }
        }
    }
}