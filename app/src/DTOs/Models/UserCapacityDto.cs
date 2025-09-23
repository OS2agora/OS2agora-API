using System.Collections.Generic;
using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class UserCapacityDto : AuditableDto<UserCapacityDto>
    {
        public Enums.UserCapacity Capacity { get; set; }

        public ICollection<UserDto> Users { get; set; } = new List<UserDto>();
    }
}