using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces.Services
{
    public interface IInvitationHandler
    {
        Task<InvitationMetaData> CreateInviteesForHearing(Hearing hearing, int sourceId, string sourceName, List<InviteeIdentifiers> inviteeIdentifiers);
        Task<InvitationMetaData> ReplaceInviteesForHearing(Hearing hearing, int sourceId, string sourceName, List<InviteeIdentifiers> inviteeIdentifiers);
    }
}