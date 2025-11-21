using Agora.Models.Common;
using Agora.Models.Enums;
using Agora.Models.Models;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Extensions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HearingRole = Agora.Models.Enums.HearingRole;
using NotificationType = Agora.Models.Models.NotificationType;
using NotificationTypeEnum = Agora.Models.Enums.NotificationType;
using UserCapacity = Agora.Models.Enums.UserCapacity;

namespace Agora.Operations.Plugins.Plugins
{
    public class NotificationPlugin : PluginBase
    {
        private readonly INotificationDao _notificationDao;
        private readonly INotificationTypeDao _notificationTypeDao;
        private readonly IHearingDao _hearingDao;
        private readonly IUserDao _userDao;
        private readonly IEventDao _eventDao;
        private readonly IHearingRoleResolver _hearingRoleResolver;
        private readonly ILogger<NotificationPlugin> _logger;

        public NotificationPlugin(IServiceProvider serviceProvider, PluginConfiguration pluginConfiguration) : base(
            serviceProvider, pluginConfiguration)
        {
            _notificationDao = serviceProvider.GetService<INotificationDao>();
            _notificationTypeDao = serviceProvider.GetService<INotificationTypeDao>();
            _hearingDao = serviceProvider.GetService<IHearingDao>();
            _userDao = serviceProvider.GetService<IUserDao>();
            _eventDao = serviceProvider.GetService<IEventDao>();
            _hearingRoleResolver = serviceProvider.GetRequiredService<IHearingRoleResolver>();
            _logger = serviceProvider.GetRequiredService<ILogger<NotificationPlugin>>();
        }

        public override async Task NotifyUsersAfterInvitedToHearing(int hearingId, List<int> userIds)
        {
            var invitedToHearingNotificationType = await GetNotificationType(NotificationTypeEnum.INVITED_TO_HEARING);
            await NotifyMultipleUsers(invitedToHearingNotificationType.Id, hearingId, userIds);
        }

        public override async Task NotifyCompaniesAfterInvitedToHearing(int hearingId, List<int> companyIds)
        {
            var invitedToHearingNotificationType = await GetNotificationType(NotificationTypeEnum.INVITED_TO_HEARING);
            await NotifyMultipleCompanies(invitedToHearingNotificationType.Id, hearingId, companyIds);
        }

        public override async Task NotifyAfterHearingResponse(int hearingId, int userId)
        {
            var hearing = await GetHearingWithUserHearingRoles(hearingId);

            var hearingOwner = await hearing.GetHearingOwner(_hearingRoleResolver);

            if (userId != hearingOwner.Id)
            {
                var currentUser = await GetUserById(userId);
                var hearingAnswerReceiptNotificationType = await GetNotificationType(NotificationTypeEnum.HEARING_ANSWER_RECEIPT);

                await NotifyUserOrCompany(hearingAnswerReceiptNotificationType, hearingId, currentUser);
            }

            await CreateEvents(EventType.HEARING_RESPONSE_RECEIVED, hearing.Id, userId);
        }

        public override async Task NotifyAfterHearingResponseDecline(int hearingId, int userId, int commentId)
        {
            var hearing = await GetHearingWithUserHearingRoles(hearingId);
            var hearingOwner = await hearing.GetHearingOwner(_hearingRoleResolver);

            var hearingResponseDeclinedNotificationType = await GetNotificationType(NotificationTypeEnum.HEARING_RESPONSE_DECLINED);

            if (userId != hearingOwner.Id)
            {
                var commentResponder = await GetUserById(userId);
                await NotifyUserOrCompany(hearingResponseDeclinedNotificationType, hearing.Id, commentResponder, commentId);
            }
        }

        public override async Task NotifyAfterConclusionPublished(int hearingId)
        {
            var hearing = await GetHearingWithCompanyAndUserHearingRoles(hearingId);

            var allUsersToNotify = (await hearing.GetUsersWithAnyOfTheRoles(_hearingRoleResolver,
                HearingRole.HEARING_RESPONDER, HearingRole.HEARING_INVITEE))
                .Where(user => user.UserCapacity.Capacity != UserCapacity.COMPANY)
                .Select(user => user.Id).ToHashSet().ToList();

            var allCompaniesToNotify = (await hearing.GetCompaniesWithAnyOfTheRoles(_hearingRoleResolver,
                HearingRole.HEARING_INVITEE, HearingRole.HEARING_RESPONDER))
                .Select(company => company.Id).ToHashSet().ToList();

            var conclusionPublishedNotificationType = await GetNotificationType(NotificationTypeEnum.HEARING_CONCLUSION_PUBLISHED);

            await NotifyMultipleUsers(conclusionPublishedNotificationType.Id, hearingId, allUsersToNotify);
            await NotifyMultipleCompanies(conclusionPublishedNotificationType.Id, hearingId, allCompaniesToNotify);
        }

