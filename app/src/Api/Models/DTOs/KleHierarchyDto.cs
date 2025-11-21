using System.Collections.Generic;
using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;

namespace Agora.Api.Models.DTOs
{
    public class KleHierarchyDto : BaseDto<KleHierarchyDto.KleHierarchyAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();

        public class KleHierarchyAttributeDto : BaseAttributeDto
        {
            private string _name;
            private string _number;

            public string Name
            {
                get => _name;
                set { _name = value; PropertyUpdated(); }
            }

            public string Number
            {
                get => _number;
                set { _number = value; PropertyUpdated(); }
            }
        }
    }
}