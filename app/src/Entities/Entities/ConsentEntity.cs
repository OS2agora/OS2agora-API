using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class ConsentEntity : AuditableEntity
    {
        //Many-to-one relationship with Global Content
        public int GlobalContentId { get; set; }
        public GlobalContentEntity GlobalContent { get; set; }

        //One-to-one relationship with Comment
        public CommentEntity Comment { get; set; }
    }
}