using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Extensions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Resolvers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventType = Agora.Models.Enums.EventType;
using HearingRole = Agora.Models.Enums.HearingRole;
using HearingStatus = Agora.Models.Enums.HearingStatus;
using NotificationContentType = Agora.Models.Enums.NotificationContentType;

namespace Agora.Operations.Services.Notifications.Forms
{
    public class DailyStatusForm : BaseNotificationForm
    {
        private readonly IEventMappingDao _eventMappingDao;

        private readonly IHearingRoleResolver _hearingRoleResolver;

        private static readonly Dictionary<EventType, int> EventOrder =
            new Dictionary<EventType, int>
            {
                {EventType.HEARING_OWNER_CHANGED, 1},
                {EventType.HEARING_STATUS_CHANGED, 2},
                {EventType.REVIEWER_ADDED, 3},
                {EventType.HEARING_RESPONSE_RECEIVED, 4},
                {EventType.HEARING_REVIEW_RECEIVED, 5}
            };

        public DailyStatusForm(IEventMappingDao eventMappingDao, IGlobalContentDao globalContentDao, INotificationContentSpecificationDao notificationContentSpecificationDao, INotificationTypeDao notificationTypeDao, 
            IFieldSystemResolver fieldSystemResolver, IHearingRoleResolver hearingRoleResolver, ITextResolver textResolver, 
            IOptions<AppOptions> options)
            : base(globalContentDao, notificationContentSpecificationDao, notificationTypeDao, fieldSystemResolver, textResolver, options)
        {
            _eventMappingDao = eventMappingDao;

            _hearingRoleResolver = hearingRoleResolver;
        }

        public override Task<NotificationContentResult> GetContentFromHearing(Hearing hearing)
        {
            throw new NotImplementedException();
        }

        public override async Task<NotificationContentResult> GetContentFromNotification(Notification notification)
        {
            var contents = GetNotificationContents(notification.NotificationType);

            var subject = GetContentByType(contents, NotificationContentType.SUBJECT);
            var footer = GetContentByType(contents, NotificationContentType.FOOTER);

            var dailyStatusContent = await BuildDailyStatusContent(notification);
            var notificationContent = dailyStatusContent + footer;
            var contentWithReplacedVariables = await ReplaceCommonVariablesAsync(notificationContent, notification.Hearing);
            var subjectWithReplacedVariables = await ReplaceCommonVariablesAsync(subject, notification.Hearing);

            return new NotificationContentResult { Content = ReplaceNewLineVariable(contentWithReplacedVariables), Subject = subjectWithReplacedVariables };
        }

        private async Task<string> BuildDailyStatusContent(Notification notification)
        {
            // Note: Notifications that end up here is originating from CreateDailyStatusNotificationCommand
            // We know that the notification always have a User, and that it will be a 'relevant' user for a given Hearing
            var contentStringBuilder = new StringBuilder();

            var eventMappings = await GetEventMappingsForNotification(notification.Id);
            var events = eventMappings.Select(em => em.Event).ToList();
            var groupedByHearing = events.GroupBy(e => e.Hearing.Id);

            contentStringBuilder.AppendLine($"<p>Hej {notification.User.Name}, </p>");
            contentStringBuilder.AppendLine("<p>Der har det seneste døgn været følgende aktiviteter, på høringer du deltager i. </p>");

            foreach (var hearingGroup in groupedByHearing)
            {
                var eventsOnHearingForUser = hearingGroup.ToList();
                var hearing = eventsOnHearingForUser.First().Hearing;

                var sortedEvents = eventsOnHearingForUser.OrderBy(e => EventOrder[e.Type]).ToList();

                var distinctAndSortedEvents = sortedEvents.GroupBy(e => e.Type);

                var hearingTitle = await GetHearingTitle(hearing);

                contentStringBuilder.AppendLine($"<p> <strong> {hearingTitle}: </strong></p>");
                contentStringBuilder.AppendLine("<ul>");

                foreach (var eventTypeGroup in distinctAndSortedEvents)
                {
                    var notificationContent = await GetNotificationContentToAdd(eventTypeGroup, hearing);
                    contentStringBuilder.AppendLine(notificationContent);
                }

                contentStringBuilder.AppendLine("</ul>");
            }

            return contentStringBuilder.ToString();
        }

