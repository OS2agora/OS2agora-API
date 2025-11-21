using System.Collections.Generic;
using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class UserCapacity : AuditableModel
    {
        public Enums.UserCapacity Capacity { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}