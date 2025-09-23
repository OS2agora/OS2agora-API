using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.Models
{
    public class ValidationRuleDto : AuditableDto<ValidationRuleDto>
    {
        public bool? CanBeEmpty { get; set; }
        public int? MaxLength { get; set; }
        public int? MinLength { get; set; }
        public int? MaxFileSize { get; set; }
        public int? MaxFileCount { get; set; }
        public string[] AllowedFileTypes { get; set; }
        public Enums.FieldType FieldType { get; set; }
        public BaseDto<FieldDto> Field { get; set; }
    }
}