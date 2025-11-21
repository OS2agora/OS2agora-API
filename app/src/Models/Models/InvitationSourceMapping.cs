using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class InvitationSourceMapping : AuditableModel
    {
        public string SourceName { get; set; }

        public int InvitationSourceId { get; set; }
        public InvitationSource InvitationSource { get; set; }

        public int? CompanyHearingRoleId { get; set; }
        public CompanyHearingRole CompanyHearingRole { get; set; }

        public int? UserHearingRoleId { get; set; }
        public UserHearingRole UserHearingRole { get; set; }
    }
}