using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class InvitationGroupMappingDto : AuditableDto<InvitationGroupMappingDto>
    {
        public BaseDto<HearingTypeDto> HearingType { get; set; }

        public BaseDto<InvitationGroupDto> InvitationGroup { get; set; }
    }
}