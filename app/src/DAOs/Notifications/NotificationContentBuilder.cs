using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BallerupKommune.Operations.Resolvers;
using FieldType = BallerupKommune.Models.Enums.FieldType;
using HearingRole = BallerupKommune.Models.Enums.HearingRole;
using CommentType = BallerupKommune.Models.Enums.CommentType;
using BallerupKommune.Models.Common;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using System;
using BallerupKommune.Models.Extensions;
using BallerupKommune.Operations.ApplicationOptions;
using Microsoft.Extensions.Options;
using NotificationType = BallerupKommune.Models.Enums.NotificationType;
using GlobalContentType = BallerupKommune.Models.Enums.GlobalContentType;

namespace BallerupKommune.DAOs.Notifications
{
    public class NotificationContentBuilder : INotificationContentBuilder
    {
        private readonly IFieldSystemResolver _fieldSystemResolver;
        private readonly IHearingRoleResolver _hearingRoleResolver;
        private readonly ICommentDao _commentDao;
        private readonly IUserDao _userDao;
        private readonly IGlobalContentDao _globalContentDao;
        private readonly IOptions<AppOptions> _options;

        private static readonly Dictionary<NotificationType, int> NotificationOrder =
            new Dictionary<NotificationType, int>
            {
                {NotificationType.CHANGED_HEARING_OWNER, 1},
                {NotificationType.CHANGED_HEARING_STATUS, 2},
                {NotificationType.ADDED_AS_REVIEWER, 3},
                {NotificationType.HEARING_RESPONSE_RECEIVED, 4},
                {NotificationType.HEARING_REVIEW_RECEIVED, 5},
                {NotificationType.INVITED_TO_HEARING, 6},
                {NotificationType.HEARING_CONCLUSION_PUBLISHED, 7},
                {NotificationType.HEARING_ANSWER_RECEIPT, 8},
                {NotificationType.HEARING_CHANGED, 9},
                {NotificationType.HEARING_RESPONSE_DECLINED, 10},
                {NotificationType.NONE, 0}
            };

        public NotificationContentBuilder(IFieldSystemResolver fieldSystemResolver, IHearingRoleResolver hearingRoleResolver, ICommentDao commentDao, IUserDao userDao, IGlobalContentDao globalContentDao, IOptions<AppOptions> options)
        {
            _fieldSystemResolver = fieldSystemResolver;
            _hearingRoleResolver = hearingRoleResolver;
            _commentDao = commentDao;
            _userDao = userDao;
            _globalContentDao = globalContentDao;
            _options = options;
        }

        public async Task<List<string>> BuildNotificationContent(Notification notification)
        {
            var notificationTemplate = notification.NotificationType.NotificationTemplate;

            var hearingTitle = await GetHearingTitle(notification);
            var termsAndConditions = await GetTermsAndConditionsText();

            var type = notification.NotificationType.Type;
            if (type != NotificationType.INVITED_TO_HEARING &&
                type != NotificationType.HEARING_ANSWER_RECEIPT &&
                type != NotificationType.HEARING_RESPONSE_DECLINED &&
                type != NotificationType.HEARING_CONCLUSION_PUBLISHED &&
                type != NotificationType.HEARING_CHANGED)
            {
                throw new ArgumentOutOfRangeException();
            }

            var notificationTemplateText = notificationTemplate?.NotificationTemplateText;
            notificationTemplateText = notificationTemplateText.Replace("{{HearingTitle}}", hearingTitle);
            notificationTemplateText = notificationTemplateText.Replace("{{LinkToHearing}}", GetLinkToHearing(notification));
            notificationTemplateText = notificationTemplateText.Replace("{{TermsAndConditions}}", termsAndConditions);

            if (notification.NotificationType.Type == NotificationType.HEARING_RESPONSE_DECLINED)
            {
                var declinedComment = notification?.Comment;
                
                if (declinedComment == null)
                {
                    throw new Exception($"Notification does not contain comment id for declined hearing response for hearing with id {notification.Hearing.Id}");
                }


                var declinedCommentInfo = declinedComment?.CommentDeclineInfo;
                if (declinedCommentInfo == null)
                {
                    throw new Exception($"Declined comment with Id {declinedComment.Id} does not have a CommentDeclineInfo - cannot create notification");
                }

                if (string.IsNullOrEmpty(declinedCommentInfo.DeclineReason))
                {
                    throw new Exception($"CommentDeclineInfo for declined comment with Id {declinedComment.Id} does not have a decline reason - cannot create notification");
                }

                var companyResponder = "";
                if (notification.Company != null)
                {
                    companyResponder = $", afgivet på vegne af jer af {declinedComment.User.Name}, ";
                }

                var commentDeclineReason = $"<ul><i>\"{declinedCommentInfo?.DeclineReason}\"</i></ul>";

                notificationTemplateText = notificationTemplateText.Replace("{{CommentNumber}}", $"#{declinedComment.Number}");
                notificationTemplateText = notificationTemplateText.Replace("{{CommentDeclinedReason}}", commentDeclineReason);
                notificationTemplateText = notificationTemplateText.Replace("{{CompanyResponder}}", companyResponder);
            }

            if (notification.NotificationType.Type == NotificationType.HEARING_ANSWER_RECEIPT)
            {
                var companyResponder = "";
                if (notification.Company != null)
                {
                    if (notification.UserId == null)
                    {
                        throw new Exception(
                            $"Notification for Company with id {notification.Company.Id} does not contain userId. Cannot send Hearing Answer Receipt.");
                    }

                    var commentResponder = await _userDao.GetAsync((int)notification.UserId);
                    companyResponder = $" blev afgivet på vegne af jer af {commentResponder.Name}, og ";
                }

                notificationTemplateText = notificationTemplateText.Replace("{{CompanyResponder}}", companyResponder);
            } 

            List<string> content = notificationTemplateText.Split("{{NewLine}}").ToList();

            return content;

        }

