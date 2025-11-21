using Agora.Models.Models;
using Agora.Operations.Models.NotificationContentSpecifications.Commands;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Threading.Tasks;
using NotificationType = Agora.Models.Enums.NotificationType;

namespace Agora.Operations.UnitTests.Models.NotificationContentSpecifications.Commands
{
    public class CreateNotificationContentSpecificationCommandValidationTests
    {
        private readonly CreateNotificationContentSpecificationCommandValidator _validator = new();

        [Test]
        public async Task CreateNotificationContentSpecification_ValidRequest_ShouldNotThrowException()
        {
            var command = new CreateNotificationContentSpecificationCommand
            {
                HearingId = 1,
                NotificationTypeEnum = NotificationType.INVITED_TO_HEARING
            };
            await ValidationTestFramework
                .For<CreateNotificationContentSpecificationCommand, NotificationContentSpecification>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task CreateNotificationContentSpecification_HearingIdIsZero_ShouldThrowValidationException()
        {
            var command = new CreateNotificationContentSpecificationCommand
            {
                HearingId = 0,
                NotificationTypeEnum = NotificationType.INVITED_TO_HEARING
            };

            await ValidationTestFramework
                .For<CreateNotificationContentSpecificationCommand, NotificationContentSpecification>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(CreateNotificationContentSpecificationCommand.HearingId));
        }

        [Test]
        public async Task CreateNotificationContentSpecification_HearingIdEmpty_ShouldThrowValidationException()
        {
            var command = new CreateNotificationContentSpecificationCommand
            {
                NotificationTypeEnum = NotificationType.INVITED_TO_HEARING
            };

            await ValidationTestFramework
                .For<CreateNotificationContentSpecificationCommand, NotificationContentSpecification>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(CreateNotificationContentSpecificationCommand.HearingId));
        }

        [Test]
        public async Task CreateNotificationContentSpecification_NotificationTypeEnumIsEmpty_ShouldThrowValidationException()
        {
            var command = new CreateNotificationContentSpecificationCommand
            {
                HearingId = 1
            };

            await ValidationTestFramework
                .For<CreateNotificationContentSpecificationCommand, NotificationContentSpecification>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(CreateNotificationContentSpecificationCommand.NotificationTypeEnum));
        }
    }
}

