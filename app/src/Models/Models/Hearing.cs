using BallerupKommune.Models.Common;
using NovaSec.Attributes;
using System;
using System.Collections.Generic;

namespace BallerupKommune.Models.Models
{
    [PostFilter("HasRole('Administrator')")]
    [PostFilter("@Security.IsHearingOwner(resultObject.Id)")]
    [PostFilter("@Security.IsHearingReviewer(resultObject.Id)")]
    [PostFilter("@Security.IsHearingInvitee(resultObject) && @Security.IsHearingPublished(resultObject)",
        "UserHearingRoles, HearingType.FieldTemplates, HearingType.HearingTemplate.Fields.FieldTemplates")]
    [PostFilter("HasRole('Employee') && @Security.IsHearingPublished(resultObject) && !resultObject.ClosedHearing && @Security.IsInternalHearing(resultObject)",
        "UserHearingRoles, HearingType.FieldTemplates, HearingType.HearingTemplate.Fields.FieldTemplates")]
    [PostFilter("HasAnyRole(['Anonymous', 'Citizen']) && @Security.IsHearingPublished(resultObject) && !resultObject.ClosedHearing && !@Security.IsInternalHearing(resultObject)",
        "UserHearingRoles, HearingType.FieldTemplates, HearingType.HearingTemplate.Fields.FieldTemplates")]
    public class Hearing : AuditableModel
    {
        public bool ClosedHearing { get; set; }
        public bool ShowComments { get; set; }

        public string ContactPersonDepartmentName { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonPhoneNumber { get; set; }
        public string EsdhTitle { get; set; }
        public string EsdhNumber { get; set; }
        public string EsdhMetaData { get; set; }

        public DateTime? Deadline { get; set; }
        public DateTime? StartDate { get; set; }

        public int? HearingStatusId { get; set; }
        public int? CommentAmount { get; set; }

        public HearingStatus HearingStatus { get; set; }
        
        
        public int? HearingTypeId { get; set; }
        public HearingType HearingType { get; set; }
        

        public int? SubjectAreaId { get; set; }
        public SubjectArea SubjectArea { get; set; }
        

        public int? KleHierarchyId { get; set; }
        public KleHierarchy KleHierarchy { get; set; }


        public int? JournalizedStatusId { get; set; }
        public JournalizedStatus JournalizedStatus { get; set; }

        public ICollection<UserHearingRole> UserHearingRoles { get; set; } = new List<UserHearingRole>();

        public ICollection<CompanyHearingRole> CompanyHearingRoles { get; set; } = new List<CompanyHearingRole>();

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Content> Contents { get; set; } = new List<Content>();

        public static List<string> DefaultIncludes => new List<string>
        {
            "HearingStatus",
            "HearingType",
            "UserHearingRoles",
            "UserHearingRoles.User",
            "CompanyHearingRoles",
            "CompanyHearingRoles.Company"
        };
    }
}