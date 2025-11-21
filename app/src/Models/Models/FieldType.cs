using System.Collections.Generic;
using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class FieldType : AuditableModel
    {
        public Enums.FieldType Type { get; set; }

        public ICollection<Field> Fields { get; set; } = new List<Field>();

        public ICollection<FieldTypeSpecification> FieldTypeSpecifications { get; set; } = new List<FieldTypeSpecification>();
    }
}
