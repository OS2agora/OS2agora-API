using BallerupKommune.Models.Common;
using System.Collections.Generic;

namespace BallerupKommune.Models.Models
{
    public class KleHierarchy : AuditableModel
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public bool IsActive { get; set; }

        public int? KleHierarchyParrentId { get; set; }
        public KleHierarchy KleHierarchyParrent { get; set; }

        public ICollection<KleHierarchy> KleHierarchyChildren { get; set; } = new List<KleHierarchy>();

        public ICollection<KleMapping> KleMappings { get; set; } = new List<KleMapping>();

        public ICollection<Hearing> Hearings { get; set; } = new List<Hearing>();
    }
}
