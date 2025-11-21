using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class InvitationKeyEntity : AuditableEntity
    {
        public string Cpr { get; set; }
        public string Cvr { get; set; }
        public string Email { get; set; }

        // Many-to-one relationship with InvitationGroup
        public int InvitationGroupId { get; set; }
        public InvitationGroupEntity InvitationGroup { get; set; }
    }
}