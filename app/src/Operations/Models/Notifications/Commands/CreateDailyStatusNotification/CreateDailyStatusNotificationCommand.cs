using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Extensions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Resolvers;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HearingRole = Agora.Models.Enums.HearingRole;
using NotificationType = Agora.Models.Enums.NotificationType;

namespace Agora.Operations.Models.Notifications.Commands.CreateDailyStatusNotification
{
    public class CreateDailyStatusNotificationCommand : IRequest<Unit>
    {
        public class CreateDailyStatusNotificationCommandHandler : IRequestHandler<CreateDailyStatusNotificationCommand, Unit>
        {
            private readonly IEventDao _eventDao;
            private readonly INotificationDao _notificationDao;
            private readonly IEventMappingDao _eventMappingDao;
            private readonly IHearingRoleResolver _hearingRoleResolver;
            private readonly ILogger<CreateDailyStatusNotificationCommand> _logger;

            public CreateDailyStatusNotificationCommandHandler(IEventDao eventDao, INotificationDao notificationDao, IHearingRoleResolver hearingRoleResolver, IEventMappingDao eventMappingDao, ILogger<CreateDailyStatusNotificationCommand> logger)
            {
                _eventDao = eventDao;
                _notificationDao = notificationDao;
                _hearingRoleResolver = hearingRoleResolver;
                _eventMappingDao = eventMappingDao;
                _logger = logger;
            }

            public async Task<Unit> Handle(CreateDailyStatusNotificationCommand request, CancellationToken cancellationToken)
            {
                // Note: We assume each Event have been created with a NotificationType
                // For each Event we identify the relevant Users affected by this Event
                // For each User a single Notification must be created, with one EventMapping per Event
                var unsentDailyStatusEvents = await _eventDao.GetAllAsync(filter: e => !e.IsSentInNotification && e.NotificationType.Type == NotificationType.DAILY_STATUS, includes: EventIncludeProperties);

                if (!unsentDailyStatusEvents.Any())
                {
                    return Unit.Value;
                }
                
                var dailyStatusNotificationType = unsentDailyStatusEvents.First().NotificationType;

                var eventsByUsers = await GroupEventsByRelevantUsers(unsentDailyStatusEvents);
                var eventMappingsToCreate = new List<EventMapping>();

                foreach (var (userId, events) in eventsByUsers)
                {
                    var notification = new Notification
                    {
                        NotificationTypeId = dailyStatusNotificationType.Id,
                        UserId = userId,
                        IsSentToQueue = false
                    };

                    var newNotification = await _notificationDao.CreateAsync(notification);

                    eventMappingsToCreate.AddRange(events.Select(singleEvent => new EventMapping
                    {
                        EventId = singleEvent.Id,
                        NotificationId = newNotification.Id
                    }));
                }

                await _eventMappingDao.CreateRangeAsync(eventMappingsToCreate);

                // Note: At this point some of the Events might not have been processed because of bad data.
                // Since we don't have a way to fix it, we will mark them all as sent.
                foreach (var processedEvent in unsentDailyStatusEvents)
                {
                    processedEvent.IsSentInNotification = true;
                    processedEvent.PropertiesUpdated = new List<string> { nameof(Event.IsSentInNotification) };
                    await _eventDao.UpdateAsync(processedEvent);
                }

                return Unit.Value;
            }

            private async Task<Dictionary<int, List<Event>>> GroupEventsByRelevantUsers(List<Event> events)
            {
                var eventsByUser = new Dictionary<int, List<Event>>();

                foreach (var eventEntity in events)
                {
                    var hearing = eventEntity.Hearing;
                    if (hearing == null)
                    {
                        _logger.LogWarning("Event with '{EventId}' have no connected Hearing. No notification will be created.", eventEntity.Id);
                        continue;
                    }

                    var hearingOwner = await hearing.GetHearingOwner(_hearingRoleResolver);
                    var reviewers = await hearing.GetUsersWithRole(_hearingRoleResolver, HearingRole.HEARING_REVIEWER);

                    if (hearingOwner == null)
                    {
                        _logger.LogWarning("Event with id: '{EventId}' and Hearing with id: '{HearingId}' does not have a hearingOwner. Notifications will still be created for other relevant users.", eventEntity.Id, hearing.Id);
                    }
                    else
                    {
                        AddEventToUser(eventsByUser, hearingOwner.Id, eventEntity);
                    }

                    foreach (var reviewer in reviewers)
                    {
                        AddEventToUser(eventsByUser, reviewer.Id, eventEntity);
                    }
                }

                return eventsByUser;
            }

            private static void AddEventToUser(Dictionary<int, List<Event>> eventsByUser, int userId, Event eventEntity)
            {
                if (!eventsByUser.ContainsKey(userId))
                {
                    eventsByUser[userId] = new List<Event>();
                }
                eventsByUser[userId].Add(eventEntity);
            }

            protected IncludeProperties EventIncludeProperties
            {
                get
                {
                    var eventIncludes = IncludeProperties.Create<Event>(null, new List<string>
                    {
                        nameof(Event.NotificationType),
                        nameof(Event.Hearing),
                        $"{nameof(Event.Hearing)}.{nameof(Hearing.UserHearingRoles)}",
                        $"{nameof(Event.Hearing)}.{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.User)}"
                    });
                    return eventIncludes;
                }
            }
        }
    }
}