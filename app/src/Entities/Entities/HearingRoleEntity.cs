using System.Collections.Generic;
using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class HearingRoleEntity : AuditableEntity
    {
        public Enums.HearingRole Role { get; set; }

        public string Name { get; set; }

        // One-to-many relationship with UserHearingRole
        public ICollection<UserHearingRoleEntity> UserHearingRoles { get; set; } = new List<UserHearingRoleEntity>();

        public ICollection<CompanyHearingRoleEntity> CompanyHearingRoles { get; set; } =
            new List<CompanyHearingRoleEntity>();
    }
}