using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class UserHearingRoleDto : AuditableDto<UserHearingRoleDto>
    {
        public BaseDto<HearingRoleDto> HearingRole { get; set; }

        public BaseDto<HearingDto> Hearing { get; set; }

        public BaseDto<UserDto> User { get; set; }
    }
}