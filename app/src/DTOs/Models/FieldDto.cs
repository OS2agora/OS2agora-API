using System.Collections.Generic;
using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class FieldDto : AuditableDto<FieldDto>
    {
        public int DisplayOrder { get; set; }
        public string Name { get; set; }
        public bool AllowTemplates { get; set; }

        public BaseDto<ValidationRuleDto> ValidationRule { get; set; }

        public BaseDto<HearingTemplateDto> HearingTemplate { get; set; }

        public BaseDto<FieldTypeDto> FieldType { get; set; }

        public ICollection<ContentDto> Contents { get; set; } = new List<ContentDto>();

        public ICollection<FieldTemplateDto> FieldTemplates { get; set; } = new List<FieldTemplateDto>();
    }
}