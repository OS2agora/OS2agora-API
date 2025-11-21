using System;
using System.Collections.Generic;

namespace Agora.DAOs.Messages.RemotePrint.Constants
{
    public static class ResponseActors
    {
        public static readonly IReadOnlySet<string> DigitalPostActors =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                DigitalPost.ActorNames.SentLetter,
                DigitalPost.ActorNames.GetStatus,
                DigitalPost.ActorNames.KombiCodeDigital,
            };

        public static readonly IReadOnlySet<string> RemotePrintActors =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                RemotePrint.ActorNames.SentLetter,
                RemotePrint.ActorNames.GetStatusStraalfors,
                RemotePrint.ActorNames.GetStatusEdora,
                RemotePrint.ActorNames.GetStatusKMD,
                RemotePrint.ActorNames.KombiCodePhysical
            };
    }
}
