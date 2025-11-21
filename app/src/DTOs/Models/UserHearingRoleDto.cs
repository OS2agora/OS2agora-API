using Agora.DTOs.Common;
using System.Collections.Generic;

namespace Agora.DTOs.Models
{
    public class UserHearingRoleDto : AuditableDto<UserHearingRoleDto>
    {
        public BaseDto<HearingRoleDto> HearingRole { get; set; }

        public BaseDto<HearingDto> Hearing { get; set; }

        public BaseDto<UserDto> User { get; set; }

        public ICollection<InvitationSourceMappingDto> InvitationSourceMappings { get; set; } =
            new List<InvitationSourceMappingDto>();
    }
}