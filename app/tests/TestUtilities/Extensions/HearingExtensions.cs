using System.Collections.Generic;
using System.Linq;
using Agora.Models.Models;

namespace Agora.TestUtilities.Extensions
{
    public static class HearingExtensions
    {
        public static bool ContainsHearingInRole(this List<Hearing> hearings, int hearingId, int userId, Agora.Models.Enums.HearingRole role)
        {
            var hearing = hearings.FirstOrDefault(x => x.Id == hearingId);
            return IsHearingInRole(hearing, userId, role);
        }

        public static bool IsHearingInRole(this Hearing hearing, int userId, Agora.Models.Enums.HearingRole role)
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
            return hearing.HearingStatus.Status != Agora.Models.Enums.HearingStatus.DRAFT &&
                   hearing.HearingStatus.Status != Agora.Models.Enums.HearingStatus.CREATED;
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
