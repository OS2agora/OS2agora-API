using BallerupKommune.Entities.Common;
using System.Collections.Generic;

namespace BallerupKommune.Entities.Entities
{
    public class GlobalContentTypeEntity : AuditableEntity
    {
        public string Name { get; set; }

        public Enums.GlobalContentType Type { get; set; }

        //One-to-many relationship with GlobalContents
        public ICollection<GlobalContentEntity> GlobalContents { get; set; } = new List<GlobalContentEntity>();
    }
}