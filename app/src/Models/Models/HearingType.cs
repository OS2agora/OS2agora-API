using System.Collections.Generic;
using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class HearingType : AuditableModel
    {
        public bool IsInternalHearing { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }

        public int HearingTemplateId { get; set; }
        public HearingTemplate HearingTemplate { get; set; }

        public ICollection<Hearing> Hearings { get; set; } = new List<Hearing>();

        public ICollection<KleMapping> KleMappings { get; set; } = new List<KleMapping>();

        public ICollection<FieldTemplate> FieldTemplates { get; set; } = new List<FieldTemplate>();

        public ICollection<InvitationGroupMapping> InvitationGroupMappings { get; set; } = new List<InvitationGroupMapping>();

        public static List<string> DefaultIncludes => new List<string>
        {
            "FieldTemplates",
            "FieldTemplates.HearingType",
            "FieldTemplates.Field",
            "HearingTemplate.Fields.FieldType",
            "HearingTemplate.Fields.HearingTemplate",
            "KleMappings.KleHierarchy",
            "KleMappings.HearingType"
        };
    }
}
