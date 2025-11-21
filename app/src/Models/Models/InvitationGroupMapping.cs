using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class InvitationGroupMapping : AuditableModel
    {
        public int HearingTypeId { get; set; }
        public HearingType HearingType { get; set; }

        public int InvitationGroupId { get; set; }
        public InvitationGroup InvitationGroup { get; set; }
    }
}