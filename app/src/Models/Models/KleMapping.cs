using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class KleMapping : AuditableModel
    {
        public int KleHierarchyId { get; set; }
        public KleHierarchy KleHierarchy { get; set; }

        public int HearingTypeId { get; set; }
        public HearingType HearingType { get; set; }
    }
}
