using System.Collections.Generic;
using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class CompanyEntity : AuditableEntity
    {
        public string Cvr { get; set; }
        public string Name { get; set; }

        // One-to-many relationship with CompanyHearingRole
        public ICollection<CompanyHearingRoleEntity> CompanyHearingRoles { get; set; } = new List<CompanyHearingRoleEntity>();

        // One-to-many relationship with Users
        public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();

        // One-to-many relationship with Notification
        public ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();
    }
}