        public async Task<string> BuildStatusNotificationContent(List<Notification> notifications, User user)
        {
            var groupedByHearing = notifications.GroupBy(notification => notification.Hearing.Id);

            var hearingTitlesWhereThisUserIsAddedAsReviewer =
                notifications
                    .Where(notification => notification.NotificationType.Type == NotificationType.ADDED_AS_REVIEWER)
                    .Select(notification => notification.Hearing)
                    .Select(hearing => hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.TITLE).Result)
                    .Select(content => content.TextContent ?? string.Empty).ToList();

            var contentStringBuilder = new StringBuilder();

            contentStringBuilder.AppendLine($"<p>Hej {user.Name}, </p>");

            if (hearingTitlesWhereThisUserIsAddedAsReviewer.Any())
            {
                contentStringBuilder.AppendLine(
                    $"<p>Du er blevet tilføjet som reviewer til høringerne: {string.Join(", ", hearingTitlesWhereThisUserIsAddedAsReviewer)} som gør at du vil modtage en daglig opdatering, hvis der har været aktivitet på høringen. </p>");
            }

            contentStringBuilder.AppendLine(
                "<p>Der har det seneste døgn været følgende aktiviteter, på høringer du deltager i. </p>");

            foreach (var hearingGroup in groupedByHearing)
            {
                var notificationsOnHearingForUser = hearingGroup.ToList();
                var hearing = notificationsOnHearingForUser.First().Hearing;

                var sortedNotifications = notificationsOnHearingForUser
                    .OrderBy(x => NotificationOrder[x.NotificationType.Type]).ToList();

                var distinctAndSortedNotifications = sortedNotifications
                    .GroupBy(notification => notification.NotificationType.Type)
                    .Select(group => group.First()).ToList();

                string hearingTitle =
                    (await hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.TITLE))
                    ?.TextContent ?? string.Empty;

                contentStringBuilder.AppendLine($"<p> <strong> {hearingTitle}: </strong></p>");
                contentStringBuilder.AppendLine(@"<ul>");

                foreach (var notification in distinctAndSortedNotifications)
                {
                    var notificationContent = await GetNotificationContentToAdd(notification, hearing);
                    contentStringBuilder.AppendLine(notificationContent);

                }

                contentStringBuilder.AppendLine(@"</ul>");
            }

            contentStringBuilder.AppendLine(@"<p>Med Venlig Hilsen</p>");
            contentStringBuilder.AppendLine(@"<p>Ballerup Kommune</p>");

