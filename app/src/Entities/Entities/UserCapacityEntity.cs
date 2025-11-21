using System.Collections.Generic;
using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class UserCapacityEntity : AuditableEntity
    {
        public Enums.UserCapacity Capacity { get; set; }

        // One-to-many relationship with UserCapacityMapping
        public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
    }
}