using System.Collections.Generic;
using Agora.Entities.Attributes;
using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class FieldEntity : AuditableEntity
    {
        public int DisplayOrder { get; set; }

        public string Name { get; set; }

        public bool AllowTemplates { get; set; }

        // One-to-one relationship with ValidationRule
        public int? ValidationRuleId { get; set; }
        [AllowRequestInclude]
        public ValidationRuleEntity ValidationRule { get; set; }

        // Many-to-one relationship with HearingTemplate
        public int HearingTemplateId { get; set; }
        public HearingTemplateEntity HearingTemplate { get; set; }

        // Many-to-one relationship with FieldType
        public int FieldTypeId { get; set; }
        [AllowRequestInclude(maxNavigationPathLength: 2)]
        public FieldTypeEntity FieldType { get; set; }

        // One-to-many-relationship with Content
        public ICollection<ContentEntity> Contents { get; set; } = new List<ContentEntity>();

        // One-to-many-relationship with FieldTemplate
        [AllowRequestInclude]
        public ICollection<FieldTemplateEntity> FieldTemplates { get; set; } = new List<FieldTemplateEntity>();

    }
}