using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class InvitationKeyDto : AuditableDto<InvitationKeyDto>
    {
        public string Cpr { get; set; }
        public string Cvr { get; set; }
        public string Email { get; set; }

        public BaseDto<InvitationGroupDto> InvitationGroup { get; set; }
    }
}