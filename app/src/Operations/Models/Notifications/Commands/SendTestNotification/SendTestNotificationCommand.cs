using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Notifications;
using Markdig;
using MediatR;
using NovaSec.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InvalidOperationException = Agora.Operations.Common.Exceptions.InvalidOperationException;
using NotificationType = Agora.Models.Enums.NotificationType;

namespace Agora.Operations.Models.Notifications.Commands.SendTestNotification
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class SendTestNotificationCommand : IRequest<Unit>
    {
        public int HearingId { get; set; }
        public NotificationType NotificationType { get; set; }

        public class SendTestNotificationCommandHandler : IRequestHandler<SendTestNotificationCommand, Unit>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IUserDao _userDao;
            private readonly ICurrentUserService _currentUserService;
            private readonly IEmailService _emailService;
            private readonly INotificationService _notificationService;

            public SendTestNotificationCommandHandler(IHearingDao hearingDao, IUserDao userDao, ICurrentUserService currentUserService, IEmailService emailService, INotificationService notificationService)
            {
                _hearingDao = hearingDao;
                _userDao = userDao;
                _currentUserService = currentUserService;
                _emailService = emailService;
                _notificationService = notificationService;
            }

            public async Task<Unit> Handle(SendTestNotificationCommand request, CancellationToken cancellationToken)
            {
                var hearing = await GetHearingWithIncludes(request.HearingId);

                if (hearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.HearingId);
                }

                var currentUserDataBaseId = _currentUserService.DatabaseUserId;

                if (!currentUserDataBaseId.HasValue)
                {
                    throw new InvalidOperationException(
                        $"Cannot send test notification. Current user does not have a database id.");
                }
                var currentUser = await _userDao.GetAsync(currentUserDataBaseId.Value);
                if (currentUser == null)
                {
                    throw new NotFoundException(nameof(User), currentUserDataBaseId.Value);
                }
                if (string.IsNullOrEmpty(currentUser.Email))
                {
                    throw new InvalidOperationException(
                        $"Cannot send test notification to user with id '{currentUserDataBaseId}'. User does not have an email");
                }

                var notificationContentResult = 
                    await _notificationService.GenerateEmailNotificationContentOfType(hearing, request.NotificationType);
                var recipient = currentUser.Email;
                var emailContent = CreateEmailContent(notificationContentResult.Content);

                var response = await _emailService.SendMessage(notificationContentResult.Subject, emailContent, recipient);

                if (!response.IsSent)
                {
                    throw new Exception($"Failed to send test notification of type {request.NotificationType} to user with id {currentUserDataBaseId}");
                }

                return Unit.Value;
            }

            private string CreateEmailContent(List<string> content)
            {
                var contentStringBuilder = new StringBuilder();

                foreach (var singleContent in content)
                {
                    contentStringBuilder.AppendLine(Markdown.ToHtml(singleContent));
                }

                return contentStringBuilder.ToString();
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