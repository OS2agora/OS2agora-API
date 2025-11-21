using Agora.DTOs.Common;
using System.Collections.Generic;

namespace Agora.DTOs.Models
{
    public class CompanyDto : AuditableDto<CompanyDto>
    {
        public string Cvr { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string StreetName { get; set; }
        public ICollection<CompanyHearingRoleDto> CompanyHearingRoles { get; set; } = new List<CompanyHearingRoleDto>();
        public ICollection<UserDto> Users { get; set; } = new List<UserDto>();
    }
}