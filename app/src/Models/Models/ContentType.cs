using System.Collections.Generic;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class ContentType : AuditableModel
    {
        public Enums.ContentType Type { get; set; }

        public ICollection<Content> Contents { get; set; } = new List<Content>();
        
        public ICollection<FieldTypeSpecification> FieldTypeSpecifications { get; set; } = new List<FieldTypeSpecification>();
    }
}
