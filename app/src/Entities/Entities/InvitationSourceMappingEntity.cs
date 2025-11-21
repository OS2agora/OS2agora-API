using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class InvitationSourceMappingEntity : AuditableEntity
    {
        public string SourceName { get; set; }

        // Many-to-one relationship with InvitationSource
        public int InvitationSourceId { get; set; }
        public InvitationSourceEntity InvitationSource { get; set; }

        // Many-to-one relationship with CompanyHearingRole
        public int? CompanyHearingRoleId { get; set; }
        public CompanyHearingRoleEntity CompanyHearingRole { get; set; }

        // Many-to-one relationship with UserHearingRole
        public int? UserHearingRoleId { get; set; }
        public UserHearingRoleEntity UserHearingRole { get; set; }
    }
}
