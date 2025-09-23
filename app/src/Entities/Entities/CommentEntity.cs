using BallerupKommune.Entities.Attributes;
using BallerupKommune.Entities.Common;
using System.Collections.Generic;

namespace BallerupKommune.Entities.Entities
{
    public class CommentEntity : AuditableEntity
    {
        public int Number { get; set; }

        public bool IsDeleted { get; set; }

        public bool ContainsSensitiveInformation { get; set; }

        public string OnBehalfOf { get; set; }

        // Many-to-one relationship with UserEntity
        public int UserId { get; set; }
        public UserEntity User { get; set; }

        // Many-to-one relationship with CommentStatus
        public int CommentStatusId { get; set; }
        public CommentStatusEntity CommentStatus { get; set; }

        public int? CommentDeclineInfoId { get; set; }
        [AllowRequestInclude]
        public CommentDeclineInfoEntity CommentDeclineInfo { get; set; }

        // Many-to-one relationship with Hearing
        public int HearingId { get; set; }
        public HearingEntity Hearing { get; set; }

        // Many-to-one relationship with CommentType
        public int CommentTypeId { get; set; }
        public CommentTypeEntity CommentType { get; set; }

        // Many-to-one relationship with Comment (parrent comment)
        public int? CommentParrentId { get; set; }
        public CommentEntity CommentParrent { get; set; }

        // One-to-many relationship with Comments (child comments)
        public ICollection<CommentEntity> CommentChildren { get; set; } = new List<CommentEntity>();

        // One-to-many relationship with Content
        [AllowRequestInclude(maxNavigationPathLength: 2)]
        public ICollection<ContentEntity> Contents { get; set; } = new List<ContentEntity>();

        // One-to-one relationship with Consent
        public int? ConsentId { get; set; }
        public ConsentEntity Consent { get; set; }
    }
}
