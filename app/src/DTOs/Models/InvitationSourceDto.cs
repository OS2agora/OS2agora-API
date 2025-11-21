using System.Collections.Generic;
using Agora.DTOs.Common;
using Agora.DTOs.Enums;

namespace Agora.DTOs.Models
{
    public class InvitationSourceDto : AuditableDto<InvitationSourceDto>
    {
        public string Name { get; set; }
        public InvitationSourceType InvitationSourceType { get; set; }
        public bool CanDeleteIndividuals { get; set; }
        public string CprColumnHeader { get; set; }
        public string EmailColumnHeader { get; set; }
        public string CvrColumnHeader { get; set; }
        public ICollection<InvitationSourceMappingDto> InvitationSourceMappings { get; set; } = new List<InvitationSourceMappingDto>();

    }
}
