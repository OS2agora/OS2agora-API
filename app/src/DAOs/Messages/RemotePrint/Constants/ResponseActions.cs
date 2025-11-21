using System;
using System.Collections.Generic;

namespace Agora.DAOs.Messages.RemotePrint.Constants
{
    /// <summary>
    /// All actions are grouped into three stages, which matches the NotificationStatus used in AGORA.
    /// The grouping is based on the documentation provided for SF1601.
    /// The documentation package can be downloaded here: https://digitaliseringskatalog.dk/integration/sf1601
    /// </summary>
    public static class ResponseActions
    {
        public static readonly IReadOnlySet<string> FailedActions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                RemotePrint.Actions.Fejlet,
                RemotePrint.Actions.Tilbagekaldt,
                DigitalPost.Actions.Fejlet
            };

        public static readonly IReadOnlySet<string> SuccessfulActions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                DigitalPost.Actions.AfleveretDigitalPost,
                RemotePrint.Actions.ModtagetPostDanmark,
            };

        public static readonly IReadOnlySet<string> AwaitingActions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                RemotePrint.Actions.Afsendt,
                RemotePrint.Actions.ModtagetFjernprint,
                RemotePrint.Actions.Klar,
                RemotePrint.Actions.AfleveretTilPrintOgKuvertering,
                RemotePrint.Actions.OpdateringFraPostDanmark,

                DigitalPost.Actions.ModtagetDigitalPost
            };
    }
}
