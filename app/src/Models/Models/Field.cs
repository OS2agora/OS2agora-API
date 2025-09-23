using System.Collections.Generic;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class Field : AuditableModel
    {
        public int DisplayOrder { get; set; }

        public string Name { get; set; }

        public bool AllowTemplates { get; set; }

        public int? ValidationRuleId { get; set; }
        public ValidationRule ValidationRule { get; set; }

        public int HearingTemplateId { get; set; }
        public HearingTemplate HearingTemplate { get; set; }

        public int FieldTypeId { get; set; }
        public FieldType FieldType { get; set; }

        public ICollection<Content> Contents { get; set; } = new List<Content>();

        public ICollection<FieldTemplate> FieldTemplates { get; set; } = new List<FieldTemplate>();

    }
}
