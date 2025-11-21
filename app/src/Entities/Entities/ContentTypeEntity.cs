using System.Collections.Generic;
using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class ContentTypeEntity : AuditableEntity
    {
        public Enums.ContentType Type { get; set; }

        // One-to-many relationship with Content
        public ICollection<ContentEntity> Contents { get; set; } = new List<ContentEntity>();

        // One-to-many relationship with Content
        public ICollection<FieldTypeSpecificationEntity> FieldTypeSpecifications { get; set; } = new List<FieldTypeSpecificationEntity>();
    }
}