using Agora.DTOs.Common;
using System.Collections.Generic;

namespace Agora.DTOs.Models
{
    public class CompanyHearingRoleDto : AuditableDto<CompanyHearingRoleDto>
    {
        public BaseDto<HearingRoleDto> HearingRole { get; set; }

        public BaseDto<HearingDto> Hearing { get; set; }

        public BaseDto<CompanyDto> Company { get; set; }

        public ICollection<InvitationSourceMappingDto> InvitationSourceMappings { get; set; } =
            new List<InvitationSourceMappingDto>();
    }
}