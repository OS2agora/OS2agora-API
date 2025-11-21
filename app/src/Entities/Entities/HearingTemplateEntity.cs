using System.Collections.Generic;
using Agora.Entities.Attributes;
using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class HearingTemplateEntity : AuditableEntity
    {
        public string Name { get; set; }

        // One-to-many relatinship with HearingType
        public ICollection<HearingTypeEntity> HearingTypes { get; set; } = new List<HearingTypeEntity>();

        // One-to-many relationship with Field
        [AllowRequestInclude(maxNavigationPathLength: 3)]
        public ICollection<FieldEntity> Fields { get; set; } = new List<FieldEntity>();
    }
}
