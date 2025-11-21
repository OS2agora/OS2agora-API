using System.Collections.Generic;
using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class CommentTypeEntity : AuditableEntity
    {
        public Enums.CommentType Type { get; set; }

        // One-to-many relationship with Comment
        public ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();

        // One-to-many relationship with CommentStatus
        public ICollection<CommentStatusEntity> CommentStatuses { get; set; } = new List<CommentStatusEntity>();
    }
}
