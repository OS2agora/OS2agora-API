using Agora.Models.Common;
using System.Collections.Generic;
using Enum = Agora.Models.Enums;

namespace Agora.Models.Models
{
    public class GlobalContentType : AuditableModel
    {
        public string Name { get; set; }

        public Enum.GlobalContentType Type { get; set; }

        public ICollection<GlobalContent> GlobalContents { get; set; } = new List<GlobalContent>();
    }
}