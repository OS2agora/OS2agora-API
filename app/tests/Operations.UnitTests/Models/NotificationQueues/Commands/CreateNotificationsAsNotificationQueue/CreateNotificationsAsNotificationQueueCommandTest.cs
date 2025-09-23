using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.Operations.Models.NotificationQueues.Commands.CreateNotificationsAsNotificationQueue;
using BallerupKommune.Operations.UnitTests.Models;
using MediatR;
using Moq;
using NUnit.Framework;
using HearingStatusEnum = BallerupKommune.Models.Enums.HearingStatus;
using ContentTypeEnum = BallerupKommune.Models.Enums.ContentType;
using NotificationTypeEnum = BallerupKommune.Models.Enums.NotificationType;
using UserCapacityEnum = BallerupKommune.Models.Enums.UserCapacity;
using NotificationMessageChannelEnum = BallerupKommune.Models.Enums.NotificationMessageChannel;
using Microsoft.Extensions.Logging;
using BallerupKommune.Operations.Common.Interfaces;
using System.Text;
using Markdig;
using static BallerupKommune.Operations.Models.NotificationQueues.Commands.CreateNotificationsAsNotificationQueue.CreateNotificationsAsNotificationQueueCommand;

namespace BallerupKommune.Operations.Models.NotificationQueues.Commands
{
    public class CreateNotificationsAsNotificationQueueCommandTest : ModelsTestBase<CreateNotificationsAsNotificationQueueCommand, Unit>
    {
        private Mock<INotificationDao> _notificationDaoMock;
        private Mock<INotificationQueueDao> _notificationQueueDaoMock;
        private Mock<IPdfService> _pdfServiceMock;
        private Mock<ILogger<CreateNotificationsAsNotificationQueueCommandHandler>> _loggerMock;
        private Mock<INotificationContentBuilder> _notificationContentBuilderMock;
        private CreateNotificationsAsNotificationQueueCommandHandler createNotificationsAsNotificationQueueCommand;

        [SetUp]
        public void SetUp()
        {
            _notificationDaoMock = new Mock<INotificationDao>();
            _notificationQueueDaoMock = new Mock<INotificationQueueDao>();
            _pdfServiceMock = new Mock<IPdfService>();
            _loggerMock = new Mock<ILogger<CreateNotificationsAsNotificationQueueCommandHandler>>();
            _notificationContentBuilderMock = new Mock<INotificationContentBuilder>();

            createNotificationsAsNotificationQueueCommand = new CreateNotificationsAsNotificationQueueCommandHandler(_notificationDaoMock.Object, _notificationQueueDaoMock.Object, _pdfServiceMock.Object, _loggerMock.Object, _notificationContentBuilderMock.Object);

            _notificationQueueDaoMock.Setup(x => x.CreateAsync(It.IsAny<NotificationQueue>(), null)).ReturnsAsync((NotificationQueue x, IncludeProperties y) => { return x; });
        }

