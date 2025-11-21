using Agora.Models.Enums;
using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Services;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromPersonalInviteeIdentifiers
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class UpdateInviteesFromPersonalInviteeIdentifiersCommand : UpdateInviteesBaseCommand
    {
        public List<InviteeIdentifiers> InviteeIdentifiers { get; set; }

        public class UpdateInviteesFromPersonalInviteeIdentifiersCommandHandler : UpdateInviteesBaseCommandHandler<UpdateInviteesFromPersonalInviteeIdentifiersCommand>
        {
            private readonly IInvitationService _invitationService;
            private readonly IInvitationSourceDao _invitationSourceDao;

            public UpdateInviteesFromPersonalInviteeIdentifiersCommandHandler(IHearingDao hearingDao, IInvitationHandler invitationHandler,
                IInvitationService invitationService, IInvitationSourceDao invitationSourceDao) : base(hearingDao, invitationHandler)
            {
                _invitationService = invitationService;
                _invitationSourceDao = invitationSourceDao;
            }

            protected override async Task<(List<InviteeIdentifiers> identifiers, InvitationSource invitationSource, string sourceName)> GetInvitationData(UpdateInviteesFromPersonalInviteeIdentifiersCommand request)
            {
                var invitationSource = await _invitationSourceDao.GetAsync(request.InvitationSourceId);
                if (invitationSource.InvitationSourceType != InvitationSourceType.PERSONAL)
                {
                    throw new InvalidOperationException("Wrong InvitationSourceType. InvationSourceType must be of type Personal when inviting individual invitees");
                }

                var sourceName = invitationSource.Name;

                return (request.InviteeIdentifiers, invitationSource, sourceName);

            }

            protected override void NormalizeAndValidateInviteeIdentifiers(List<InviteeIdentifiers> inviteeIdentifiers)
            {
                var (isValid, errors) = _invitationService.NormalizeAndValidateInviteeIdentifiers(inviteeIdentifiers);

                if (!isValid)
                {
                    var invalidTypes = new List<string>();

                    if (errors.Any(e => e is InvalidCprException))
                    {
                        invalidTypes.Add("Cpr numbers");
                    }
                    if (errors.Any(e => e is InvalidCvrException))
                    {
                        invalidTypes.Add("Cvr numbers");
                    }
                    if (errors.Any(e => e is InvalidEmailException))
                    {
                        invalidTypes.Add("Emails");
                    }

                    var message = $"InviteeIdentifiers contain invalid {string.Join(" and ", invalidTypes)}.";
                    throw new InvalidOperationException(message);
                }
            }
        }
    }
}