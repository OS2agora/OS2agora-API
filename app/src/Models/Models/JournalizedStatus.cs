using System.Collections.Generic;
using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class JournalizedStatus : AuditableModel
    {
        public Enums.JournalizedStatus Status { get; set; }

        public ICollection<Hearing> Hearings { get; set; } = new List<Hearing>();
    }
}
