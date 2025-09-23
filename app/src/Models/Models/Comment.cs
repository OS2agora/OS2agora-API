using BallerupKommune.Models.Common;
using System.Collections.Generic;

namespace BallerupKommune.Models.Models
{
    public class Comment : AuditableModel
    {
        public int Number { get; set; }

        public bool IsDeleted { get; set; }
        public bool ContainsSensitiveInformation { get; set; }

        public string OnBehalfOf { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int CommentStatusId { get; set; } 
        public CommentStatus CommentStatus { get; set; }

        public int HearingId { get; set; }
        public Hearing Hearing { get; set; }

        public int CommentTypeId { get; set; }
        public CommentType CommentType { get; set; }

        public int? CommentParrentId { get; set; }
        public Comment CommentParrent { get; set; }
        public int? CommentDeclineInfoId { get; set; }
        public CommentDeclineInfo CommentDeclineInfo { get; set; }
        public int? ConsentId { get; set; }
        public Consent Consent { get; set; }

        public ICollection<Comment> CommentChildren { get; set; } = new List<Comment>();

        public ICollection<Content> Contents { get; set; } = new List<Content>();

        public static List<string> DefaultIncludes => new List<string>
        {
            "CommentType",
            "CommentStatus",
            "CommentParrent",
            "CommentDeclineInfo",
            "User",
        };
    }
}