        private async Task<string> GetNotificationContentToAdd(IGrouping<EventType, Event> eventsByType, Hearing hearing)
        {
            switch (eventsByType.Key)
            {
                case EventType.HEARING_OWNER_CHANGED:
                    return await BuildHearingOwnerChangedContent(hearing);
                case EventType.HEARING_STATUS_CHANGED:
                    return BuildHearingStatusChangedContent(hearing);
                case EventType.REVIEWER_ADDED:
                    return await BuildReviewerAddedContent(eventsByType, hearing);
                case EventType.HEARING_RESPONSE_RECEIVED:
                    return BuildHearingResponseReceivedContent(eventsByType);
                case EventType.HEARING_REVIEW_RECEIVED:
                    return BuildHearingReviewReceivedContent(eventsByType);
                default:
                    throw new ArgumentOutOfRangeException(nameof(EventType));
            }
        }

        private async Task<string> BuildHearingOwnerChangedContent(Hearing hearing)
        {
            // We can either find the hearing owner from the Hearing, or group events and find the latest event.
            var hearingOwner = await hearing.GetHearingOwner(_hearingRoleResolver);

            var text = $"<li>Høringsejer: Høringen har fået ny høringsejer: <strong>{hearingOwner}.</strong></li>";
            return text;
        }

        private static string BuildHearingStatusChangedContent(Hearing hearing)
        {
            // We can either find the hearing status from the Hearing, or group events and find the latest event.
            var hearingStatus = GetHearingStatusString(hearing.HearingStatus.Status);

            var text = $"<li>Høringsstatus: Høringen har fået ny status: <strong>{hearingStatus}.</strong></li>";
            return text;
        }

        private async Task<string> BuildReviewerAddedContent(IGrouping<EventType, Event> eventsByType, Hearing hearing)
        {
            var allReviewers = await hearing.GetUsersWithRole(_hearingRoleResolver, HearingRole.HEARING_REVIEWER);
            var reviewersFromEvents = eventsByType.Select(e => e.UserId);
            var relevantReviewers = allReviewers.Where(user => reviewersFromEvents.Contains(user.Id));

            var text = $"<li>Reviewer: Der er tilføjet følgende reviewere: <ul><li>{string.Join(", ", relevantReviewers.Select(user => user.Name))}</li></ul></li>";
            return text;
        }

        private static string BuildHearingResponseReceivedContent(IGrouping<EventType, Event> eventsByType)
        {
            var hearingResponseCount = eventsByType.Count();

            var text = $"<li>Høringssvar: Der er modtaget <strong>{hearingResponseCount}</strong> høringssvar.</li>";
            return text;
        }

        private static string BuildHearingReviewReceivedContent(IGrouping<EventType, Event> eventsByType)
        {
            var hearingReviewsCount = eventsByType.Count();

            var text = $"<li>Høringskommentarer: Der er modtaget <strong>{hearingReviewsCount}</strong> høringskommentarer.</li>";
            return text;
        }

        private static string GetHearingStatusString(HearingStatus hearingStatus)
        {
            switch (hearingStatus)
            {
                case HearingStatus.CREATED:
                    return "Oprettet";
                case HearingStatus.DRAFT:
                    return "Kladde";
                case HearingStatus.AWAITING_STARTDATE:
                    return "Afventer Startdato";
                case HearingStatus.ACTIVE:
                    return "Aktiv";
                case HearingStatus.AWAITING_CONCLUSION:
                    return "Afventer Konklusion";
                case HearingStatus.CONCLUDED:
                    return "Konkluderet";
                case HearingStatus.NONE:
                default:
                    throw new Exception("Cannot convert hearing status to string");
            }
        }

        private async Task<List<EventMapping>> GetEventMappingsForNotification(int notificationId)
        {
            var eventMappingIncludes = IncludeProperties.Create<EventMapping>(null, new List<string>
            {
                nameof(EventMapping.Event),
                $"{nameof(EventMapping.Event)}.{nameof(Event.NotificationType)}",
                $"{nameof(EventMapping.Event)}.{nameof(Event.Hearing)}",
                $"{nameof(EventMapping.Event)}.{nameof(Event.Hearing)}.{nameof(Hearing.HearingStatus)}",
                $"{nameof(EventMapping.Event)}.{nameof(Event.Hearing)}.{nameof(Hearing.Contents)}",
                $"{nameof(EventMapping.Event)}.{nameof(Event.Hearing)}.{nameof(Hearing.Contents)}.{nameof(Content.ContentType)}",
                $"{nameof(EventMapping.Event)}.{nameof(Event.Hearing)}.{nameof(Hearing.UserHearingRoles)}",
                $"{nameof(EventMapping.Event)}.{nameof(Event.Hearing)}.{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.User)}"
            });
            
            return await _eventMappingDao.GetAllAsync(eventMappingIncludes, em => em.NotificationId == notificationId);
        }
    }
}