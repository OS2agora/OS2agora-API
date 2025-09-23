using BallerupKommune.DTOs.Common;
using System.Collections.Generic;

namespace BallerupKommune.DTOs.Models
{
    public class CompanyDto : AuditableDto<CompanyDto>
    {
        public string Cvr { get; set; }
        public string Name { get; set; }
        public ICollection<CompanyHearingRoleDto> CompanyHearingRoles { get; set; } = new List<CompanyHearingRoleDto>();
        public ICollection<UserDto> Users { get; set; } = new List<UserDto>();
    }
}
