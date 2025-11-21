using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NotificationType = Agora.Models.Enums.NotificationType;

namespace Agora.Operations.Models.Notifications.Queries.GetInvitationNotifications
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class GetInvitationNotificationsQuery : IRequest<List<Notification>>
    {
        public int HearingId { get; set; }

        public class GetInvitationNotificationsQueryHandler : IRequestHandler<GetInvitationNotificationsQuery, List<Notification>>
        {
            private readonly INotificationDao _notificationDao;

            public GetInvitationNotificationsQueryHandler(INotificationDao notificationDao)
            {
                _notificationDao = notificationDao;
            }

            public async Task<List<Notification>> Handle(GetInvitationNotificationsQuery request, CancellationToken cancellationToken)
            {
                var includes = new IncludeProperties(null, new List<string>
                {
                    nameof(Notification.Hearing),
                    nameof(Notification.User),
                    nameof(Notification.Company),
                    nameof(Notification.NotificationType),
                    nameof(Notification.NotificationQueue)
                });

                var allHearingNotifications = await _notificationDao.GetAllAsync(includes,
                    notification => notification.HearingId == request.HearingId);

                return allHearingNotifications.Where(notification =>
                    notification.NotificationType.Type == NotificationType.INVITED_TO_HEARING).ToList();
            }
        }
    }
}