using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Notifications;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NotificationType = Agora.Models.Enums.NotificationType;

namespace Agora.Operations.Models.Notifications.Queries.ExportNotification
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class ExportNotificationQuery : IRequest<FileDownload>
    {
        public int HearingId { get; set; }
        public NotificationType NotificationType { get; set; }

        public class ExportNotificationQueryHandler : IRequestHandler<ExportNotificationQuery, FileDownload>
        {
            private readonly IHearingDao _hearingDao;
            private readonly INotificationService _notificationService;

            public ExportNotificationQueryHandler(IHearingDao hearingDao, INotificationService notificationService)
            {
                _hearingDao = hearingDao;
                _notificationService = notificationService;
            }

            public async Task<FileDownload> Handle(ExportNotificationQuery request,
                CancellationToken cancellationToken)
            {
                var hearing = await GetHearingWithIncludes(request.HearingId);

                if (hearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.HearingId);
                }

                return await _notificationService.GenerateDownloadNotificationContentOfType(hearing, request.NotificationType);
            }

            private async Task<Hearing> GetHearingWithIncludes(int id)
            {
                var includes = IncludeProperties.Create<Hearing>(null, new List<string>
                {
                    nameof(Hearing.NotificationContentSpecifications),
                    $"{nameof(Hearing.NotificationContentSpecifications)}.{nameof(NotificationContentSpecification.NotificationType)}",
                    $"{nameof(Hearing.NotificationContentSpecifications)}.{nameof(NotificationContentSpecification.SubjectContent)}",
                    $"{nameof(Hearing.NotificationContentSpecifications)}.{nameof(NotificationContentSpecification.SubjectContent)}.{nameof(NotificationContent.NotificationContentType)}",
                    $"{nameof(Hearing.NotificationContentSpecifications)}.{nameof(NotificationContentSpecification.HeaderContent)}",
                    $"{nameof(Hearing.NotificationContentSpecifications)}.{nameof(NotificationContentSpecification.HeaderContent)}.{nameof(NotificationContent.NotificationContentType)}",
                    $"{nameof(Hearing.NotificationContentSpecifications)}.{nameof(NotificationContentSpecification.BodyContent)}",
                    $"{nameof(Hearing.NotificationContentSpecifications)}.{nameof(NotificationContentSpecification.BodyContent)}.{nameof(NotificationContent.NotificationContentType)}",
                    $"{nameof(Hearing.NotificationContentSpecifications)}.{nameof(NotificationContentSpecification.FooterContent)}",
                    $"{nameof(Hearing.NotificationContentSpecifications)}.{nameof(NotificationContentSpecification.FooterContent)}.{nameof(NotificationContent.NotificationContentType)}",
                    nameof(Hearing.Contents),
                    $"{nameof(Hearing.Contents)}.{nameof(Content.ContentType)}"

                });

                return await _hearingDao.GetAsync(id, includes);
            }
        }
    }
}