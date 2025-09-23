using System.Collections.Generic;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class JournalizedStatus : AuditableModel
    {
        public Enums.JournalizedStatus Status { get; set; }

        public ICollection<Hearing> Hearings { get; set; } = new List<Hearing>();
    }
}
