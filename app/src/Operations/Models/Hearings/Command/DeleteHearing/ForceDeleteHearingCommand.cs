using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Plugins;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Agora.Models.Common;
using Agora.Operations.Common.Interfaces.Files;
using System;
using Microsoft.Extensions.Logging;

namespace Agora.Operations.Models.Hearings.Command.DeleteHearing
{
    [PreAuthorize("HasRole('Administrator')")]
    public class ForceDeleteHearingCommand : IRequest
    {
        public int Id { get; set; }

        public class ForceDeleteHearingCommandHandler : IRequestHandler<ForceDeleteHearingCommand>
        {
            private readonly IHearingDao _hearingDao;
            private readonly ILogger<ForceDeleteHearingCommandHandler> _logger;
            private readonly IFileService _fileService;
            private readonly INotificationQueueDao _notificationQueueDao;
            private readonly IPluginService _pluginService;
            private readonly IConsentDao _consentDao;
            private readonly ICommentDeclineInfoDao _commentDeclineInfoDao;

            public ForceDeleteHearingCommandHandler(IHearingDao hearingDao, ILogger<ForceDeleteHearingCommandHandler> logger, IFileService fileService, INotificationQueueDao notificationQueueDao, IPluginService pluginService, IConsentDao consentDao, ICommentDeclineInfoDao commentDeclineInfoDao)
            {
                _hearingDao = hearingDao;
                _logger = logger;
                _fileService = fileService;
                _notificationQueueDao = notificationQueueDao;
                _pluginService = pluginService;
                _consentDao = consentDao;
                _commentDeclineInfoDao = commentDeclineInfoDao;
            }

            public async Task<Unit> Handle(ForceDeleteHearingCommand request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<Hearing>(null, new List<string>
                {
                    $"{nameof(Hearing.HearingStatus)}",
                    $"{nameof(Hearing.Notifications)}.{nameof(Notification.NotificationQueue)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.Consent)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.CommentDeclineInfo)}"
                });
                var currentHearing = await _hearingDao.GetAsync(request.Id, includes);

                if (currentHearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.Id);
                }

                await _pluginService.BeforeHearingDelete(currentHearing.Id, currentHearing.HearingStatus.Status);

                // Cascading delete ensures deleting of Contents, UserHearingRoles, CompanyHearingRoles, Comments, and Notifications attached to the hearing
                await _hearingDao.DeleteAsync(request.Id);

                // Manual cleaning of dangling entities
                await RemoveDanglingEntities(currentHearing);
                await DeleteDirectory(currentHearing);

                await _pluginService.AfterHearingDelete(currentHearing.Id);


                return Unit.Value;
            }

            // Removal of dangling entities. Identical behavior exists in GeneralCleanupCommand, DeleteHearingCommand and ForceDeleteHearingCommand.
            private async Task RemoveDanglingEntities(Hearing hearing)
            {

                var notificationQueuesToDelete = hearing.Notifications
                    .Where(notification => notification.NotificationQueue != null)
                    .Select(notification => notification.NotificationQueue);
                foreach (var notificationQueue in notificationQueuesToDelete)
                {
                    try
                    {
                        await _notificationQueueDao.DeleteAsync(notificationQueue.Id);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"An error occured while trying to delete the dangling notificationQueue with id: {notificationQueue.Id}  for the deletion of hearing: {hearing.Id}. Error message: {e.Message}", hearing.Id, notificationQueue.Id);
                    }
                }

                var consentsToDelete = hearing.Comments.Where(comment => comment.Consent != null)
                    .Select(comment => comment.Consent);

                foreach (var consent in consentsToDelete)
                {
                    try
                    {
                        await _consentDao.DeleteAsync(consent.Id);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"An error occured while trying to delete the dangling consent with id: {consent.Id} for the deletion of hearing: {hearing.Id}. Error message: {e.Message}", hearing.Id, consent.Id);
                    }

                }

                var commentDeclineInfosToDelete = hearing.Comments.Where(comment => comment.CommentDeclineInfo != null)
                    .Select(comment => comment.CommentDeclineInfo);

                foreach (var commentDeclineInfo in commentDeclineInfosToDelete)
                {
                    try
                    {
                        await _commentDeclineInfoDao.DeleteAsync(commentDeclineInfo.Id);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"An error occured while trying to delete the dangling commentDeclineInfo with id: {commentDeclineInfo.Id} for the deletion of hearing: {hearing.Id}. Error message: {e.Message}", hearing.Id, commentDeclineInfo.Id);
                    }
                }
            }

            private async Task DeleteDirectory(Hearing hearing)
            {
                try
                {
                    _fileService.DeleteDirectory(hearing.Id);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An error occured while trying to delete the dangling file directory for the hearing: {hearing.Id}. Error message: {e.Message}", hearing.Id);

                }
            }
        }

    }
}
