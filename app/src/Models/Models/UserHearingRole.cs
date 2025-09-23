using System.Collections.Generic;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class UserHearingRole : AuditableModel
    {
        public int HearingRoleId { get; set; } 
        public HearingRole HearingRole { get; set; }

        public int HearingId { get; set; }
        public Hearing Hearing {get; set;}

        public int UserId { get; set; }
        public User User { get; set; }

        public static List<string> DefaultIncludes => new List<string>
        {
            "Hearing",
            "HearingRole",
            "User"
        };
    }
}
