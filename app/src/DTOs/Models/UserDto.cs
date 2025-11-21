using System.Collections.Generic;
using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class UserDto : AuditableDto<UserDto>
    {
        public string Name { get; set; }
        public string EmployeeDisplayName { get; set; }
        public string Email { get; set; }
        public string Cpr { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string StreetName { get; set; }

        public bool IsAdministrator { get; set; }
        public bool IsHearingCreator { get; set; }

        public string Identifier { get; set; }
        public BaseDto<CompanyDto> Company { get; set; }

        public BaseDto<UserCapacityDto> UserCapacity { get; set; }

        public ICollection<UserHearingRoleDto> UserHearingRoles { get; set; } = new List<UserHearingRoleDto>();

        public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }
}