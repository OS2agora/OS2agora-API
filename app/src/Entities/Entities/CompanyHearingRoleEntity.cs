using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class CompanyHearingRoleEntity : AuditableEntity
    {
        // Many-to-one-relationship with HearingRole
        public int HearingRoleId { get; set; }
        public HearingRoleEntity HearingRole { get; set; }

        // Many-to-one relationship with HearingEntity
        public int HearingId { get; set; }
        public HearingEntity Hearing {get; set;}

        // Many-to-one relationship with Company
        public int CompanyId { get; set; }
        public CompanyEntity Company { get; set; }
    }
}
