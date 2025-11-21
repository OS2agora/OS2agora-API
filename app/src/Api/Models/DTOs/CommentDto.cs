using System.Collections.Generic;
using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;

namespace Agora.Api.Models.DTOs
{
    public class CommentDto : BaseDto<CommentDto.CommentAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>
        {
            new DtoRelationship("hearing"),
            new DtoRelationship("user"),
            new DtoRelationship("commentStatus"),
            new DtoRelationship("commentType")
        };

        public class CommentAttributeDto : BaseAttributeDto
        {
            private int _number;
            
            private bool _isDeleted;
            private bool _containsSensitiveInformation;

            private string _onBehalfOf;

            public int Number
            {
                get => _number;
                set { _number = value; PropertyUpdated(); }
            }

            public bool IsDeleted
            {
                get => _isDeleted;
                set { _isDeleted = value; PropertyUpdated(); }
            }

            public bool ContainsSensitiveInformation
            {
                get => _containsSensitiveInformation;
                set { _containsSensitiveInformation = value; PropertyUpdated(); }
            }

            public string OnBehalfOf
            {
                get => _onBehalfOf;
                set { _onBehalfOf = value; PropertyUpdated(); }
            }
        }
    }
}