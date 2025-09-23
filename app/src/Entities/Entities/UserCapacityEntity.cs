using System.Collections.Generic;
using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class UserCapacityEntity : AuditableEntity
    {
        public Enums.UserCapacity Capacity { get; set; }

        // One-to-many relationship with Users
        public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
    }
}