using System.Collections.Generic;
using Agora.Entities.Common;
using Agora.Entities.Enums;

namespace Agora.Entities.Entities
{
    public class InvitationSourceEntity : AuditableEntity
    {
        public string Name { get; set; }
        public InvitationSourceType InvitationSourceType { get; set; }
        public bool CanDeleteIndividuals { get; set; }
        public string CprColumnHeader { get; set; }
        public string EmailColumnHeader { get; set; }
        public string CvrColumnHeader { get; set; }

        // One-to-many relationship with InvitationSourceMapping
        public ICollection<InvitationSourceMappingEntity> InvitationSourceMappings { get; set; } = new List<InvitationSourceMappingEntity>();
    }
}
