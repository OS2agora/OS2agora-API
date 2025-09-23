using System.Collections.Generic;
using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class HearingTypeDto : AuditableDto<HearingTypeDto>
    {
        public bool IsInternalHearing { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }

        public BaseDto<HearingTemplateDto> HearingTemplate { get; set; }

        public ICollection<HearingDto> Hearings { get; set; } = new List<HearingDto>();

        public ICollection<KleMappingDto> KleMappings { get; set; } = new List<KleMappingDto>();

        public ICollection<FieldTemplateDto> FieldTemplates { get; set; } = new List<FieldTemplateDto>();
    }
}