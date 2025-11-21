using System.Collections.Generic;
using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class User : AuditableModel
    {
        public string PersonalIdentifier { get; set; }
        public string Name { get; set; }
        public string EmployeeDisplayName { get; set; }
        public string Email { get; set; }
        public string Cpr { get; set; }
        public string Cvr { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Municipality { get; set; }
        public string StreetName { get; set; }

        public bool IsAdministrator { get; set; }
        public bool IsHearingCreator { get; set; }

        public string Identifier { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public int? UserCapacityId { get; set; }
        public UserCapacity UserCapacity { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public ICollection<UserHearingRole> UserHearingRoles { get; set; } = new List<UserHearingRole>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Event> Events { get; set; } = new List<Event>();

        public static List<string> DefaultIncludes => new List<string>
        {
            "UserCapacity"
        };
    }
}