using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class FieldTemplateDto : AuditableDto<FieldTemplateDto>
    {
        public string Name { get; set; }
        public string Text { get; set; }

        public BaseDto<FieldDto> Field { get; set; }
        public BaseDto<HearingTypeDto> HearingType { get; set; }
    }
}