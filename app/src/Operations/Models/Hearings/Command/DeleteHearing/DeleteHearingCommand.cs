using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Interfaces.Plugins;
using HearingStatus = BallerupKommune.Models.Enums.HearingStatus;
using NovaSec.Attributes;

namespace BallerupKommune.Operations.Models.Hearings.Command.DeleteHearing
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.Id)")]
    public class DeleteHearingCommand : IRequest
    {
        public int Id { get; set; }

        public class DeleteHearingCommandHandler : IRequestHandler<DeleteHearingCommand>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IFileService _fileService;
            private readonly INotificationQueueDao _notificationQueueDao;
            private readonly IPluginService _pluginService;

            public DeleteHearingCommandHandler(IHearingDao hearingDao, IFileService fileService, INotificationQueueDao notificationQueueDao, IPluginService pluginService)
            {
                _hearingDao = hearingDao;
                _fileService = fileService;
                _notificationQueueDao = notificationQueueDao;
                _pluginService = pluginService;
            }

            public async Task<Unit> Handle(DeleteHearingCommand request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<Hearing>(null, new List<string> { nameof(Hearing.HearingStatus), $"{nameof(Hearing.Notifications)}.{nameof(Notification.NotificationQueue)}" });
                var currentHearing = await _hearingDao.GetAsync(request.Id, includes);

                if (currentHearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.Id);
                }

                if (currentHearing.HearingStatus.Status != HearingStatus.CREATED &&
                    currentHearing.HearingStatus.Status != HearingStatus.DRAFT)
                {
                    throw new Exception("Cannot delete Hearing when status is not DRAFT or CREATED");
                }

                await _pluginService.BeforeHearingDelete(currentHearing.Id, currentHearing.HearingStatus.Status);

                // Cascading delete ensures deleting of Contents, UserHearingRoles, Comments, and Notifications attatched to the hearing
                await _hearingDao.DeleteAsync(request.Id);

                // Manual cleaning of dangling entities
                await RemoveDanglingEntities(currentHearing);

                await _pluginService.AfterHearingDelete(currentHearing.Id);
                

                return Unit.Value;
            }

            private async Task RemoveDanglingEntities(Hearing hearing)
            {
                var notificationQueuesToDelete = hearing.Notifications
                    .Where(notification => notification.NotificationQueue != null)
                    .Select(notification => notification.NotificationQueue);

                foreach (var notificationQueue in notificationQueuesToDelete)
                {
                    await _notificationQueueDao.DeleteAsync(notificationQueue.Id);
                }

                _fileService.DeleteDirectory(hearing.Id);
            }
        }
    }
}