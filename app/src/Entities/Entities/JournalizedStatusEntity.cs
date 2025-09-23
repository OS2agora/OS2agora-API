using System.Collections.Generic;
using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class JournalizedStatusEntity : AuditableEntity
    {
        public Enums.JournalizedStatus Status { get; set; }

        // One-to-many relationship with Content
        public ICollection<HearingEntity> Hearings { get; set; } = new List<HearingEntity>();
    }
}
