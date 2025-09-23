using System.Collections.Generic;
using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class HearingTemplateDto : AuditableDto<HearingTemplateDto>
    {
        public string Name { get; set; }

        public ICollection<HearingTypeDto> HearingTypes { get; set; } = new List<HearingTypeDto>();

        public ICollection<FieldDto> Fields { get; set; } = new List<FieldDto>();
    }
}