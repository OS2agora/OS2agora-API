using System.Collections.Generic;
using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class ContentType : AuditableModel
    {
        public Enums.ContentType Type { get; set; }

        public ICollection<Content> Contents { get; set; } = new List<Content>();
        
        public ICollection<FieldTypeSpecification> FieldTypeSpecifications { get; set; } = new List<FieldTypeSpecification>();
    }
}
