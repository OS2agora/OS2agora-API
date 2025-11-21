using Agora.Models.Common;
using System.Collections.Generic;

namespace Agora.Models.Models
{
    public class InvitationGroup : AuditableModel
    {
        public string Name { get; set; }

        public ICollection<InvitationKey> InvitationKeys { get; set; } = new List<InvitationKey>();

        public ICollection<InvitationGroupMapping> InvitationGroupMappings { get; set; } = new List<InvitationGroupMapping>();

        public static List<string> DefaultIncludes => new List<string>
        {
            "InvitationKeys", "InvitationGroupMappings"
        };
    }
}