        [Test]
        public async Task Handle_Frequency_Instant([Values] HearingStatusEnum hearingStatus)
        {
            // Arrange

            // Mock text content for notification
            string templateText = "This is some random text{{NewLine}}{{NewLine}}{{HearingTitle}}{{NewLine}}{{LinkToHearing}}{{NewLine}}{{TermsAndConditions}}";
            List<string> lines = new List<string>() { "This is some random text", "", "Title", $"www.{hearingStatus}Url.dk/hearing/0", "Terms and Conditions" };
            var stringBuilder = new StringBuilder();
            foreach (string line in lines)
            {
                stringBuilder.AppendLine(Markdown.ToHtml(line));
            }
            var replacedText = stringBuilder.ToString();
            string pdfText = Convert.ToBase64String(Encoding.UTF8.GetBytes(replacedText));

            Notification updatedNotification = null;

            // Mock interfaces
            _notificationDaoMock.Setup(x => x.GetAllAsync(It.IsAny<IncludeProperties>(), It.IsAny<Expression<Func<Notification, bool>>>())).ReturnsAsync(GetNotificationList(templateText, hearingStatus));
            _notificationDaoMock.Setup(x => x.UpdateAsync(It.IsAny<Notification>(), null)).Callback((Notification x, IncludeProperties y) => { updatedNotification = x; });
            _pdfServiceMock.Setup(x => x.CreateTextPdf(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Encoding.UTF8.GetBytes(replacedText));
            _notificationContentBuilderMock.Setup(x => x.BuildNotificationContent(It.IsAny<Notification>())).Returns(Task.FromResult(new List<string> { replacedText }));

            // Act
            var request = new CreateNotificationsAsNotificationQueueCommand()
            {
                Frequency = BallerupKommune.Models.Enums.NotificationFrequency.INSTANT
            };

            await createNotificationsAsNotificationQueueCommand.Handle(request, CancellationToken.None);

            var hearingIsActive =
                hearingStatus != HearingStatusEnum.CREATED && hearingStatus != HearingStatusEnum.DRAFT;

            // Assert
            if (!hearingIsActive)
            {
                // Notifications fail if hearings are internal and user doesn't have an email
                _notificationDaoMock.Verify(x => x.UpdateAsync(updatedNotification, null), Times.Never());
            } 
            else
            {
                _notificationDaoMock.Verify(x => x.UpdateAsync(updatedNotification, null), Times.Once());
            }
            
        }

        [Test]
        public void Handle_GetNotificationChannel([Values] UserCapacityEnum userCapacity)
        {
            if (userCapacity == UserCapacityEnum.NONE)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => createNotificationsAsNotificationQueueCommand.GetNotificationChannel(userCapacity));
            }
            else if (userCapacity == UserCapacityEnum.CITIZEN || userCapacity == UserCapacityEnum.COMPANY)
            {
                NotificationMessageChannelEnum messageChannel = createNotificationsAsNotificationQueueCommand.GetNotificationChannel(userCapacity);
                Assert.That(messageChannel, Is.EqualTo(NotificationMessageChannelEnum.EBOKS));
            } 
            else if (userCapacity == UserCapacityEnum.EMPLOYEE)
            {
                NotificationMessageChannelEnum messageChannel = createNotificationsAsNotificationQueueCommand.GetNotificationChannel(userCapacity);
                Assert.That(messageChannel, Is.EqualTo(NotificationMessageChannelEnum.EMAIL));
            } 
        }

        private List<Notification> GetNotificationList(string templateText, HearingStatusEnum hearingStatusStatus)
        {
            var notification = new Notification()
            {
                NotificationType = new NotificationType()
                {
                    Type = NotificationTypeEnum.INVITED_TO_HEARING,
                    NotificationTemplate = new NotificationTemplate()
                    {
                        NotificationTemplateText = templateText
                    },
                    
                },
                Hearing = new Hearing()
                {
                    HearingStatus = new HearingStatus()
                    {
                        Status = hearingStatusStatus
                    },
                    Contents = new Collection<Content>()
                    {
                        new Content()
                        {
                            FieldId = 1,
                            ContentType = new ContentType()
                            {
                                Type = ContentTypeEnum.TEXT
                            },
                            TextContent = "Title"
                        }
                    }
                },
                Comment = new Comment()
                {
                    CommentDeclineInfo = new CommentDeclineInfo()
                    {
                        DeclineReason = "declined comment due to <something>",
                        DeclinerInitials = "SOMEONE"
                    },
                    UserId = 123,
                    CommentStatus = new CommentStatus()
                    {
                        Status = BallerupKommune.Models.Enums.CommentStatus.NOT_APPROVED
                    }
                },
                User = new User()
                {
                    Email = "test@email.dk",
                    Id = 123,
                    UserCapacity = new UserCapacity()
                    {
                        Capacity = UserCapacityEnum.EMPLOYEE,
                    }
                }
            };

            return new List<Notification>() { notification };
        }
    }
}