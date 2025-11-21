using System.Collections.Generic;
using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class HearingStatus : AuditableModel
    {
        public Enums.HearingStatus Status { get; set; }
        public string Name { get; set; }

        public ICollection<Hearing> Hearings { get; set; } = new List<Hearing>();
    }
}
