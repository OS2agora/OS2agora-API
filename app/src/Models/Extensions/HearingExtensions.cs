using Agora.Models.Models;

namespace Agora.Models.Extensions
{
    public static class HearingExtensions
    {
        public static string CreateLinkToHearing(this Hearing hearing, string internalUrl, string publicUrl)
        {
            var isInternal = hearing.HearingType.IsInternalHearing;
            return isInternal
                ? internalUrl.Replace("{hearingId}", hearing.Id.ToString())
                : publicUrl.Replace("{hearingId}", hearing.Id.ToString());
        }
    }
}