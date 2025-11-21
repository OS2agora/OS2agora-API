using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class FieldTemplateEntity : AuditableEntity
    {
        public string Name { get; set; }

        public string Text { get; set; }

        // Many-to-one relationship with Field
        public int FieldId { get; set; }
        public FieldEntity Field { get; set; }

        // Many-to-one relationship with HearingType
        public int HearingTypeId { get; set; }
        public HearingTypeEntity HearingType { get; set; }
    }
}