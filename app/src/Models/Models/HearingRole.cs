using System.Collections.Generic;
using BallerupKommune.Models.Common;
using NovaSec.Attributes;

namespace BallerupKommune.Models.Models
{
    [PostFilter("HasAnyRole(['Administrator', 'HearingOwner'])")]
    [PostFilter("HasRole('Employee') && @Security.HasRoleOnAnyHearing(resultObject.Role)")]
    public class HearingRole : AuditableModel
    {
        public Enums.HearingRole Role { get; set; }

        public string Name { get; set; }

        public ICollection<UserHearingRole> UserHearingRoles { get; set; } = new List<UserHearingRole>();

        public ICollection<CompanyHearingRole> CompanyHearingRoles { get; set; } = new List<CompanyHearingRole>();
    }
}
