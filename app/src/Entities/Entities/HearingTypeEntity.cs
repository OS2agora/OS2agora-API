using System.Collections.Generic;
using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class HearingTypeEntity : AuditableEntity
    {
        public bool IsInternalHearing { get; set; }

        public bool IsActive { get; set; }

        public string Name { get; set; }

        // Many-to-one relationship with HearingTemplate
        public int HearingTemplateId { get; set; }
        public HearingTemplateEntity HearingTemplate { get; set; }

        // One-to-many relationship with Hearing
        public ICollection<HearingEntity> Hearings { get; set; } = new List<HearingEntity>();

        // One-to-many relationship with KleMapping
        public ICollection<KleMappingEntity> KleMappings { get; set; } = new List<KleMappingEntity>();

        // One-to-many relationship with HearingType
        public ICollection<FieldTemplateEntity> FieldTemplates { get; set; } = new List<FieldTemplateEntity>();
    }
}