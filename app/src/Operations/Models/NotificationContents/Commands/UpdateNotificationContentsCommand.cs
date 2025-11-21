using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvalidOperationException = Agora.Operations.Common.Exceptions.InvalidOperationException;
using NotificationContentType = Agora.Models.Enums.NotificationContentType;
using NotificationType = Agora.Models.Enums.NotificationType;

namespace Agora.Operations.Models.NotificationContents.Commands
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class UpdateNotificationContentsCommand : IRequest<List<NotificationContent>>
    {
        public int HearingId { get; set; }
        public int NotificationContentSpecificationId { get; set; }
        public List<NotificationContent> NotificationContents { get; set; }

        public class UpdateNotificationContentsCommandHandler : IRequestHandler<UpdateNotificationContentsCommand, List<NotificationContent>>
        {
            private readonly INotificationContentDao _notificationContentDao;
            private readonly INotificationContentSpecificationDao _notificationContentSpecificationDao;

            public UpdateNotificationContentsCommandHandler(INotificationContentDao notificationContentDao, INotificationContentSpecificationDao notificationContentSpecificationDao)
            {
                _notificationContentDao = notificationContentDao;
                _notificationContentSpecificationDao = notificationContentSpecificationDao;
            }

            public async Task<List<NotificationContent>> Handle(UpdateNotificationContentsCommand request, CancellationToken cancellationToken)
            {
                var notificationContentSpecification =
                    await GetNotificationContentSpecification(request.NotificationContentSpecificationId);

                ValidateHearing(notificationContentSpecification, request.HearingId);
                ValidateNotificationTypeAllowsUpdate(notificationContentSpecification);

                var notificationContentIds = request.NotificationContents.Select(nc => nc.Id).ToList();
                var notificationContentsFromDatabase = await GetNotificationContents(notificationContentIds);

                ValidateNotificationContentBelongsToSpecification(notificationContentsFromDatabase, notificationContentSpecification);
                ValidateNotificationContentTypeAllowsUpdate(notificationContentsFromDatabase);

                var updatedNotificationContents = new List<NotificationContent>();
                foreach (var notificationContent in request.NotificationContents)
                {
                    var updated = await _notificationContentDao.UpdateAsync(notificationContent);
                    updatedNotificationContents.Add(updated);
                }

                return updatedNotificationContents;
            }

            private async Task<NotificationContentSpecification> GetNotificationContentSpecification(int id)
            {
                var includes = IncludeProperties.Create<NotificationContentSpecification>(null, new List<string>
                {
                    nameof(NotificationContentSpecification.NotificationType),
                    nameof(NotificationContentSpecification.Hearing)
                });

                var notificationContentSpecification = await _notificationContentSpecificationDao.GetAsync(id, includes);

                if (notificationContentSpecification == null)
                {
                    throw new NotFoundException(nameof(NotificationContentSpecification), id);
                }

                return notificationContentSpecification;
            }

            private async Task<List<NotificationContent>> GetNotificationContents(List<int> ids)
            {
                var includes = IncludeProperties.Create<NotificationContent>(null, new List<string>
                {
                    nameof(NotificationContent.NotificationContentType),
                });

                var notificationContents =
                    await _notificationContentDao.GetAllAsync(includes, nc => ids.Contains(nc.Id));

                if (notificationContents.Count == 0)
                {
                    throw new NotFoundException(nameof(NotificationContent), string.Join(", ", ids));
                }

                return notificationContents;
            }

            private static void ValidateNotificationContentBelongsToSpecification(List<NotificationContent> notificationContents, NotificationContentSpecification notificationContentSpecification)
            {
                var errorIds = new List<int>();
                foreach (var notificationContent in notificationContents)
                {
                    var notificationContentIdFromSpecification =
                        GetContentIdFromNotificationContentSpecification(notificationContentSpecification,
                            notificationContent.NotificationContentType.Type);

                    if (notificationContentIdFromSpecification != notificationContent.Id)
                    {
                        errorIds.Add(notificationContent.Id);
                    }
                }

                if (errorIds.Count > 0)
                {
                    throw new InvalidOperationException($"NotificationContent with id '{string.Join(", ", errorIds)}' is not content of NotificationContentSpecification with Id ${notificationContentSpecification.Id}");
                }
            }

            private static void ValidateHearing(NotificationContentSpecification notificationContentSpecification, int hearingIdFromRequest)
            {
                var hearingId = notificationContentSpecification.HearingId;
                if (hearingId != hearingIdFromRequest)
                {
                    throw new InvalidOperationException($"Hearing with id: {hearingId} does not match URL parameter id: {hearingIdFromRequest}.");
                }
            }

            private static void ValidateNotificationContentTypeAllowsUpdate(List<NotificationContent> notificationContents)
            {
                foreach (var notificationContent in notificationContents)
                {
                    var notificationContentType = notificationContent.NotificationContentType;

                    if (notificationContentType == null)
                    {
                        throw new InvalidOperationException($"No NotificationContentType found with id: {notificationContent.NotificationContentTypeId}.");
                    }

                    if (!notificationContentType.CanEdit)
                    {
                        throw new InvalidOperationException($"NotificationContentType with type: {notificationContentType.Type} does not allow being edited.");
                    }
                }
            }

            private static void ValidateNotificationTypeAllowsUpdate(NotificationContentSpecification notificationContentSpecification)
            {
                var notificationType = notificationContentSpecification.NotificationType.Type;

                if (notificationType != NotificationType.INVITED_TO_HEARING && notificationType != NotificationType.HEARING_CONCLUSION_PUBLISHED)
                {
                    throw new InvalidOperationException($"Only possible to update NotificationContent for NotificationTypes: '{NotificationType.INVITED_TO_HEARING}' and '{NotificationType.HEARING_CONCLUSION_PUBLISHED}'.");
                }
            }

            private static int? GetContentIdFromNotificationContentSpecification(
                NotificationContentSpecification notificationContentSpecification, NotificationContentType type)
            {
                switch (type)
                {
                    case NotificationContentType.SUBJECT:
                        return notificationContentSpecification.SubjectContentId;
                    case NotificationContentType.HEADER:
                        return notificationContentSpecification.HeaderContentId;
                    case NotificationContentType.BODY:
                        return notificationContentSpecification.BodyContentId;
                    case NotificationContentType.FOOTER:
                        return notificationContentSpecification.FooterContentId;
                    case NotificationContentType.NONE:
                    default:
                        return null;
                }
            }
        }
    }
}