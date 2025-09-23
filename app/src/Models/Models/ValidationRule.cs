using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class ValidationRule : AuditableModel
    {
        public bool? CanBeEmpty { get; set; }
        public int? MaxLength { get; set; }
        public int? MinLength { get; set; }
        public int? MaxFileSize { get; set; }
        public int? MaxFileCount { get; set; }
        public string[] AllowedFileTypes { get; set; }
        public Enums.FieldType FieldType { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }
    }
}
