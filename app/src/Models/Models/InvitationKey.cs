using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class InvitationKey : AuditableModel
    {
        public string Cpr { get; set; }
        public string Cvr { get; set; }
        public string Email { get; set; }

        public int InvitationGroupId { get; set; }
        public InvitationGroup InvitationGroup { get; set; }
    }
}