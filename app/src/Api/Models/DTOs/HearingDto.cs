using System;
using System.Collections.Generic;
using BallerupKommune.Api.Models.Common;
using BallerupKommune.Api.Models.JsonApi;

namespace BallerupKommune.Api.Models.DTOs
{
    public class HearingDto : BaseDto<HearingDto.HearingAttributeDto>
    {
        public override List<DtoRelationship> AllowedRelationships => new List<DtoRelationship>
        {
            new DtoRelationship("subjectArea"),
            new DtoRelationship("hearingStatus"),
            new DtoRelationship("kleHierarchy"),
            new DtoRelationship("hearingType")
        };

        public class HearingAttributeDto : BaseAttributeDto
        {
            private bool _closedHearing;
            private bool _showComments;
            private string _contactPersonDepartmentName;
            private string _contactPersonEmail;
            private string _contactPersonName;
            private string _contactPersonPhoneNumber;
            private string _esdhTitle;
            private string _esdhNumber;
            private DateTime? _deadline;
            private DateTime? _startDate;

            public bool ClosedHearing
            {
                get => _closedHearing;
                set { _closedHearing = value; PropertyUpdated(); }
            }


            public bool ShowComments
            {
                get => _showComments;
                set { _showComments = value; PropertyUpdated(); }
            }

            public string ContactPersonDepartmentName
            {
                get => _contactPersonDepartmentName;
                set { _contactPersonDepartmentName = value; PropertyUpdated(); }
            }

            public string ContactPersonEmail
            {
                get => _contactPersonEmail;
                set { _contactPersonEmail = value; PropertyUpdated(); }
            }

            public string ContactPersonName
            {
                get => _contactPersonName;
                set { _contactPersonName = value; PropertyUpdated(); }
            }

            public string ContactPersonPhoneNumber
            {
                get => _contactPersonPhoneNumber;
                set { _contactPersonPhoneNumber = value; PropertyUpdated(); }
            }

            public string EsdhTitle
            {
                get => _esdhTitle;
                set { _esdhTitle = value; PropertyUpdated(); }
            }

            public string EsdhNumber
            {
                get => _esdhNumber;
                set { _esdhNumber = value; PropertyUpdated(); }
            }

            public DateTime? Deadline
            {
                get => _deadline;
                set { _deadline = value; PropertyUpdated(); }
            }

            public DateTime? StartDate
            {
                get => _startDate;
                set { _startDate = value; PropertyUpdated(); }
            }
        }
    }
}