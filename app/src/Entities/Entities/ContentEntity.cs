using Agora.Entities.Attributes;
using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class ContentEntity : AuditableEntity
    {
        public string TextContent { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FileContentType { get; set; }

        // Many-to-one relationship with Comment
        public int? CommentId { get; set; }
        public CommentEntity Comment { get; set; }

        // Many-to-one relationship with Hearing
        public int HearingId { get; set; }
        public HearingEntity Hearing { get; set; }

        // Many-to-one relationship with Field
        public int? FieldId { get; set; }
        public FieldEntity Field { get; set; }

        // Many-to-one relationship with ContentType
        public int ContentTypeId { get; set; }
        [AllowRequestInclude]
        public ContentTypeEntity ContentType { get; set; }
    }
}