using Agora.DTOs.Common;
using System.Collections.Generic;

namespace Agora.DTOs.Models
{
    public class InvitationGroupDto : AuditableDto<InvitationGroupDto>
    {
        public string Name { get; set; }

        public ICollection<InvitationKeyDto> InvitationKeys { get; set; } = new List<InvitationKeyDto>();

        public ICollection<InvitationGroupMappingDto> InvitationGroupMappings { get; set; } = new List<InvitationGroupMappingDto>();
    }
}