using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.Operations.Common.Interfaces.Plugins;
using BallerupKommune.Operations.Common.Interfaces;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using BallerupKommune.Models.Common;

namespace BallerupKommune.Operations.Models.Hearings.Command.DeleteHearing
{
    [PreAuthorize("HasRole('Administrator')")]
    public class ForceDeleteHearingCommand : IRequest
    {
        public int Id { get; set; }

        public class ForceDeleteHearingCommandHandler : IRequestHandler<ForceDeleteHearingCommand>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IFileService _fileService;
            private readonly INotificationQueueDao _notificationQueueDao;
            private readonly IPluginService _pluginService;

            public ForceDeleteHearingCommandHandler(IHearingDao hearingDao, IFileService fileService, INotificationQueueDao notificationQueueDao, IPluginService pluginService)
            {
                _hearingDao = hearingDao;
                _fileService = fileService;
                _notificationQueueDao = notificationQueueDao;
                _pluginService = pluginService;
            }

            public async Task<Unit> Handle(ForceDeleteHearingCommand request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<Hearing>(null, new List<string> { nameof(Hearing.HearingStatus), $"{nameof(Hearing.Notifications)}.{nameof(Notification.NotificationQueue)}" });
                var currentHearing = await _hearingDao.GetAsync(request.Id, includes);

                if (currentHearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.Id);
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
                var notificationQueuesIdsToDelete = hearing.Notifications
                    .Where(notification => notification.NotificationQueue != null)
                    .Select(notification => notification.NotificationQueue.Id).Distinct();

                foreach (var notificationQueueId in notificationQueuesIdsToDelete)
                {
                    await _notificationQueueDao.DeleteAsync(notificationQueueId);
                }

                _fileService.DeleteDirectory(hearing.Id);
            }
        }

    }
}
