using System.Collections.Generic;
using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class UserDto : AuditableDto<UserDto>
    {
        public string PersonalIdentifier { get; set; }
        public string Name { get; set; }
        public string EmployeeDisplayName { get; set; }
        public string Email { get; set; }
        public string Cpr { get; set; }
        public string Cvr { get; set; }

        public bool IsAdministrator { get; set; }
        public bool IsHearingCreator { get; set; }

        // TODO: Should this be here?
        public string Identifier { get; set; }

        public BaseDto<CompanyDto> Company { get; set; }

        public BaseDto<UserCapacityDto> UserCapacity { get; set; }

        public ICollection<UserHearingRoleDto> UserHearingRoles { get; set; } = new List<UserHearingRoleDto>();

        public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }
}