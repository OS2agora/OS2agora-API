using System.Collections.Generic;
using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class UserEntity : AuditableEntity
    {
        public string PersonalIdentifier { get; set; }

        public string Name { get; set; }

        public string EmployeeDisplayName { get; set; }

        public string Email { get; set; }

        public string Cpr { get; set; }

        public string Cvr { get; set; }

        public bool IsAdministrator { get; set; }

        public bool IsHearingCreator { get; set; }

        // Used to match with IdentityUser
        public string Identifier { get; set; }

        // Many-to-one relationship with Company
        public int? CompanyId { get; set; }
        public CompanyEntity Company { get; set; }

        // Many-to-one relationship with UserCapacity
        public int? UserCapacityId { get; set; }
        public UserCapacityEntity UserCapacity { get; set; }

        // One-to-many relationship with Notification
        public ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();

        // One-to-many relationship with UserHearingRole
        public ICollection<UserHearingRoleEntity> UserHearingRoles { get; set; } = new List<UserHearingRoleEntity>();

        // One-to-many relationship with Comment
        public ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();
    }
}
