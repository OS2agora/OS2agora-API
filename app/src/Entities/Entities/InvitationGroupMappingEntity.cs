using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class InvitationGroupMappingEntity : AuditableEntity
    {
        // Many-to-one relationship with HearingType
        public int HearingTypeId { get; set; }
        public HearingTypeEntity HearingType { get; set; }

        // Many-to-one relationship with InvitationGroup
        public int InvitationGroupId { get; set; }
        public InvitationGroupEntity InvitationGroup { get; set; }
    }
}