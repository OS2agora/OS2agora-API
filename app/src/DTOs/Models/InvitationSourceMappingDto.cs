using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class InvitationSourceMappingDto : AuditableDto<InvitationSourceMappingDto>
    {
        public string SourceName { get; set; }

        public int InvitationSourceId { get; set; }
        public InvitationSourceDto InvitationSource { get; set; }

        public int? CompanyHearingRoleId { get; set; }
        public CompanyHearingRoleDto CompanyHearingRole { get; set; }

        public int? UserHearingRoleId {get; set; }
        public UserHearingRoleDto UserHearingRole { get; set; }
    }
}
