using System.Collections.Generic;
using Agora.Models.Common;

namespace Agora.Models.Models
{

    public class HearingTemplate : AuditableModel
    {
        public string Name { get; set; }
        public ICollection<HearingType> HearingTypes { get; set; } = new List<HearingType>();

        public ICollection<Field> Fields { get; set; } = new List<Field>();
    }
}
