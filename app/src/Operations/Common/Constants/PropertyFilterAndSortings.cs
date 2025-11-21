using System.Collections.Generic;
using Agora.Models.Models;

namespace Agora.Operations.Common.Constants
{
    public static class PropertyFilterAndSorting
    {
        public static class HearingProperties
        {
            public static readonly string Deadline = "Deadline";
            public static readonly string StartDate = "StartDate";
            public static readonly string HearingStatus = "HearingStatus";
            public static readonly string HearingType = "HearingType";
            public static readonly string SubjectArea = "SubjectArea";
            public static readonly string CityArea = "CityArea";
            public static readonly string Title = "Title";
            public static readonly string UserHearingRole = "UserHearingRole";
            public static readonly string CompanyHearingRole = "CompanyHearingRole";
            public static readonly string HearingOwner = "HearingOwner";

            public static class Groups
            {
                public static readonly string Deadline = "Deadline";
                public static readonly string StartDate = "StartDate";
                public static readonly string HearingStatus = "HearingStatus";
                public static readonly string HearingType = "HearingType";
                public static readonly string SubjectArea = "SubjectArea";
                public static readonly string CityArea = "CityArea";
                public static readonly string Title = "Title";
                public static readonly string HearingRole = "HearingRole";
                public static readonly string HearingOwner = "HearingOwner";
            }

            public static class Includes
            {
                public static readonly List<string> Deadline = new List<string>();
                public static readonly List<string> StartDate = new List<string>();
                public static readonly List<string> HearingStatus = new List<string>
                {
                    nameof(Hearing.HearingStatus)
                };
                public static readonly List<string> HearingType = new List<string>
                {
                    nameof(Hearing.HearingType)
                };
                public static readonly List<string> SubjectArea = new List<string>
                {
                    nameof(Hearing.SubjectArea)
                };
                public static readonly List<string> CityArea = new List<string>
                {
                    nameof(Hearing.CityArea)
                };
                public static readonly List<string> Title = new List<string>
                {
                    nameof(Hearing.Contents),
                    $"{nameof(Hearing.Contents)}.{nameof(Content.Field)}",
                    $"{nameof(Hearing.Contents)}.{nameof(Content.Field)}.{nameof(Content.Field.FieldType)}"
                };
                public static readonly List<string> UserHearingRole = new List<string>
                {
                    nameof(Hearing.UserHearingRoles),
                    $"{nameof(Hearing.UserHearingRoles)}.{nameof(Agora.Models.Models.UserHearingRole.HearingRole)}"
                };

                public static readonly List<string> CompanyHearingRole = new List<string>
                {
                    nameof(Hearing.CompanyHearingRoles),
                    $"{nameof(Hearing.CompanyHearingRoles)}.{nameof(Agora.Models.Models.CompanyHearingRole.HearingRole)}"
                };
            }
        }
    }
}
