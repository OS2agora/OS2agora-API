using System.Collections.Generic;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class User : AuditableModel
    {
        public string PersonalIdentifier { get; set; }
        public string Name { get; set; }
        public string EmployeeDisplayName { get; set; }
        public string Email { get; set; }
        public string Cpr { get; set; }
        public string Cvr { get; set; }

        public bool IsAdministrator { get; set; }
        public bool IsHearingCreator { get; set; }

        public string Identifier { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public int? UserCapacityId { get; set; }
        public UserCapacity UserCapacity { get; set; }

        public ICollection<Notification> Notifications { get; set; } 

        public ICollection<UserHearingRole> UserHearingRoles { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public static List<string> DefaultIncludes => new List<string>
        {
            "UserCapacity"
        };
    }
}