using System.Collections.Generic;
using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;

namespace Agora.Api.Models.DTOs
{
    public class CommentTypeDto : BaseDto<CommentTypeDto.CommentTypeAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>();

        public class CommentTypeAttributeDto : BaseAttributeDto
        {
            private Enums.CommentType _commentType;

            public Enums.CommentType CommentType
            {
                get => _commentType;
                set { _commentType = value; PropertyUpdated(); }
            }
        }
    }
}