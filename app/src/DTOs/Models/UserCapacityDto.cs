using System.Collections.Generic;
using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class UserCapacityDto : AuditableDto<UserCapacityDto>
    {
        public Enums.UserCapacity Capacity { get; set; }

        public ICollection<UserDto> Users { get; set; } = new List<UserDto>();
    }
}