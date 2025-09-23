using System.Collections.Generic;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class UserCapacity : AuditableModel
    {
        public Enums.UserCapacity Capacity { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}