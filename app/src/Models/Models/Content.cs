using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class Content : AuditableModel
    {
        public string TextContent { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileContentType { get; set; }

        public int? CommentId { get; set; }
        public Comment Comment { get; set; }

        public int HearingId { get; set; }
        public Hearing Hearing { get; set; }

        public int? FieldId { get; set; }
        public Field Field { get; set; }

        public int ContentTypeId { get; set; }
        public ContentType ContentType { get; set; }
    }
}
