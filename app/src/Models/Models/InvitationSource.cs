using Agora.Models.Enums;
using System.Collections.Generic;
using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class InvitationSource : AuditableModel
    {
        public string Name { get; set; }
        public InvitationSourceType InvitationSourceType { get; set; }
        public bool CanDeleteIndividuals { get; set; }
        public string CprColumnHeader { get; set; }
        public string EmailColumnHeader { get; set; }
        public string CvrColumnHeader { get; set; }
        public ICollection<InvitationSourceMapping> InvitationSourceMappings { get; set; } = new List<InvitationSourceMapping>();
    }
}