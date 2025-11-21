using System.Collections.Generic;
using Agora.Models.Common;
using NovaSec.Attributes;

namespace Agora.Models.Models
{
    [PostFilter("HasAnyRole(['Administrator', 'HearingOwner'])")]
    [PostFilter("@Security.CanSeeSubjectArea(resultObject)")]
    public class SubjectArea : AuditableModel
    {
        public bool IsActive { get; set; }
        public string Name { get; set; }

        public ICollection<Hearing> Hearings { get; set; } = new List<Hearing>();
    }
}
