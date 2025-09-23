using System.Collections.Generic;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{

    public class HearingTemplate : AuditableModel
    {
        public string Name { get; set; }
        public ICollection<HearingType> HearingTypes { get; set; } = new List<HearingType>();

        public ICollection<Field> Fields { get; set; } = new List<Field>();
    }
}
