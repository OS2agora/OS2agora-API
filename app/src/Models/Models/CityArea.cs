using System.Collections.Generic;
using Agora.Models.Common;
using NovaSec.Attributes;

namespace Agora.Models.Models
{
    [PostFilter("HasAnyRole(['Administrator', 'HearingOwner'])")]
    [PostFilter("@Security.CanSeeCityArea(resultObject)")]
    public class CityArea : AuditableModel
    {
        public bool IsActive { get; set; }
        public string Name { get; set; }

        public ICollection<Hearing> Hearings { get; set; } = new List<Hearing>();
    }
}