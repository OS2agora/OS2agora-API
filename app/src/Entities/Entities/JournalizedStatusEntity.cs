using System.Collections.Generic;
using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class JournalizedStatusEntity : AuditableEntity
    {
        public Enums.JournalizedStatus Status { get; set; }

        // One-to-many relationship with Content
        public ICollection<HearingEntity> Hearings { get; set; } = new List<HearingEntity>();
    }
}
