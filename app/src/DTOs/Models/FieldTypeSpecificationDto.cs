using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class FieldTypeSpecificationDto : AuditableDto<FieldTypeSpecificationDto>
    {
        public string Name { get; set; }

        public BaseDto<FieldTypeDto> FieldType { get; set; }

        public BaseDto<ContentTypeDto> ContentType { get; set; }
    }
}