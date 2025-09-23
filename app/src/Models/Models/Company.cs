using System.Collections.Generic;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class Company : AuditableModel
    {
        public string Cvr { get; set; }
        public string Name { get; set; }
        public ICollection<CompanyHearingRole> CompanyHearingRoles { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
