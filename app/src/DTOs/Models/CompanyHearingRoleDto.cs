using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class CompanyHearingRoleDto : AuditableDto<CompanyHearingRoleDto>
    {
        public BaseDto<HearingRoleDto> HearingRole { get; set; }

        public BaseDto<HearingDto> Hearing { get; set; }

        public BaseDto<CompanyDto> Company { get; set; }
    }
}