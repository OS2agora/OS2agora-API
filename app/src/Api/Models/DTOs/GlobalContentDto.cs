using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;
using System.Collections.Generic;

namespace Agora.Api.Models.DTOs
{
    public class GlobalContentDto : BaseDto<GlobalContentDto.GlobalContentAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();
        public class GlobalContentAttributeDto : BaseAttributeDto
        {
            private string _content;

            public string Content
            {
                get => _content;
                set { _content = value; PropertyUpdated(); }
            }
        }
    }
}