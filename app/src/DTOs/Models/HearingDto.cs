using System;
using System.Collections.Generic;
using Agora.DTOs.Common;

namespace Agora.DTOs.Models
{
    public class HearingDto : AuditableDto<HearingDto>
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

        public DateTime? Deadline { get; set; }
        public DateTime? StartDate { get; set; }

        public int? CommentAmount { get; set; }

        public BaseDto<JournalizedStatusDto> JournalizedStatus { get; set; }

        public BaseDto<HearingStatusDto> HearingStatus { get; set; }

        public BaseDto<HearingTypeDto> HearingType { get; set; }

        public BaseDto<SubjectAreaDto> SubjectArea { get; set; }

        public BaseDto<CityAreaDto> CityArea { get; set; }

        public BaseDto<KleHierarchyDto> KleHierarchy { get; set; }

        public ICollection<UserHearingRoleDto> UserHearingRoles { get; set; } = new List<UserHearingRoleDto>();

        public ICollection<CompanyHearingRoleDto> CompanyHearingRoles { get; set; } = new List<CompanyHearingRoleDto>();

        public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();

        public ICollection<ContentDto> Contents { get; set; } = new List<ContentDto>();
    }
}