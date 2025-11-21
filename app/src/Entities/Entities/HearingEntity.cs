using System;
using System.Collections.Generic;
using Agora.Entities.Attributes;
using Agora.Entities.Common;

namespace Agora.Entities.Entities
{
    public class HearingEntity : AuditableEntity
    {
        public bool ClosedHearing { get; set; }

        public bool ShowComments { get; set; }

        public bool AutoApproveComments { get; set; }

        public string ContactPersonDepartmentName { get; set; }

        public string ContactPersonEmail { get; set; }

        public string ContactPersonName { get; set; }

        public string ContactPersonPhoneNumber { get; set; }

        public string EsdhTitle { get; set; }

        public string EsdhNumber { get; set; }

        public string EsdhMetaData { get; set; }

        public DateTime? Deadline { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ConcludedDate { get; set; }

        // Many-to-one relationship with HearingStatus
        public int? HearingStatusId { get; set; }
        public HearingStatusEntity HearingStatus { get; set; }

        // Many-to-one relationship with HearingType
        public int? HearingTypeId { get; set; }
        public HearingTypeEntity HearingType { get; set; }

        // Many-to-one relationship with SubjectArea
        public int? SubjectAreaId { get; set; }
        public SubjectAreaEntity SubjectArea { get; set; }

        // Many-to-one relationship with CityArea
        public int? CityAreaId { get; set; }
        public CityAreaEntity CityArea { get; set; }

        // Many-to-one relationship with KleHierarchy
        public int? KleHierarchyId { get; set; }
        public KleHierarchyEntity KleHierarchy { get; set; }

        // Many-to-one relationship with JournalizedStatus
        public int? JournalizedStatusId { get; set; }
        public JournalizedStatusEntity JournalizedStatus { get; set; }

        // One-to-many relationship with UserHearingRoleEntity
        [AllowRequestInclude(maxNavigationPathLength: 2)]
        public ICollection<UserHearingRoleEntity> UserHearingRoles { get; set; } = new List<UserHearingRoleEntity>();

        // One-to-many relationship with CompanyHearingRoleEntity
        [AllowRequestInclude(maxNavigationPathLength: 2)]
        public ICollection<CompanyHearingRoleEntity> CompanyHearingRoles { get; set; } = new List<CompanyHearingRoleEntity>();

        // One-to-many relationship with Notification
        public ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();

        // One-to-many relationship with Comment
        public ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();

        // One-to-many relationship with Content
        [AllowRequestInclude(maxNavigationPathLength: 2)]
        public ICollection<ContentEntity> Contents { get; set; } = new List<ContentEntity>();

        // One-to-many relationship with Event
        public ICollection<EventEntity> Events { get; set; } = new List<EventEntity>();

        // One-to-many relationship with NotificationContentSpecification
        public ICollection<NotificationContentSpecificationEntity> NotificationContentSpecifications { get; set; } = new List<NotificationContentSpecificationEntity>();
    }
}