using System.Collections.Generic;
using BallerupKommune.DTOs.Common;
using BallerupKommune.DTOs.Enums;

namespace BallerupKommune.DTOs.Models
{
    public class CommentStatusDto : AuditableDto<CommentStatusDto>
    {
        public CommentStatus Status { get; set; }

        public BaseDto<CommentTypeDto> CommentType { get; set; }

        public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }
}