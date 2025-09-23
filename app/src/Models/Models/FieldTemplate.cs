using System.Collections.Generic;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class FieldTemplate : AuditableModel
    {
        public string Name { get; set; }
        public string Text { get; set; }

        public int FieldId { get; set; }
        public Field Field { get; set; }
        
        public int HearingTypeId { get; set; }
        public HearingType HearingType { get; set; }

        public static List<string> DefaultIncludes => new List<string>
        {
            "Field.FieldType", "HearingType"
        };
    }
}