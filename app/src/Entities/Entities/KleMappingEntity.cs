using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class KleMappingEntity : AuditableEntity
    {
        // Many-to-one relationship with KleHierarchy
        public int KleHierarchyId { get; set; }
        public KleHierarchyEntity KleHierarchy { get; set; }

        // Many-to-one relationship with HearingType
        public int HearingTypeId { get; set; }
        public HearingTypeEntity HearingType { get; set; }
    }
}
