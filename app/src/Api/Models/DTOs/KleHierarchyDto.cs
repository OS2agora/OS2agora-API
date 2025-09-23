using System.Collections.Generic;
using BallerupKommune.Api.Models.Common;
using BallerupKommune.Api.Models.JsonApi;

namespace BallerupKommune.Api.Models.DTOs
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