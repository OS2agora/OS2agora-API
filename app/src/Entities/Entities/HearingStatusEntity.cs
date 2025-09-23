using System.Collections.Generic;
using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class HearingStatusEntity : AuditableEntity
    {
        public Enums.HearingStatus Status { get; set; }

        public string Name { get; set; }

        // One-to-many relationship with Hearing
        public ICollection<HearingEntity> Hearings { get; set; } = new List<HearingEntity>();
    }
}