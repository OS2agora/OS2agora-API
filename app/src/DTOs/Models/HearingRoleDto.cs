using System.Collections.Generic;
using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class HearingRoleDto : AuditableDto<HearingRoleDto>
    {
        public Enums.HearingRole Role { get; set; }

        public string Name { get; set; }

        public ICollection<UserHearingRoleDto> UserHearingRoles { get; set; } = new List<UserHearingRoleDto>();

        public ICollection<CompanyHearingRoleDto> CompanyHearingRoles { get; set; } =
            new List<CompanyHearingRoleDto>();
    }
}