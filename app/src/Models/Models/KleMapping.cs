using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class KleMapping : AuditableModel
    {
        public int KleHierarchyId { get; set; }
        public KleHierarchy KleHierarchy { get; set; }

        public int HearingTypeId { get; set; }
        public HearingType HearingType { get; set; }
    }
}
