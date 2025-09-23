using System.Collections.Generic;
using BallerupKommune.Api.Models.Common;
using BallerupKommune.Api.Models.JsonApi;

namespace BallerupKommune.Api.Models.DTOs
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