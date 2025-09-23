using System.Collections.Generic;
using System.Linq;
using BallerupKommune.Models.Models;

namespace BallerupKommune.TestUtilities.Extensions
{
    public static class HearingExtensions
    {
        public static bool ContainsHearingInRole(this List<Hearing> hearings, int hearingId, int userId, BallerupKommune.Models.Enums.HearingRole role)
        {
            var hearing = hearings.FirstOrDefault(x => x.Id == hearingId);
            return IsHearingInRole(hearing, userId, role);
        }

        public static bool IsHearingInRole(this Hearing hearing, int userId, BallerupKommune.Models.Enums.HearingRole role)
        {
            if (hearing?.UserHearingRoles == null)
            {
                return false;
            }
            return hearing.UserHearingRoles.Any(uhr =>
                uhr.User.Id == userId &&
                uhr.HearingRole.Role == role);
        }

        public static bool ContainsPublishedHearing(this List<Hearing> hearings, int hearingId)
        {
            var hearing = hearings.FirstOrDefault(x => x.Id == hearingId);
            return IsHearingPublished(hearing);
        }
        public static bool IsHearingPublished(this Hearing hearing)
        {
            if (hearing?.HearingStatus?.Status == null)
            {
                return false;
            }
            return hearing.HearingStatus.Status != BallerupKommune.Models.Enums.HearingStatus.DRAFT &&
                   hearing.HearingStatus.Status != BallerupKommune.Models.Enums.HearingStatus.CREATED;
        }

        public static bool ContainsInternalHearing(this List<Hearing> hearings, int hearingId)
        {
            var hearing = hearings.FirstOrDefault(x => x.Id == hearingId);
            return IsInternalHearing(hearing);
        }

        public static bool IsInternalHearing(this Hearing hearing)
        {
            if (hearing?.HearingType == null)
            {
                return false;
            }
            return hearing.HearingType.IsInternalHearing;
        }

    }

}
