using System.Collections.Generic;
using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;

namespace Agora.Api.Models.DTOs
{
    public class SubjectAreaDto : BaseDto<SubjectAreaDto.SubjectAreaAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();

        public class SubjectAreaAttributeDto : BaseAttributeDto
        {
            private string _name;
            private bool _isActive;

            public string Name
            {
                get => _name;
                set { _name = value; PropertyUpdated(); }
            }

            public bool IsActive
            {
                get => _isActive;
                set { _isActive = value; PropertyUpdated(); }
            }
        }
    }
}