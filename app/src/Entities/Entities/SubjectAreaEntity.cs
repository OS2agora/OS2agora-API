using System.Collections.Generic;
using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class SubjectAreaEntity : AuditableEntity
    {
        public bool IsActive { get; set; }

        public string Name { get; set; }

        // Many-to-one relationship with Hearing
        public ICollection<HearingEntity> Hearings { get; set; } = new List<HearingEntity>();
    }
}
