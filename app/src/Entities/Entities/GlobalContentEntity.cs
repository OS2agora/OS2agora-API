using Agora.Entities.Common;
using System.Collections.Generic;

namespace Agora.Entities.Entities
{
    public class GlobalContentEntity : AuditableEntity
    {
        public int Version { get; set; }

        public string Content { get; set; }

        //Many-to-one relationship with GlobalContentType
        public GlobalContentTypeEntity GlobalContentType { get; set; }
        public int GlobalContentTypeId { get; set; }

        //One-to-many relationship with Consent
        public ICollection<ConsentEntity> Consents { get; set; } = new List<ConsentEntity>();
    }
}