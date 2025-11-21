using Agora.DTOs.Common;
using System.Collections.Generic;

namespace Agora.DTOs.Models
{
    public class CommentDto : AuditableDto<CommentDto>
    {
        public int Number { get; set; }

        public bool IsDeleted { get; set; }
        public bool ContainsSensitiveInformation { get; set; }

        public string OnBehalfOf { get; set; }
        
        public BaseDto<CommentDeclineInfoDto> CommentDeclineInfo { get; set; }

        public BaseDto<UserDto> User { get; set; }

        public BaseDto<CommentStatusDto> CommentStatus { get; set; }

        public BaseDto<HearingDto> Hearing { get; set; }

        public BaseDto<CommentTypeDto> CommentType { get; set; }

        public BaseDto<CommentDto> CommentParrent { get; set; }

        public BaseDto<ConsentDto> Consent { get; set; }

        public ICollection<CommentDto> CommentChildren { get; set; } = new List<CommentDto>();

        public ICollection<ContentDto> Contents { get; set; } = new List<ContentDto>();
    }
}