using BallerupKommune.Models.Common;
using System.Collections.Generic;

namespace BallerupKommune.Models.Models
{
    public class GlobalContent : AuditableModel
    {
        public int Version { get; set; }

        public string Content { get; set; }

        public int GlobalContentTypeId { get; set; }
        public GlobalContentType GlobalContentType { get; set; }

        public ICollection<Consent> Consents { get; set; } = new List<Consent>();

        public static List<string> DefaultIncludes => new List<string>
        {
            "GlobalContentType"
        };
    }
}