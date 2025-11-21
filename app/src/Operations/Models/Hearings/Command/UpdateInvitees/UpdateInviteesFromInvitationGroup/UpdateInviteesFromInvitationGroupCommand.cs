using Agora.Models.Common;
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

namespace Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromInvitationGroup
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class UpdateInviteesFromInvitationGroupCommand : UpdateInviteesBaseCommand
    {
        public int InvitationGroupId { get; set; }

        public class UpdateInviteesFromInvitationGroupCommandHandler : UpdateInviteesBaseCommandHandler<UpdateInviteesFromInvitationGroupCommand>
        {
            private readonly IInvitationGroupDao _invitationGroupDao;
            private readonly IInvitationService _invitationService;
            private readonly IInvitationSourceDao _invitationSourceDao;

            public UpdateInviteesFromInvitationGroupCommandHandler(IHearingDao hearingDao, IInvitationGroupDao invitationGroupDao,
                IInvitationHandler invitationHandler, IInvitationService invitationService, IInvitationSourceDao invitationSourceDao) : base(hearingDao, invitationHandler)
            {
                _invitationGroupDao = invitationGroupDao;
                _invitationService = invitationService;
                _invitationSourceDao = invitationSourceDao;
            }

            protected override async Task<(List<InviteeIdentifiers> identifiers, InvitationSource invitationSource, string sourceName)> GetInvitationData(UpdateInviteesFromInvitationGroupCommand request)
            {
                var invitationSource = await _invitationSourceDao.GetAsync(request.InvitationSourceId);
                if (invitationSource.InvitationSourceType != InvitationSourceType.INVITATION_GROUP)
                {
                    throw new InvalidOperationException("Wrong InvitationSourceType. InvitationSourceType must be of type Invitation_Group when inviting an invitation group");
                }

                var invitationGroup = await _invitationGroupDao.GetAsync(request.InvitationGroupId, IncludeProperties.Create<InvitationGroup>());

                if (invitationGroup == null)
                {
                    throw new NotFoundException(nameof(InvitationGroup), request.InvitationGroupId);
                }

                var hearing = await _hearingDao.GetAsync(request.HearingId, IncludeProperties.Create<Hearing>());

                if (invitationGroup.InvitationGroupMappings.All(mapping => mapping.HearingTypeId != hearing.HearingTypeId))
                {
                    throw new InvalidOperationException(
                        $"Provided invitationGroup with Id {request.InvitationGroupId} cannot be used to create invitees for hearing with Id {request.HearingId}");
                }

                var sourceName = $"{invitationSource.Name}: {invitationGroup.Name}";

                var uploadedInviteeIdentifiers = invitationGroup.InvitationKeys.Select(key => new InviteeIdentifiers
                {
                    Cpr = key.Cpr,
                    Cvr = key.Cvr,
                    Email = key.Email
                }).ToList();

                return (uploadedInviteeIdentifiers, invitationSource, sourceName);
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

                    var message = $"Invitation Keys for InvitationGroup contain invalid {string.Join(" and ", invalidTypes)}.";
                    throw new InvalidOperationException(message);
                }
            }
        }
    }
}