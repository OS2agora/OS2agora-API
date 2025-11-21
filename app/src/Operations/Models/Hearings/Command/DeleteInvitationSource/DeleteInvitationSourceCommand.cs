using Agora.Models.Common;
using Agora.Models.Common.CustomResponse;
using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Extensions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Services;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.Models.Hearings.Command.DeleteInvitationSource
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class DeleteInvitationSourceCommand : IRequest<MetaDataResponse<Hearing, InvitationMetaData>>
    {
        public int HearingId { get; set; }
        public string InvitationSourceName { get; set; }

        public class DeleteInvitationSourceCommandHandler : IRequestHandler<DeleteInvitationSourceCommand, MetaDataResponse<Hearing, InvitationMetaData>>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IInvitationService _invitationService;
            private readonly IInvitationSourceMappingDao _invitationSourceMappingDao;
            private readonly IUserHearingRoleDao _userHearingRoleDao;
            private readonly ICompanyHearingRoleDao _companyHearingRoleDao;

            public DeleteInvitationSourceCommandHandler(IHearingDao hearingDao, IInvitationService invitationService, IInvitationSourceMappingDao invitationSourceMappingDao, IUserHearingRoleDao userHearingRoleDao, ICompanyHearingRoleDao companyHearingRoleDao)
            {
                _hearingDao = hearingDao;
                _invitationService = invitationService;
                _invitationSourceMappingDao = invitationSourceMappingDao;
                _userHearingRoleDao = userHearingRoleDao;
                _companyHearingRoleDao = companyHearingRoleDao;
            }

            public async Task<MetaDataResponse<Hearing, InvitationMetaData>> Handle(DeleteInvitationSourceCommand request, CancellationToken cancellationToken)
            {
                var hearing = await GetHearingWithIncludes(request.HearingId);

                if (hearing == null)
                {
                    throw new NotFoundException($"Hearing with id {request.HearingId} not found. Cannot perform delete operation");
                }

                if (hearing.IsPublished())
                {
                    throw new InvalidOperationException("Cannot delete invitations when status is not DRAFT, CREATED, or AWAITING_STARTDATE");
                }

                // Find all invitationSourceMappings to delete for the given invitation source
                var invitationSourceMappingIdsToRemove = new List<int>();
                invitationSourceMappingIdsToRemove.AddRange(_invitationService.GetInvitationSourceMappingIdsFromUserHearingRoles(hearing.UserHearingRoles, request.InvitationSourceName));
                invitationSourceMappingIdsToRemove.AddRange(_invitationService.GetInvitationSourceMappingIdsFromCompanyHearingRoles(hearing.CompanyHearingRoles, request.InvitationSourceName));

                // Find all UserHearingRoles and CompanyHearingRoles that should be removed
                var userHearingRoleIdsToRemove = _invitationService.GetUserHearingRoleIdsWithSingleInvitationSourceMapping(hearing.UserHearingRoles, request.InvitationSourceName);
                var companyHearingRoleIdsToRemove = _invitationService.GetCompanyHearingRoleIdsWithSingleInvitationSourceMapping(hearing.CompanyHearingRoles, request.InvitationSourceName);

                await _invitationSourceMappingDao.DeleteRangeAsync(invitationSourceMappingIdsToRemove.ToArray());
                await _userHearingRoleDao.DeleteRangeAsync(userHearingRoleIdsToRemove.ToArray());
                await _companyHearingRoleDao.DeleteRangeAsync(companyHearingRoleIdsToRemove.ToArray());

                // Reload hearing to get updated metadata
                hearing = await GetHearingWithIncludes(request.HearingId);

                return new MetaDataResponse<Hearing, InvitationMetaData>
                {
                    ResponseData = hearing,
                    Meta = new InvitationMetaData
                    {
                        DeletedInvitees = userHearingRoleIdsToRemove.Count + companyHearingRoleIdsToRemove.Count,
                    }
                };
            }

            private async Task<Hearing> GetHearingWithIncludes(int hearingId)
            {
                var systemIncludes = new List<string>
                {
                    $"{nameof(Hearing.HearingStatus)}",
                    $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.InvitationSourceMappings)}",
                    $"{nameof(Hearing.CompanyHearingRoles)}.{nameof(CompanyHearingRole.InvitationSourceMappings)}"
                };

                var includes = IncludeProperties.Create<Hearing>(null, systemIncludes);
                return await _hearingDao.GetAsync(hearingId, includes);
            }
        }
    }
}