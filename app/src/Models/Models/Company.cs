using System.Collections.Generic;
using Agora.Models.Common;

namespace Agora.Models.Models
{
    public class Company : AuditableModel
    {
        public string Cvr { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Municipality { get; set; }
        public string StreetName { get; set; }
        public ICollection<CompanyHearingRole> CompanyHearingRoles { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}