        public override async Task NotifyAfterHearingChanged(int hearingId)
        {
            var hearing = await GetHearingWithCompanyAndUserHearingRoles(hearingId);

            var allUsersToNotify = (await hearing.GetUsersWithAnyOfTheRoles(_hearingRoleResolver,
                    HearingRole.HEARING_RESPONDER, HearingRole.HEARING_INVITEE))
                .Where(user => user.UserCapacity.Capacity != UserCapacity.COMPANY)
                .Select(user => user.Id).ToHashSet().ToList();

            var allCompaniesToNotify = (await hearing.GetCompaniesWithAnyOfTheRoles(_hearingRoleResolver,
                    HearingRole.HEARING_INVITEE, HearingRole.HEARING_RESPONDER))
                .Select(company => company.Id).ToHashSet().ToList();

            var hearingChangedNotificationType = await GetNotificationType(NotificationTypeEnum.HEARING_CHANGED);

            await NotifyMultipleUsers(hearingChangedNotificationType.Id, hearing.Id, allUsersToNotify);
            await NotifyMultipleCompanies(hearingChangedNotificationType.Id, hearingId, allCompaniesToNotify);
        }

        public override async Task NotifyAfterAddedAsReviewer(int hearingId, int userId)
        {
            var addedAsReviewerNotificationType = await GetNotificationType(NotificationTypeEnum.ADDED_AS_REVIEWER);
            await NotifyUser(addedAsReviewerNotificationType.Id, hearingId, userId);
            await CreateEvents(EventType.REVIEWER_ADDED, hearingId, userId);
        }

        public override async Task NotifyAfterChangeHearingOwner(int hearingId)
        {
            var hearing = await GetHearingWithUserHearingRoles(hearingId);
            var newHearingOwner = await hearing.GetHearingOwner(_hearingRoleResolver);
            await CreateEvents(EventType.HEARING_OWNER_CHANGED, hearingId, newHearingOwner.Id);
        }

        public override async Task NotifyAfterChangeHearingStatus(int hearingId)
        {
            await CreateEvents(EventType.HEARING_STATUS_CHANGED, hearingId);
        }

        public override async Task NotifyAfterHearingReview(int hearingId)
        {
            await CreateEvents(EventType.HEARING_REVIEW_RECEIVED, hearingId);
        }

        private async Task NotifyUser(int notificationTypeId, int hearingId, int userId, int commentId = -1)
        {
            var notification = commentId == -1
                ? CreateUserNotificationModel(notificationTypeId, hearingId, userId)
                : CreateUserNotificationModelWithComment(notificationTypeId, hearingId, userId, commentId);

            await _notificationDao.CreateAsync(notification);
        }

        private async Task NotifyMultipleUsers(int notificationTypeId, int hearingId, List<int> userIds)
        {
            var notifications = new List<Notification>();

            foreach (var userId in userIds)
            {
                var notification = CreateUserNotificationModel(notificationTypeId, hearingId, userId);
                notifications.Add(notification);
            }

            await _notificationDao.CreateRangeAsync(notifications);
        }

        private async Task NotifyCompanyWithUser(int notificationTypeId, int hearingId, int companyId, int userId, int commentId = -1)
        {
            var notification = commentId == -1
                ? CreateCompanyNotificationModel(notificationTypeId, hearingId, companyId, userId)
                : CreateCompanyNotificationModelWithComment(notificationTypeId, hearingId, companyId, commentId, userId);

            await _notificationDao.CreateAsync(notification);

        }

        private async Task NotifyMultipleCompanies(int notificationTypeId, int hearingId, List<int> companyIds)
        {
            var notifications = new List<Notification>();

            foreach (var companyId in companyIds)
            {
                var notification = CreateCompanyNotificationModel(notificationTypeId, hearingId, companyId);
                notifications.Add(notification);
            }

            await _notificationDao.CreateRangeAsync(notifications);
        }

