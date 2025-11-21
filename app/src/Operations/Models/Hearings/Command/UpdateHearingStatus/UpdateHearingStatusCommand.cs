using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Operations.Common.Extensions;
using Agora.Operations.Common.Interfaces.Plugins;
using Agora.Operations.Models.NotificationContentSpecifications.Commands;
using Agora.Operations.Resolvers;
using Microsoft.Extensions.Logging;
using FieldType = Agora.Models.Enums.FieldType;
using HearingStatus = Agora.Models.Enums.HearingStatus;
using HearingRole = Agora.Models.Enums.HearingRole;
using NotificationType = Agora.Models.Enums.NotificationType;

namespace Agora.Operations.Models.Hearings.Command.UpdateHearingStatus
{
    public class UpdateHearingStatusCommand : IRequest<Unit>
    {
        public class UpdateHearingStatusCommandHandler : IRequestHandler<UpdateHearingStatusCommand, Unit>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IHearingStatusDao _hearingStatusDao;
            private readonly IHearingRoleResolver _hearingRoleResolver;
            private readonly IUserHearingRoleDao _userHearingRoleDao;
            private readonly ICompanyHearingRoleDao _companyHearingRoleDao;
            private readonly ISender _mediator;
            private readonly IPluginService _pluginService;
            private readonly ILogger<UpdateHearingStatusCommandHandler> _logger;

            public UpdateHearingStatusCommandHandler(IHearingDao hearingDao, IHearingStatusDao hearingStatusDao,
                IPluginService pluginService, IHearingRoleResolver hearingRoleResolver, 
                IUserHearingRoleDao userHearingRoleDao, ICompanyHearingRoleDao companyHearingRoleDao, 
                ISender mediator, ILogger<UpdateHearingStatusCommandHandler> logger)
            {
                _hearingDao = hearingDao;
                _hearingStatusDao = hearingStatusDao;
                _pluginService = pluginService;
                _hearingRoleResolver = hearingRoleResolver;
                _userHearingRoleDao = userHearingRoleDao;
                _companyHearingRoleDao = companyHearingRoleDao;
                _mediator = mediator;
                _logger = logger;
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
                    try
                    {
                        await ProgressHearingStatus(hearing, activeHearingStatus, awaitingConclusionHearingStatus, cancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Failed to progress hearing status for hearing with id: {hearingId}. Message: {message}", hearing.Id, e.Message);
                    }
                }

                return Unit.Value;
            }

            private async Task ProgressHearingStatus(Hearing hearing, Agora.Models.Models.HearingStatus activeHearingStatus, Agora.Models.Models.HearingStatus awaitingConclusionHearingStatus, CancellationToken cancellationToken)
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

                    // Deadline passed and status is active = Should await conclusion
                    if (hearing.Deadline.Value < now.Date)
                    {
                        if (hearing.HearingStatus.Status == HearingStatus.ACTIVE)
                        {
                            await _mediator.Send(new CreateNotificationContentSpecificationCommand
                            {
                                HearingId = hearing.Id,
                                NotificationTypeEnum = NotificationType.HEARING_CONCLUSION_PUBLISHED
                            }, cancellationToken);

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