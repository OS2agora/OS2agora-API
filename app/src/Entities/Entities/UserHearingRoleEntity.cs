using Agora.Entities.Attributes;
using Agora.Entities.Common;
using System.Collections.Generic;

namespace Agora.Entities.Entities
{
    public class UserHearingRoleEntity : AuditableEntity
    {
        // Many-to-one-relationship with HearingRole
        public int HearingRoleId { get; set; }
        public HearingRoleEntity HearingRole { get; set; }

        // Many-to-one relationship with HearingEntity
        public int HearingId { get; set; }
        public HearingEntity Hearing {get; set;}

        // Many-to-one relationship with User
        public int UserId { get; set; }
        public UserEntity User { get; set; }

        // One-to-many-relationship with InvitationSourceMapping
        [AllowRequestInclude(maxNavigationPathLength: 1)]
        public ICollection<InvitationSourceMappingEntity> InvitationSourceMappings { get; set; } =
            new List<InvitationSourceMappingEntity>();
    }
}
