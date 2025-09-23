using System.Collections.Generic;
using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class CommentTypeDto : AuditableDto<CommentTypeDto>
    {
        public Enums.CommentType CommentType { get; set; }

        public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();

        public ICollection<CommentStatusDto> CommentStatuses { get; set; } = new List<CommentStatusDto>();
    }
}