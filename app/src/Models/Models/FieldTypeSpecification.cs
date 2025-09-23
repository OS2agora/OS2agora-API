using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class FieldTypeSpecification : AuditableModel
    {
        public string Name { get; set; }

        public int FieldTypeId { get; set; }
        public FieldType FieldType { get; set; }

        public int ContentTypeId { get; set; }
        public ContentType ContentType { get; set; }
    }
}
