using System.Collections.Generic;
using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class CommentStatusEntity : AuditableEntity
    {
        public Enums.CommentStatus Status { get; set; }

        // Many-to-one relationship with CommentType
        public int CommentTypeId { get; set; }
        public CommentTypeEntity CommentType { get; set; }

        // One-to-many relationship with Comment
        public ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();
    }
}
