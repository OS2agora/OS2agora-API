using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Extensions;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Models.InvitationKeys.Commands.UpdateInvitationKeys
{
    [PreAuthorize("HasRole('Administrator')")]
    public class UpdateInvitationKeysCommand : IRequest<List<InvitationKey>>
    {
        public List<InvitationKey> InvitationKeys { get; set; }
        public int InvitationGroupId { get; set; }

        public class UpdateInvitationKeysCommandHandler : IRequestHandler<UpdateInvitationKeysCommand, List<InvitationKey>>
        {
            private readonly IInvitationKeyDao _invitationKeyDao;
            private readonly IInvitationGroupDao _invitationGroupDao;

            public UpdateInvitationKeysCommandHandler(IInvitationGroupDao invitationGroupDao, IInvitationKeyDao invitationKeyDao)
            {
                _invitationGroupDao = invitationGroupDao;
                _invitationKeyDao = invitationKeyDao;
            }

            public async Task<List<InvitationKey>> Handle(UpdateInvitationKeysCommand request, CancellationToken cancellationToken)
            {
                var invitationGroupIncludes = IncludeProperties.Create<InvitationGroup>(null, new List<string>
                {
                    nameof(InvitationGroup.InvitationKeys)
                });
                var invitationGroup = await _invitationGroupDao.GetAsync(request.InvitationGroupId, invitationGroupIncludes);

                if (invitationGroup == null)
                {
                    throw new NotFoundException(nameof(InvitationGroup), request.InvitationGroupId);
                }

                List<InvitationKey> distinctInvitationKeys;
                try
                {
                    // Remove duplicated invitationKeys entries from the request
                    distinctInvitationKeys = request.InvitationKeys
                        .GroupBy(invitationKey => new
                        {
                            Cpr = invitationKey.Cpr?.NormalizeAndValidateCpr(),
                            Cvr = invitationKey.Cvr?.NormalizeAndValidateCvr(),
                            Email = invitationKey.Email?.NormalizeAndValidateEmail()
                        })
                        .Select(group => group.First())
                        .Where(invitationKey =>
                            !string.IsNullOrWhiteSpace(invitationKey.Cpr) ||
                            !string.IsNullOrWhiteSpace(invitationKey.Cvr) ||
                            !string.IsNullOrWhiteSpace(invitationKey.Email))
                        .ToList();
                }
                catch (ValidationException exception)
                {
                    throw new ValidationException("Invalid InvitationKey found", exception);
                }


                // Find InvitationKeys to create
                var invitationKeysToCreate = distinctInvitationKeys.Where(incoming =>
                    !invitationGroup.InvitationKeys.Any(existing => AreInvitationKeysEqual(existing, incoming))).ToList();

                // Find InvitationKeys to delete
                var invitationKeysToDelete = invitationGroup.InvitationKeys.Where(existing =>
                    !distinctInvitationKeys.Any(incoming => AreInvitationKeysEqual(existing, incoming))).ToList();

                // Delete invitationKeys
                var idsToDelete = invitationKeysToDelete.Select(invitationKey => invitationKey.Id).ToArray();
                await _invitationKeyDao.DeleteRangeAsync(idsToDelete);

                // Create invitationKeys
                var includes = IncludeProperties.Create<InvitationKey>(null, new List<string>
                {
                    nameof(InvitationGroupMapping.InvitationGroup)
                });
                var allInvitationKeys = await _invitationKeyDao.CreateRangeAsync(invitationKeysToCreate, includes);
                var allInvitationKeysForInvitationGroup = allInvitationKeys.Where(x => x.InvitationGroupId == request.InvitationGroupId);

                return allInvitationKeysForInvitationGroup.ToList();
            }

            private static bool AreInvitationKeysEqual(InvitationKey existing, InvitationKey incoming)
            {
                var existingCpr = existing.Cpr?.Trim();
                var incomingCpr = incoming.Cpr?.Trim();
                var existingCvr = existing.Cvr?.Trim();
                var incomingCvr = incoming.Cvr?.Trim();
                var existingEmail = existing.Email?.Trim();
                var incomingEmail = incoming.Email?.Trim();

                return string.Equals(existingCpr, incomingCpr, System.StringComparison.InvariantCultureIgnoreCase) &&
                       string.Equals(existingCvr, incomingCvr, System.StringComparison.InvariantCultureIgnoreCase) &&
                       string.Equals(existingEmail, incomingEmail, System.StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}