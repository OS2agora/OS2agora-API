using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class FieldTypeSpecificationEntity : AuditableEntity
    {
        public string Name { get; set; }

        // Many-to-one relationship with FieldType
        public int FieldTypeId { get; set; }
        public FieldTypeEntity FieldType { get; set; }

        // Many-to-one relationship with ContentType
        public int ContentTypeId { get; set; }
        public ContentTypeEntity ContentType { get; set; }
    }
}
