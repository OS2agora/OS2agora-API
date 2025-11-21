using System.Collections.Generic;
using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;

namespace Agora.Api.Models.DTOs
{
    public class CommentStatusDto : BaseDto<CommentStatusDto.CommentStatusAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>
        {
            new DtoRelationship("hearing"),
            new DtoRelationship("user")
        };

        public class CommentStatusAttributeDto : BaseAttributeDto
        {
            private Enums.CommentStatus _commentStatus;

            public Enums.CommentStatus CommentStatus
            {
                get => _commentStatus;
                set { _commentStatus = value; PropertyUpdated(); }
            }
        }
    }
}