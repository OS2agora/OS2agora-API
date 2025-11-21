using System.Collections.Generic;
using Agora.Entities.Attributes;
using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class FieldTypeEntity : AuditableEntity
    {
        public Enums.FieldType Type { get; set; }

        // One-to-many relationship with Field
        public ICollection<FieldEntity> Fields { get; set; } = new List<FieldEntity>();

        // One-to-many relationship with FieldTypeSpecification
        [AllowRequestInclude]
        public ICollection<FieldTypeSpecificationEntity> FieldTypeSpecifications { get; set; } = new List<FieldTypeSpecificationEntity>();
    }
}
