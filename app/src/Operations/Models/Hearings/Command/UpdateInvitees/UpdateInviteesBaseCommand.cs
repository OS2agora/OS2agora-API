using Agora.Models.Common;
using Agora.Models.Common.CustomResponse;
using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Services;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HearingStatus = Agora.Models.Enums.HearingStatus;

namespace Agora.Operations.Models.Hearings.Command.UpdateInvitees
{
    public abstract class UpdateInviteesBaseCommand : IRequest<MetaDataResponse<Hearing, InvitationMetaData>>
    {
        public int HearingId { get; set; }
        public int InvitationSourceId { get; set; }

        public abstract class UpdateInviteesBaseCommandHandler<TRequest> : IRequestHandler<TRequest, MetaDataResponse<Hearing, InvitationMetaData>>
            where TRequest : UpdateInviteesBaseCommand
        {
            protected readonly IHearingDao _hearingDao;
            private readonly IInvitationHandler _invitationHandler;

            protected UpdateInviteesBaseCommandHandler(IHearingDao hearingDao, IInvitationHandler invitationHandler)
            {
                _hearingDao = hearingDao;
                _invitationHandler = invitationHandler;
            }

            protected abstract Task<(List<InviteeIdentifiers> identifiers, InvitationSource invitationSource, string sourceName)> GetInvitationData(TRequest request);

            protected abstract void NormalizeAndValidateInviteeIdentifiers(List<InviteeIdentifiers> inviteeIdentifiers);

            public async Task<MetaDataResponse<Hearing, InvitationMetaData>> Handle(TRequest request, CancellationToken cancellationToken)
            {
                var hearing = await GetHearingWithIncludes(request.HearingId,
                    new List<string> { nameof(Hearing.HearingStatus) });
                var currentStatus = hearing.HearingStatus.Status;

                if (currentStatus == HearingStatus.AWAITING_CONCLUSION || currentStatus == HearingStatus.CONCLUDED)
                {
                    throw new InvalidOperationException(
                        $"Cannot add invitees to a hearing with status {HearingStatus.AWAITING_CONCLUSION} or {HearingStatus.CONCLUDED}");
                }

                var (uploadedInviteeIdentifiers, invitationSource, sourceName) = await GetInvitationData(request);

                NormalizeAndValidateInviteeIdentifiers(uploadedInviteeIdentifiers);

                InvitationMetaData metaData;
                if (invitationSource.CanDeleteIndividuals)
                {
                    metaData = await _invitationHandler.CreateInviteesForHearing(hearing, invitationSource.Id, sourceName, uploadedInviteeIdentifiers);
                }
                else
                {
                    metaData = await _invitationHandler.ReplaceInviteesForHearing(hearing, invitationSource.Id, sourceName, uploadedInviteeIdentifiers);
                }

                hearing = await GetHearingWithIncludes(request.HearingId);

                return new MetaDataResponse<Hearing, InvitationMetaData>
                {
                    ResponseData = hearing,
                    Meta = metaData
                };
            }

            private async Task<Hearing> GetHearingWithIncludes(int hearingId, List<string> includes = null)
            {
                var systemIncludes = new List<string>
                {
                    nameof(Hearing.UserHearingRoles),
                    $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.User)}",
                    $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.HearingRole)}",
                    $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.InvitationSourceMappings)}",
                    nameof(Hearing.CompanyHearingRoles),
                    $"{nameof(Hearing.CompanyHearingRoles)}.{nameof(CompanyHearingRole.Company)}",
                    $"{nameof(Hearing.CompanyHearingRoles)}.{nameof(CompanyHearingRole.HearingRole)}",
                    $"{nameof(Hearing.CompanyHearingRoles)}.{nameof(CompanyHearingRole.InvitationSourceMappings)}"
                };

                if (includes != null)
                {
                    systemIncludes.AddRange(includes);
                }
                
                var hearingIncludes = IncludeProperties.Create<Hearing>(null, systemIncludes);

                return await _hearingDao.GetAsync(hearingId, hearingIncludes);
            }
        }
    }
}