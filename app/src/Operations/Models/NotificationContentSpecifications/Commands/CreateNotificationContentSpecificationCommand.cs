using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NotificationTypeEnum = Agora.Models.Enums.NotificationType;

namespace Agora.Operations.Models.NotificationContentSpecifications.Commands
{
    public class CreateNotificationContentSpecificationCommand : IRequest<NotificationContentSpecification>
    {
        public int HearingId { get; set; }
        public NotificationTypeEnum NotificationTypeEnum { get; set; }

        public class CreateNotificationContentSpecificationCommandHandler : IRequestHandler<
            CreateNotificationContentSpecificationCommand, NotificationContentSpecification>
        {
            private readonly IHearingDao _hearingDao;
            private readonly INotificationContentDao _notificationContentDao;
            private readonly INotificationContentSpecificationDao _notificationContentSpecificationDao;
            private readonly INotificationTypeDao _notificationTypeDao;
            public CreateNotificationContentSpecificationCommandHandler(IHearingDao hearingDao, INotificationContentDao notificationContentDao, INotificationContentSpecificationDao notificationContentSpecificationDao, INotificationTypeDao notificationTypeDao)
            {
                _hearingDao = hearingDao;
                _notificationContentDao = notificationContentDao;
                _notificationContentSpecificationDao = notificationContentSpecificationDao;
                _notificationTypeDao = notificationTypeDao;
            }

            public async Task<NotificationContentSpecification> Handle(CreateNotificationContentSpecificationCommand request,
                CancellationToken cancellationToken)
            {
                var hearing = await GetHearingWithIncludes(request.HearingId);

                var notificationType = await GetNotificationTypeOfType(request.NotificationTypeEnum);

                var hearingHasNotificationContentSpecification =
                    hearing.NotificationContentSpecifications.Any(ncs => ncs.NotificationTypeId == notificationType.Id);

                if (hearingHasNotificationContentSpecification)
                {
                    return await UpdateExistingNotificationContentSpecification(hearing, notificationType);
                }

                var newNotificationContentSpecification = new NotificationContentSpecification
                {
                    NotificationTypeId = notificationType.Id,
                    HearingId = hearing.Id
                };

                await AddNotificationContent(newNotificationContentSpecification, notificationType);

                return await _notificationContentSpecificationDao.CreateAsync(newNotificationContentSpecification);

            }

            private async Task<NotificationContentSpecification> UpdateExistingNotificationContentSpecification(
                Hearing hearing, NotificationType notificationType)
            {
                var currentNotificationContentSpecification =
                        hearing.NotificationContentSpecifications.SingleOrDefault(ncs =>
                            ncs.NotificationTypeId == notificationType.Id) ?? new NotificationContentSpecification();


                await AddNotificationContent(currentNotificationContentSpecification, notificationType, true);

                if (currentNotificationContentSpecification.PropertiesUpdated == null || !currentNotificationContentSpecification.PropertiesUpdated.Any())
                {
                    return currentNotificationContentSpecification;
                }

                return await _notificationContentSpecificationDao.UpdateAsync(currentNotificationContentSpecification);
            }

            private async Task AddNotificationContent(NotificationContentSpecification notificationContentSpecification, NotificationType notificationType, bool isUpdate = false)
            {
                if (notificationContentSpecification.SubjectContentId == null)
                {
                    var subjectContent = await CreateNotificationContent(
                        notificationType.SubjectTemplate.NotificationContentTypeId,
                        notificationType.SubjectTemplate.TextContent);

                    notificationContentSpecification.SubjectContentId = subjectContent.Id;

                    if (isUpdate)
                    {
                        AddToPropertiesUpdated(notificationContentSpecification, nameof(notificationContentSpecification.SubjectContentId));
                    }
                }

                if (notificationContentSpecification.HeaderContentId == null)
                {
                    var headerContent = await CreateNotificationContent(
                        notificationType.HeaderTemplate.NotificationContentTypeId,
                        notificationType.HeaderTemplate.TextContent);

                    notificationContentSpecification.HeaderContentId = headerContent.Id;

                    if (isUpdate)
                    {
                        AddToPropertiesUpdated(notificationContentSpecification, nameof(notificationContentSpecification.HeaderContentId));
                    }
                }

                if (notificationContentSpecification.BodyContentId == null)
                {
                    var bodyContent = await CreateNotificationContent(
                        notificationType.BodyTemplate.NotificationContentTypeId,
                        notificationType.BodyTemplate.TextContent);

                    notificationContentSpecification.BodyContentId = bodyContent.Id;

                    if (isUpdate)
                    {
                        AddToPropertiesUpdated(notificationContentSpecification, nameof(notificationContentSpecification.BodyContentId));
                    }
                }

                if (notificationContentSpecification.FooterContentId == null)
                {
                    var footerContent = await CreateNotificationContent(
                        notificationType.FooterTemplate.NotificationContentTypeId,
                        notificationType.FooterTemplate.TextContent);

                    notificationContentSpecification.FooterContentId = footerContent.Id;

                    if (isUpdate)
                    {
                        AddToPropertiesUpdated(notificationContentSpecification, nameof(notificationContentSpecification.FooterContentId));
                    }
                }
            }

            private async Task<NotificationContent> CreateNotificationContent(int notificationContentTypeId,
                string textContent)
            {
                return await _notificationContentDao.CreateAsync(new NotificationContent
                {
                    NotificationContentTypeId = notificationContentTypeId,
                    TextContent = textContent
                });
            }

            private void AddToPropertiesUpdated(NotificationContentSpecification notificationContentSpecification,
                string propertyName)
            {
                if (notificationContentSpecification.PropertiesUpdated == null)
                {
                    notificationContentSpecification.PropertiesUpdated = new List<string> { propertyName };
                }
                else
                {
                    notificationContentSpecification.PropertiesUpdated.Add(propertyName);
                }
            }

            private async Task<Hearing> GetHearingWithIncludes(int hearingId)
            {
                var hearingIncludes = IncludeProperties.Create<Hearing>(null, new List<string>
                {
                    nameof(Hearing.NotificationContentSpecifications),
                    $"{nameof(Hearing.NotificationContentSpecifications)}.{nameof(NotificationContentSpecification.SubjectContent)}",
                    $"{nameof(Hearing.NotificationContentSpecifications)}.{nameof(NotificationContentSpecification.HeaderContent)}",
                    $"{nameof(Hearing.NotificationContentSpecifications)}.{nameof(NotificationContentSpecification.BodyContent)}",
                    $"{nameof(Hearing.NotificationContentSpecifications)}.{nameof(NotificationContentSpecification.FooterContent)}"
                });

                var hearing = await _hearingDao.GetAsync(hearingId, hearingIncludes);

                if (hearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), hearingId);
                }

                return hearing;
            }

            private async Task<NotificationType> GetNotificationTypeOfType(NotificationTypeEnum type)
            {
                var notificationTypeIncludes = IncludeProperties.Create<NotificationType>(null, new List<string>
                {
                    nameof(NotificationType.SubjectTemplate),
                    nameof(NotificationType.HeaderTemplate),
                    nameof(NotificationType.BodyTemplate),
                    nameof(NotificationType.FooterTemplate)
                });

                var allNotificationType = await _notificationTypeDao.GetAllAsync(notificationTypeIncludes);
                var notificationType = allNotificationType.FirstOrDefault(nt => nt.Type == type);
                if (notificationType == null)
                {
                    throw new NotFoundException(nameof(NotificationType), type);
                }

                return notificationType;
            }
        }

    }
}