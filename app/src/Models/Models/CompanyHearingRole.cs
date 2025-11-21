using System.Collections.Generic;
using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class CompanyHearingRole : AuditableModel
    {
        public int HearingRoleId { get; set; }
        public HearingRole HearingRole { get; set; }

        public int HearingId { get; set; }
        public Hearing Hearing { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<InvitationSourceMapping> InvitationSourceMappings { get; set; } =
            new List<InvitationSourceMapping>();

        public static List<string> DefaultIncludes => new List<string>
        {
            "Hearing",
            "HearingRole",
            "Company"
        };
    }
}