            return contentStringBuilder.ToString();
        }

        private async Task<string> GetNotificationContentToAdd(Notification notification, Hearing hearing)
        {
            var notificationTemplate = notification.NotificationType.NotificationTemplate;
            string content = string.Empty;
            if (notificationTemplate.NotificationTemplateText != null)
            {
                switch (notification.NotificationType.Type)
                {
                    case NotificationType.ADDED_AS_REVIEWER:
                        content = await BuildAddedAsReviewerContent(notificationTemplate, hearing);
                        break;
                    case NotificationType.CHANGED_HEARING_OWNER:
                        content = await BuildChangedHearingOwnerContent(notificationTemplate, hearing);
                        break;
                    case NotificationType.CHANGED_HEARING_STATUS:
                        content = BuildChangedHearingStatusContent(notificationTemplate, hearing);
                        break;
                    case NotificationType.HEARING_RESPONSE_RECEIVED:
                        content = await BuildHearingResponseContent(notificationTemplate, hearing);
                        break;
                    case NotificationType.HEARING_REVIEW_RECEIVED:
                        content = await BuildHearingReviewReceivedContent(notificationTemplate, hearing);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return content;
        }

        private async Task<string> BuildAddedAsReviewerContent(NotificationTemplate notificationTemplate, Hearing hearing)
        {
            List<User> hearingReviewers =
                await hearing.GetUsersWithRole(_hearingRoleResolver,
                    HearingRole.HEARING_REVIEWER);
            var replacedReviewers =
                notificationTemplate.NotificationTemplateText.Replace("{{Reviewers}}",
                    string.Join(", ", hearingReviewers.Select(user => user.Name).ToList()));

            return replacedReviewers;
        }

        private async Task<string> BuildChangedHearingOwnerContent(NotificationTemplate notificationTemplate, Hearing hearing)
        {
            string hearingOwner = (await hearing.GetHearingOwner(_hearingRoleResolver)).EmployeeDisplayName;
            var replacedHearingOwner =
                notificationTemplate.NotificationTemplateText.Replace(
                    "{{HearingOwner}}",
                    hearingOwner);
            return replacedHearingOwner;
        }

        private string BuildChangedHearingStatusContent(NotificationTemplate notificationTemplate,
            Hearing hearing)
        {
            var hearingStatus = hearing.HearingStatus.Name;
            var replacedHearingStatus =
                notificationTemplate.NotificationTemplateText.Replace(
                    "{{HearingStatus}}",
                    hearingStatus);
            return replacedHearingStatus;
        }

        private async Task<string> BuildHearingResponseContent(NotificationTemplate notificationTemplate, Hearing hearing)
        {
            int hearingResponseCount =
                await GetCommentCountOfType(hearing.Id, CommentType.HEARING_RESPONSE);
            var replacedHearingResponseCount =
                notificationTemplate.NotificationTemplateText.Replace(
                    "{{HearingResponseCount}}", hearingResponseCount.ToString());
            return replacedHearingResponseCount;
        }

        private async Task<string> BuildHearingReviewReceivedContent(NotificationTemplate notificationTemplate,
            Hearing hearing)
        {
            int hearingReviewsCount =
                await GetCommentCountOfType(hearing.Id, CommentType.HEARING_REVIEW);
            var replacedHearingReviewsCount =
                notificationTemplate.NotificationTemplateText.Replace(
                    "{{HearingReviewCount}}", hearingReviewsCount.ToString());
            return replacedHearingReviewsCount;
        }

        private async Task<int> GetCommentCountOfType(int hearingId, CommentType type)
        {
            var includes = IncludeProperties.Create<Comment>(null, new List<string> { nameof(Comment.CommentType) });
            List<Comment> comments = await _commentDao.GetAllAsync(includes, new List<int> { hearingId });
            return comments.Count(comment => comment.CommentType.Type == type);
        }

        public async Task<string> GetHearingTitle(Notification notification)
        {
            return (await notification.Hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.TITLE))
                ?.TextContent ?? string.Empty;
        }

        public async Task<string> GetTermsAndConditionsText()
        {
            // TODO: This could be optimized by knowing the exact comment that triggered this notification.
            // That way we could insert the correct text-content from the comment and get the correct consent text.
            var termsAndConditionsGlobalContent =
                await _globalContentDao.GetLatestVersionOfTypeAsync(GlobalContentType.TERMS_AND_CONDITIONS);
            return termsAndConditionsGlobalContent?.Content ?? string.Empty;
        }

        public string GetLinkToHearing(Notification notification)
        {
            return notification.Hearing.CreateLinkToHearing(_options.Value.PathToReadInternalHearing,
                _options.Value.PathToReadPublicHearing) ?? string.Empty;
        }
    }
}