        private async Task NotifyUserOrCompany(NotificationType notificationType, int hearingId, User user, int commentId = -1)
        {
            if (user.UserCapacity.Capacity == UserCapacity.COMPANY)
            {
                var companyId = user?.CompanyId;
                if (companyId == null)
                {
                    _logger.LogWarning("Cannot create notification '{NotificationTypeName}' for User with Id {UserId}. User has UserCapacity Company, but has no CompanyId.", notificationType.Name, user.Id);
                    return;
                }

                await NotifyCompanyWithUser(notificationType.Id, hearingId, (int)companyId, user.Id, commentId);
            }
            else
            {
                await NotifyUser(notificationType.Id, hearingId, user.Id, commentId);
            }
        }

        private async Task<NotificationType> GetNotificationType(NotificationTypeEnum type)
        {
            var allNotificationTypes = await _notificationTypeDao.GetAllAsync();
            var notificationType = allNotificationTypes.Single(x => x.Type == type);

            return notificationType;
        }

        private async Task<Hearing> GetHearingWithUserHearingRoles(int hearingId)
        {
            var hearingIncludes = IncludeProperties.Create<Hearing>(null, new List<string>
            {
                nameof(Hearing.UserHearingRoles),
                $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.HearingRole)}",
                $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.User)}"
            });

            var hearingWithIncludes = await _hearingDao.GetAsync(hearingId, hearingIncludes);
            return hearingWithIncludes;
        }

        private async Task<Hearing> GetHearingWithCompanyAndUserHearingRoles(int hearingId)
        {
            var hearingIncludes = IncludeProperties.Create<Hearing>(null, new List<string>
            {
                nameof(Hearing.UserHearingRoles),
                $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.HearingRole)}",
                $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.User)}",
                nameof(Hearing.CompanyHearingRoles),
                $"{nameof(Hearing.CompanyHearingRoles)}.{nameof(CompanyHearingRole.HearingRole)}",
                $"{nameof(Hearing.CompanyHearingRoles)}.{nameof(CompanyHearingRole.Company)}"
            });

            var hearingWithIncludes = await _hearingDao.GetAsync(hearingId, hearingIncludes);
            return hearingWithIncludes;
        }

        private async Task<User> GetUserById(int userId)
        {
            var includes = IncludeProperties.Create<User>(null, new List<string>
            {
                nameof(User.UserCapacity)
            });
            return await _userDao.GetAsync(userId, includes);
        }

        private Notification CreateCompanyNotificationModel(int notificationTypeId, int hearingId, int companyId, int? userId = null)
        {
            return new Notification
            {
                NotificationTypeId = notificationTypeId,
                HearingId = hearingId,
                CompanyId = companyId,
                UserId = userId,
                IsSentToQueue = false
            };
        }

        private Notification CreateCompanyNotificationModelWithComment(int notificationTypeId, int hearingId, int companyId, int commentId, int? userId = null)
        {
            return new Notification
            {
                NotificationTypeId = notificationTypeId,
                HearingId = hearingId,
                CompanyId = companyId,
                CommentId = commentId,
                UserId = userId,
                IsSentToQueue = false
            };
        }

        private Notification CreateUserNotificationModel(int notificationTypeId, int hearingId, int userId)
        {
            return new Notification
            {
                NotificationTypeId = notificationTypeId,
                HearingId = hearingId,
                UserId = userId,
                IsSentToQueue = false
            };
        }

        private Notification CreateUserNotificationModelWithComment(int notificationTypeId, int hearingId, int userId, int commentId)
        {
            return new Notification
            {
                NotificationTypeId = notificationTypeId,
                HearingId = hearingId,
                UserId = userId,
                CommentId = commentId,
                IsSentToQueue = false
            };
        }

        private async Task CreateEvents(EventType eventType, int hearingId, int? userId = null)
        {
            switch (eventType)
            {
                case EventType.HEARING_OWNER_CHANGED:
                case EventType.HEARING_STATUS_CHANGED:
                case EventType.REVIEWER_ADDED:
                case EventType.HEARING_RESPONSE_RECEIVED:
                case EventType.HEARING_REVIEW_RECEIVED:
                case EventType.HEARING_ACTIVATED:
                    await CreateDailyStatusEvent(eventType, hearingId, userId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }
        }

        private async Task CreateDailyStatusEvent(EventType eventType, int hearingId, int? userId = null)
        {
            var dailyNotificationType = await GetNotificationType(NotificationTypeEnum.DAILY_STATUS);

            await _eventDao.CreateAsync(new Event
            {
                Type = eventType,
                HearingId = hearingId,
                UserId = userId,
                NotificationTypeId = dailyNotificationType.Id,
                IsSentInNotification = false
            });
        }
    }
}