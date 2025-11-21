using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HearingStatus = Agora.Models.Enums.HearingStatus;
using JournalizedStatus = Agora.Models.Enums.JournalizedStatus;
using Agora.Operations.Common.Interfaces.Files;

namespace Agora.Operations.Common.Cleanup.Command
{
    public class GeneralCleanupCommand : IRequest
    {

        // A cleanup command intended to be called by the system only. Due to the lack of authorization, this should not be exposed in any controller.
        public class GeneralCleanupCommandHandler : IRequestHandler<GeneralCleanupCommand>
        {
            private readonly IHearingDao _hearingDao;
            private readonly ILogger<GeneralCleanupCommandHandler> _logger;
            private readonly INotificationQueueDao _notificationQueueDao;
            private readonly IFileService _fileService;
            private readonly IConsentDao _consentDao;
            private readonly ICommentDeclineInfoDao _commentDeclineInfoDao;


            public GeneralCleanupCommandHandler(IHearingDao hearingDao, ILogger<GeneralCleanupCommandHandler> logger, INotificationQueueDao notificationQueueDao, IFileService fileService, IConsentDao consentDao, ICommentDeclineInfoDao commentDeclineInfoDao)
            {
                _hearingDao = hearingDao;
                _logger = logger;
                _fileService = fileService;
                _notificationQueueDao = notificationQueueDao;
                _consentDao = consentDao;
                _commentDeclineInfoDao = commentDeclineInfoDao;
            }

            public async Task<Unit> Handle(GeneralCleanupCommand request, CancellationToken cancellationToken)
            {

                var includes = IncludeProperties.Create<Hearing>(null, new List<string>
                {
                    $"{nameof(Hearing.HearingStatus)}",
                    $"{nameof(Hearing.JournalizedStatus)}",
                    $"{nameof(Hearing.Notifications)}.{nameof(Notification.NotificationQueue)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.Consent)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.CommentDeclineInfo)}"
                });

                var allConcludedHearings = await _hearingDao.GetAllAsync(includes, hearing =>
                    hearing.HearingStatus.Status == HearingStatus.CONCLUDED);
                var concludedHearingsToBeDeleted = allConcludedHearings.Where(hearing => hearing.ConcludedDate != null && DateTime.Compare((DateTime)hearing.ConcludedDate, DateTime.Now.AddDays(-730)) < 0).ToList();

                _logger.LogInformation($"Total number of concluded hearings: {allConcludedHearings.Count}. Number of old hearings to be deleted {concludedHearingsToBeDeleted.Count}", allConcludedHearings.Count, concludedHearingsToBeDeleted.Count);

                foreach (Hearing hearing in concludedHearingsToBeDeleted)
                {
                    if (hearing.JournalizedStatus.Status != JournalizedStatus.JOURNALIZED)
                    {
                        _logger.LogError($"Deletion of an old hearing with hearingId \"{hearing.Id}\" and journalizedStatus \"{hearing.JournalizedStatus.Status}\" was stopped, since it has not yet been successfully journalized.", hearing.Id, hearing.JournalizedStatus.Status);
                    }
                    else
                    {

                        try
                        {
                            // Cascading delete ensures deleting of Contents, UserHearingRoles, CompanyHearingRoles, Comments, and Notifications attached to the hearing
                            await _hearingDao.DeleteAsync(hearing.Id);

                            // Manual cleaning of dangling entities
                            await RemoveDanglingEntities(hearing);
                            await DeleteDirectory(hearing);
                            _logger.LogInformation($"Successfully deleted hearing with HearingId:{hearing.Id}.", hearing.Id);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, $"An error occured while trying to delete hearing with HearingId:{hearing.Id}. Error message: {e.Message}", hearing.Id);
                        }
                    }

                }


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
