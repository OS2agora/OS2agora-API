using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
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
    }
}
