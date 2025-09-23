using System.Collections.Generic;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class HearingStatus : AuditableModel
    {
        public Enums.HearingStatus Status { get; set; }
        public string Name { get; set; }

        public ICollection<Hearing> Hearings { get; set; } = new List<Hearing>();
    }
}
