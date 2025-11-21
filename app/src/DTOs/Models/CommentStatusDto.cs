using System.Collections.Generic;
using Agora.DTOs.Common;
using Agora.DTOs.Enums;

namespace Agora.DTOs.Models
{
    public class CommentStatusDto : AuditableDto<CommentStatusDto>
    {
        public CommentStatus Status { get; set; }

        public BaseDto<CommentTypeDto> CommentType { get; set; }

        public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }
}