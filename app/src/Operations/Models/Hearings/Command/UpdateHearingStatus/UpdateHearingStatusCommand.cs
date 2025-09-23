using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Operations.Common.Extensions;
using BallerupKommune.Operations.Common.Interfaces.Plugins;
using BallerupKommune.Operations.Resolvers;
using FieldType = BallerupKommune.Models.Enums.FieldType;
using HearingRole = BallerupKommune.Models.Enums.HearingRole;
using HearingStatus = BallerupKommune.Models.Enums.HearingStatus;

namespace BallerupKommune.Operations.Models.Hearings.Command.UpdateHearingStatus
{
    public class UpdateHearingStatusCommand : IRequest<Unit>
    {
        public class UpdateHearingStatusCommandHandler : IRequestHandler<UpdateHearingStatusCommand, Unit>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IHearingStatusDao _hearingStatusDao;
            private readonly IFieldSystemResolver _fieldSystemResolver;
            private readonly IHearingRoleResolver _hearingRoleResolver;
            private readonly IUserHearingRoleDao _userHearingRoleDao;
            private readonly ICompanyHearingRoleDao _companyHearingRoleDao;
            private readonly IPluginService _pluginService;

            public UpdateHearingStatusCommandHandler(IHearingDao hearingDao, IHearingStatusDao hearingStatusDao, 
                IFieldSystemResolver fieldSystemResolver, IPluginService pluginService, IHearingRoleResolver hearingRoleResolver,
                IUserHearingRoleDao userHearingRoleDao, ICompanyHearingRoleDao companyHearingRoleDao)
            {
                _hearingDao = hearingDao;
                _hearingStatusDao = hearingStatusDao;
                _fieldSystemResolver = fieldSystemResolver;
                _pluginService = pluginService;
                _hearingRoleResolver = hearingRoleResolver;
                _userHearingRoleDao = userHearingRoleDao;
                _companyHearingRoleDao = companyHearingRoleDao;
            }

            public async Task<Unit> Handle(UpdateHearingStatusCommand request, CancellationToken cancellationToken)
            {
                var defaultIncludes = IncludeProperties.Create<Hearing>(null, new List<string>
                {
                    $"{nameof(Hearing.Contents)}",
                    $"{nameof(Hearing.Contents)}.{nameof(Content.ContentType)}",
                });
                var allHearings = await _hearingDao.GetAllAsync(defaultIncludes, 
                    x => x.HearingStatus.Status == HearingStatus.AWAITING_STARTDATE || 
                    x.HearingStatus.Status == HearingStatus.ACTIVE ||
                    x.HearingStatus.Status == HearingStatus.AWAITING_CONCLUSION);

                var hearingStatus = await _hearingStatusDao.GetAllAsync();
                var activeHearingStatus = hearingStatus.Single(status => status.Status == HearingStatus.ACTIVE);
                var awaitingConclusionHearingStatus = hearingStatus.Single(status => status.Status == HearingStatus.AWAITING_CONCLUSION);

                foreach (var hearing in allHearings)
                {
                    await ProgressHearingStatus(hearing, activeHearingStatus, awaitingConclusionHearingStatus);
                }

                return Unit.Value;
            }

            private async Task ProgressHearingStatus(Hearing hearing, BallerupKommune.Models.Models.HearingStatus activeHearingStatus, BallerupKommune.Models.Models.HearingStatus awaitingConclusionHearingStatus)
            {
                var now = DateTime.Now;

                if (hearing.StartDate.HasValue && hearing.Deadline.HasValue)
                {
                    // Between start and deadline and haven't been changed to active = Should be active
                    if (hearing.StartDate.Value <= now.Date && now.Date <= hearing.Deadline.Value &&
                        hearing.HearingStatus.Status == HearingStatus.AWAITING_STARTDATE)
                    {
                        hearing.HearingStatusId = activeHearingStatus.Id;
                        hearing.HearingStatus = activeHearingStatus;
                        await _hearingDao.UpdateAsync(hearing);

                        await CreateInvitationNotifications(hearing.Id);
                    }

                    // Deadline is moved to the future and is awaiting conclusion = Should return to active status
                    if (hearing.Deadline.Value > now.Date && hearing.HearingStatus.Status == HearingStatus.AWAITING_CONCLUSION) {
                        hearing.HearingStatusId = activeHearingStatus.Id;
                        hearing.HearingStatus = activeHearingStatus;
                        await _hearingDao.UpdateAsync(hearing);
                    }

                    // Deadline passed status is active, and no conclusion is written = Should await conclusion
                    if (hearing.Deadline.Value < now.Date)
                    {
                        var conclusion =
                            (await hearing.GetContentsOfFieldType(_fieldSystemResolver, FieldType.CONCLUSION))
                            .SingleOrDefault();
                        if (conclusion == null && hearing.HearingStatus.Status == HearingStatus.ACTIVE)
                        {
                            hearing.HearingStatusId = awaitingConclusionHearingStatus.Id;
                            hearing.HearingStatus = awaitingConclusionHearingStatus;
                            await _hearingDao.UpdateAsync(hearing);
                        }
                    }
                }
            }

            private async Task CreateInvitationNotifications(int hearingId)
            {
                await CreateUserInvitationNotifications(hearingId);
                await CreateCompanyInvitationNotifications(hearingId);
            }

            private async Task CreateUserInvitationNotifications(int hearingId)
            {
                var inviteeHearingRole = await _hearingRoleResolver.GetHearingRole(HearingRole.HEARING_INVITEE);
                var userHearingRolesForHearing = await _userHearingRoleDao.GetUserHearingRolesForHearing(hearingId);

                var userIdsToNotify =
                    userHearingRolesForHearing.Where(uhr => uhr.HearingRoleId == inviteeHearingRole.Id).Select(uhr => uhr.UserId).ToHashSet().ToList();

                await _pluginService.NotifyUsersAfterInvitedToHearing(hearingId, userIdsToNotify);
            }

            private async Task CreateCompanyInvitationNotifications(int hearingId)
            {
                var inviteeHearingRole = await _hearingRoleResolver.GetHearingRole(HearingRole.HEARING_INVITEE);
                var companyHearingRolesForHearing =
                    await _companyHearingRoleDao.GetCompanyHearingRolesForHearing(hearingId);

                var companyIdsToNotify = companyHearingRolesForHearing
                    .Where(chr => chr.HearingRoleId == inviteeHearingRole.Id).Select(chr => chr.CompanyId).ToHashSet()
                    .ToList();

                await _pluginService.NotifyCompaniesAfterInvitedToHearing(hearingId, companyIdsToNotify);
            }
        }
    }
}