using System.Collections.Generic;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Models.Models
{
    public class CommentStatus : AuditableModel
    {
        public Enums.CommentStatus Status { get; set; }

        public int CommentTypeId { get; set; }
        public CommentType CommentType { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public static List<string> DefaultIncludes => new List<string>
        {
            "CommentType"
        };
    }
}
