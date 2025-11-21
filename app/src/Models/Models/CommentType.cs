using System.Collections.Generic;
using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class CommentType : AuditableModel
    {
        public Enums.CommentType Type { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<CommentStatus> CommentStatuses { get; set; } = new List<CommentStatus>();
    }
}
