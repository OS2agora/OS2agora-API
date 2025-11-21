using System.Collections.Generic;
using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class CommentTypeDto : AuditableDto<CommentTypeDto>
    {
        public Enums.CommentType CommentType { get; set; }

        public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();

        public ICollection<CommentStatusDto> CommentStatuses { get; set; } = new List<CommentStatusDto>();
    }
}