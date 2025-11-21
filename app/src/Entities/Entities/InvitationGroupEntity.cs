using Agora.Entities.Common;
using System.Collections.Generic;

namespace Agora.Entities.Entities
{
    public class InvitationGroupEntity : AuditableEntity
    {
        public string Name { get; set; }

        // One-to-many relationship with InvitationKey
        public ICollection<InvitationKeyEntity> InvitationKeys { get; set; } = new List<InvitationKeyEntity>();

        // One-to-many relationship with InvitationGroupMapping
        public ICollection<InvitationGroupMappingEntity> InvitationGroupMappings { get; set; } = new List<InvitationGroupMappingEntity>();
    }
}