using BallerupKommune.Models.Models;
using BallerupKommune.Operations.ApplicationOptions;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Operations.Common.Extensions;
using BallerupKommune.Operations.Resolvers;
using Microsoft.Extensions.Logging;
using HearingRole = BallerupKommune.Models.Enums.HearingRole;
using NotificationTypeEnum = BallerupKommune.Models.Enums.NotificationType;
using UserCapacity = BallerupKommune.Models.Enums.UserCapacity;

namespace BallerupKommune.Operations.Plugins.Plugins
{
    public class NotificationPlugin : PluginBase
    {
        private readonly INotificationDao _notificationDao;
        private readonly INotificationTypeDao _notificationTypeDao;
        private readonly IHearingDao _hearingDao;
        private readonly IUserDao _userDao;
        private readonly IHearingRoleResolver _hearingRoleResolver;
        private readonly ILogger<NotificationPlugin> _logger;

        public NotificationPlugin(IServiceProvider serviceProvider, PluginConfiguration pluginConfiguration) : base(
            serviceProvider, pluginConfiguration)
        {
            _notificationDao = serviceProvider.GetService<INotificationDao>();
            _notificationTypeDao = serviceProvider.GetService<INotificationTypeDao>();
            _hearingDao = serviceProvider.GetService<IHearingDao>();
            _userDao = serviceProvider.GetService<IUserDao>();
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

            User hearingOwner = await hearing.GetHearingOwner(_hearingRoleResolver);

            if (userId != hearingOwner.Id)
            {
                var currentUser = await GetUserById(userId);
                var hearingAnswerReceiptNotificationType =
                    await GetNotificationType(NotificationTypeEnum.HEARING_ANSWER_RECEIPT);

                await NotifyUserOrCompany(hearingAnswerReceiptNotificationType, hearingId, currentUser);
            }

            var hearingResponseReceivedNotificationType =
                await GetNotificationType(NotificationTypeEnum.HEARING_RESPONSE_RECEIVED);

            await NotifyHearingOwnerAndReviewers(hearingResponseReceivedNotificationType.Id, hearing);

        }

        public override async Task NotifyAfterHearingResponseDecline(int hearingId, int userId, int commentId)
        {
            var hearing = await GetHearingWithUserHearingRoles(hearingId);
            User hearingOwner = await hearing.GetHearingOwner(_hearingRoleResolver);

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
        }

        public override async Task NotifyAfterChangeHearingOwner(int hearingId)
        {
            var hearing = await GetHearingWithUserHearingRoles(hearingId);
            var changedHearingOwnerNotificationType =
                await GetNotificationType(NotificationTypeEnum.CHANGED_HEARING_OWNER);

            await NotifyHearingOwnerAndReviewers(changedHearingOwnerNotificationType.Id, hearing);
        }

        public override async Task NotifyAfterChangeHearingStatus(int hearingId)
        {
            var hearing = await GetHearingWithUserHearingRoles(hearingId);
            var changedHearingStatusNotificationType =
                await GetNotificationType(NotificationTypeEnum.CHANGED_HEARING_STATUS);

            await NotifyHearingOwnerAndReviewers(changedHearingStatusNotificationType.Id, hearing);
        }

        public override async Task NotifyAfterHearingReview(int hearingId)
        {
            var hearing = await GetHearingWithUserHearingRoles(hearingId);
            var hearingReviewReceivedNotificationType =
                await GetNotificationType(NotificationTypeEnum.HEARING_REVIEW_RECEIVED);

            await NotifyHearingOwnerAndReviewers(hearingReviewReceivedNotificationType.Id, hearing);
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
            List<Notification> notifications = new List<Notification>();

            foreach (int userId in userIds)
            {
                var notification = CreateUserNotificationModel(notificationTypeId, hearingId, userId);
                notifications.Add(notification);
            }

            await _notificationDao.CreateRangeAsync(notifications);
        }

        private async Task NotifyCompany(int notificationTypeId, int hearingId, int companyId, int commentId = -1)
        {
            var notification = commentId == -1 
                ? CreateCompanyNotificationModel(notificationTypeId, hearingId, companyId) 
                : CreateCompanyNotificationModelWithComment(notificationTypeId, hearingId, companyId, commentId);

            await _notificationDao.CreateAsync(notification);
        }

        private async Task NotifyCompanyWithUser(int notificationTypeId, int hearingId, int companyId, int userId,
            int commentId = -1)
        {
            var notification = commentId == -1
                ? CreateCompanyNotificationModel(notificationTypeId, hearingId, companyId, userId)
                : CreateCompanyNotificationModelWithComment(notificationTypeId, hearingId, companyId, commentId, userId);

            await _notificationDao.CreateAsync(notification);

        }

        private async Task NotifyMultipleCompanies(int notificationTypeId, int hearingId, List<int> companyIds)
        {
            List<Notification> notifications = new List<Notification>();

            foreach (int companyId in companyIds)
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
                    _logger.LogWarning($"Cannot create notification '{notificationType.Name}' for User with Id {user.Id}. User has UserCapacity Company, but has no CompanyId.");
                    return;
                }

                await NotifyCompanyWithUser(notificationType.Id, hearingId, (int)companyId, user.Id, commentId);
            }
            else
            {
                await NotifyUser(notificationType.Id, hearingId, user.Id, commentId);
            }
        }

        private async Task NotifyHearingOwnerAndReviewers(int notificationTypeId, Hearing hearing)
        {
            User hearingOwner = await hearing.GetHearingOwner(_hearingRoleResolver);
            List<User> hearingReviewers = await hearing.GetUsersWithRole(_hearingRoleResolver, HearingRole.HEARING_REVIEWER);
            List<int> reviewerIds = hearingReviewers.Select(reviewer => reviewer.Id).ToList();

            await NotifyUser(notificationTypeId, hearing.Id, hearingOwner.Id);
            await NotifyMultipleUsers(notificationTypeId, hearing.Id, reviewerIds);
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
                UserId = userId
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
                UserId = userId
            };
        }

        private Notification CreateUserNotificationModel(int notificationTypeId, int hearingId, int userId)
        {
            return new Notification
            {
                NotificationTypeId = notificationTypeId,
                HearingId = hearingId,
                UserId = userId
            };
        }

        private Notification CreateUserNotificationModelWithComment(int notificationTypeId, int hearingId, int userId, int commentId)
        {
            return new Notification
            {
                NotificationTypeId = notificationTypeId,
                HearingId = hearingId,
                UserId = userId,
                CommentId = commentId
            };
        }
    }
}