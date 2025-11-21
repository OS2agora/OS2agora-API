using Agora.Models.Common;
using Agora.Models.Common.CustomResponse;
using Agora.Models.Common.Services.InvitationService;
using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Services;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Operations.Common.Extensions;
using HearingRole = Agora.Models.Enums.HearingRole;
using HearingStatus = Agora.Models.Enums.HearingStatus;
using InvalidOperationException = Agora.Operations.Common.Exceptions.InvalidOperationException;

namespace Agora.Operations.Models.Hearings.Command.DeleteInvitationSourceMappings
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class DeleteInvitationSourceMappingsCommand : IRequest<MetaDataResponse<Hearing, InvitationMetaData>>
    {
        public int HearingId { get; set; }
        public List<int> InvitationSourceMappingIds { get; set; }
        public bool DeleteFromAllInvitationSources { get; set; }

        public class DeleteInvitationSourceMappingsCommandHandler : IRequestHandler<
            DeleteInvitationSourceMappingsCommand, MetaDataResponse<Hearing, InvitationMetaData>>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IInvitationSourceMappingDao _invitationSourceMappingDao;
            private readonly IUserHearingRoleDao _userHearingRoleDao;
            private readonly ICompanyHearingRoleDao _companyHearingRoleDao;
            private readonly IInvitationService _invitationService;

            public DeleteInvitationSourceMappingsCommandHandler(IHearingDao hearingDao, IInvitationSourceMappingDao invitationSourceMappingDao,
                IUserHearingRoleDao userHearingRoleDao, ICompanyHearingRoleDao companyHearingRoleDao, IInvitationService invitationService)
            {
                _hearingDao = hearingDao;
                _invitationSourceMappingDao = invitationSourceMappingDao;
                _userHearingRoleDao = userHearingRoleDao;
                _companyHearingRoleDao = companyHearingRoleDao;
                _invitationService = invitationService;
            }

            public async Task<MetaDataResponse<Hearing, InvitationMetaData>> Handle(
                DeleteInvitationSourceMappingsCommand request, CancellationToken cancellationToken)
            {
                // Load hearing with all necessary includes and validate it exists and its status
                var hearing = await GetHearingWithIncludes(request.HearingId, true);
                if (hearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.HearingId);
                }
                
                if (hearing.IsPublished())
                {
                    throw new InvalidOperationException("Cannot delete invitations when status is not DRAFT, CREATED, or AWAITING_STARTDATE");
                }

                // Get all invitationSourceMapping that exist on hearing
                var allInvitationSourceMappingsOnHearing = new List<InvitationSourceMapping>();
                allInvitationSourceMappingsOnHearing.AddRange(
                    hearing.UserHearingRoles.SelectMany(uhr => uhr.InvitationSourceMappings));
                allInvitationSourceMappingsOnHearing.AddRange(
                    hearing.CompanyHearingRoles.SelectMany(chr => chr.InvitationSourceMappings));

                // Validate that the requested invitationSourceMappingIds exist on the hearing
                var allInvitationSourceMappingIds = allInvitationSourceMappingsOnHearing.Select(ism => ism.Id).ToHashSet();
                foreach (var invitationSourceMappingId in request.InvitationSourceMappingIds)
                {
                    if (!allInvitationSourceMappingIds.Contains(invitationSourceMappingId))
                    {
                        throw new NotFoundException(
                            $"No InvitationSourceMapping with id {invitationSourceMappingId} on hearing with id {hearing.Id}");
                    }
                }

                // Get all UserHearingRoles and CompanyHearingRoles that exist on hearing
                var allUserHearingRolesOnHearing = hearing.UserHearingRoles
                    .Where(uhr => uhr.HearingRole.Role == HearingRole.HEARING_INVITEE)
                    .ToList();
                var allCompanyHearingRolesOnHearing = hearing.CompanyHearingRoles
                    .Where(uhr => uhr.HearingRole.Role == HearingRole.HEARING_INVITEE)
                    .ToList();

                // Get the invitationSourceMappings to delete
                var invitationSourceMappingIdsSet = request.InvitationSourceMappingIds.ToHashSet();
                var invitationSourceMappingsToDelete = allInvitationSourceMappingsOnHearing
                    .Where(ism => invitationSourceMappingIdsSet.Contains(ism.Id)).ToList();

                GetInvitationSourceMappingsToDeleteResponse response;

                if (request.DeleteFromAllInvitationSources)
                {
                    response = _invitationService.GetInvitationSourceMappingsToDeleteFromAllSources(
                        invitationSourceMappingsToDelete,
                        allUserHearingRolesOnHearing,
                        allCompanyHearingRolesOnHearing);
                }
                else
                {
                    response = _invitationService.GetInvitationSourceMappingsToDelete(
                        invitationSourceMappingsToDelete,
                        allUserHearingRolesOnHearing,
                        allCompanyHearingRolesOnHearing);
                }

                // Validate that no disallowed invitationSourceMappings are being deleted
                if (response.InvitationSourceMappingsWithoutIndividualDeletion.Any())
                {
                    throw new InvalidOperationException(
                        $"Cannot delete invitationSourceMappings. " +
                        $"The following ids are associated with invitationSources with CanDeleteIndividuals disabled " +
                        $"ids: {string.Join(", ", response.InvitationSourceMappingsWithoutIndividualDeletion)}");
                }

                // Perform delete operations. HashSets are used to avoid duplicated Ids to be sent to database query
                await _invitationSourceMappingDao.DeleteRangeAsync(response.InvitationSourceMappingIdsToRemove.ToHashSet().ToArray());
                await _userHearingRoleDao.DeleteRangeAsync(response.UserHearingRoleIdsToRemove.ToHashSet().ToArray());
                await _companyHearingRoleDao.DeleteRangeAsync(response.CompanyHearingRoleIdsToRemove.ToHashSet().ToArray());

                // Reload hearing to get updated metadata
                hearing = await GetHearingWithIncludes(request.HearingId, false);

                return new MetaDataResponse<Hearing, InvitationMetaData>
                {
                    ResponseData = hearing,
                    Meta = new InvitationMetaData
                    {
                        DeletedInvitees = response.UserHearingRoleIdsToRemove.Count + response.CompanyHearingRoleIdsToRemove.Count,
                    }
                };
            }

            private async Task<Hearing> GetHearingWithIncludes(int hearingId, bool includeInvitationSource)
            {
                var systemIncludes = new List<string>
                {
                    $"{nameof(Hearing.HearingStatus)}",
                    $"{nameof(Hearing.UserHearingRoles)}",
                    $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.HearingRole)}",
                    $"{nameof(Hearing.CompanyHearingRoles)}",
                    $"{nameof(Hearing.CompanyHearingRoles)}.{nameof(CompanyHearingRole.HearingRole)}",
                    $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.InvitationSourceMappings)}",
                    $"{nameof(Hearing.CompanyHearingRoles)}.{nameof(CompanyHearingRole.InvitationSourceMappings)}",
                };

                if (includeInvitationSource)
                {
                    systemIncludes.Add($"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.InvitationSourceMappings)}.{nameof(InvitationSourceMapping.InvitationSource)}");
                    systemIncludes.Add(
                        $"{nameof(Hearing.CompanyHearingRoles)}.{nameof(CompanyHearingRole.InvitationSourceMappings)}.{nameof(InvitationSourceMapping.InvitationSource)}");
                }

                var includes = IncludeProperties.Create<Hearing>(null, systemIncludes);
                return await _hearingDao.GetAsync(hearingId, includes);
            }
        }
    }
}