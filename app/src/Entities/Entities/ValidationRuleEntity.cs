using BallerupKommune.Entities.Common;

namespace BallerupKommune.Entities.Entities
{
    public class ValidationRuleEntity : AuditableEntity
    {
        public Enums.FieldType FieldType { get; set; }
        public bool? CanBeEmpty { get; set; }

        public int? MaxLength { get; set; }

        public int? MinLength { get; set; }

        public int? MaxFileSize { get; set; }

        public int? MaxFileCount { get; set; }

        public string[] AllowedFileTypes { get; set; }

        // One-to-one relationship with Field
        public FieldEntity Field { get; set; }
    }
}
