using System.Collections.Generic;
using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class KleHierarchyEntity : AuditableEntity
    {
        public string Name { get; set; }

        public string Number { get; set; }

        public bool IsActive { get; set; }

        // Many-to-one relationship with KleHierarchy (parrent KleNumbers)
        public int? KleHierarchyParrentId { get; set; }
        public KleHierarchyEntity KleHierarchyParrent { get; set; }

        // One-to-many relationship with KleHierarchy (child KleNumbers)
        public ICollection<KleHierarchyEntity> KleHierarchyChildren { get; set; } = new List<KleHierarchyEntity>();

        // One-to-many relationship with KleMapping
        public ICollection<KleMappingEntity> KleMappings { get; set; } = new List<KleMappingEntity>();

        // One-to-many relationship with Hearing
        public ICollection<HearingEntity> Hearings { get; set; } = new List<HearingEntity>();
    }
}