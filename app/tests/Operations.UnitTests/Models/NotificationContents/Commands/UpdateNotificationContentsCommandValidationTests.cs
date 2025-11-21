using Agora.Models.Models;
using Agora.Operations.Models.NotificationContents.Commands;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.NotificationContents.Commands
{
    public class UpdateNotificationContentsCommandValidationTests
    {
        private readonly UpdateNotificationContentsCommandValidator _validator = new();

        [Test]
        public async Task UpdateNotificationContents_ValidRequest_ShouldNotThrowException()
        {
            var command = new UpdateNotificationContentsCommand
            {
                HearingId = 1,
                NotificationContentSpecificationId = 1,
                NotificationContents = new List<NotificationContent>
                {
                    new NotificationContent
                    {
                        Id = 1,
                        TextContent = "Valid content"
                    }
                }
            };
            await ValidationTestFramework
                .For<UpdateNotificationContentsCommand, List<NotificationContent>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task UpdateNotificationContents_HearingIdIsZero_ShouldThrowValidationException()
        {
            var command = new UpdateNotificationContentsCommand
            {
                HearingId = 0,
                NotificationContentSpecificationId = 1,
                NotificationContents = new List<NotificationContent>
                {
                    new NotificationContent
                    {
                        Id = 1,
                        TextContent = "Valid content"
                    }
                }
            };

            await ValidationTestFramework
                .For<UpdateNotificationContentsCommand, List<NotificationContent>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateNotificationContentsCommand.HearingId));
        }

        [Test]
        public async Task UpdateNotificationContents_NotificationContentSpecificationIdIsZero_ShouldThrowValidationException()
        {
            var command = new UpdateNotificationContentsCommand
            {
                HearingId = 1,
                NotificationContentSpecificationId = 0,
                NotificationContents = new List<NotificationContent>
                {
                    new NotificationContent
                    {
                        Id = 1,
                        TextContent = "Valid content"
                    }
                }
            };

            await ValidationTestFramework
                .For<UpdateNotificationContentsCommand, List<NotificationContent>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateNotificationContentsCommand.NotificationContentSpecificationId));
        }

        [Test]
        public async Task UpdateNotificationContents_NotificationContentIsNull_ShouldThrowValidationException()
        {
            var command = new UpdateNotificationContentsCommand
            {
                HearingId = 1,
                NotificationContentSpecificationId = 1,
                NotificationContents = new List<NotificationContent>
                {
                    null
                }
            };

            await ValidationTestFramework
                .For<UpdateNotificationContentsCommand, List<NotificationContent>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateNotificationContentsCommand.NotificationContents) + "[0]");
        }

        [Test]
        public async Task UpdateNotificationContents_NotificationContentsIsNull_ShouldThrowValidationException()
        {
            var command = new UpdateNotificationContentsCommand
            {
                HearingId = 1,
                NotificationContentSpecificationId = 1,
                NotificationContents = null
            };

            await ValidationTestFramework
                .For<UpdateNotificationContentsCommand, List<NotificationContent>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateNotificationContentsCommand.NotificationContents));
        }

        [Test]
        public async Task UpdateNotificationContents_NotificationContentsIsEmpty_ShouldThrowValidationException()
        {
            var command = new UpdateNotificationContentsCommand
            {
                HearingId = 1,
                NotificationContentSpecificationId = 1,
                NotificationContents = new List<NotificationContent>()
            };

            await ValidationTestFramework
                .For<UpdateNotificationContentsCommand, List<NotificationContent>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateNotificationContentsCommand.NotificationContents));
        }

        [Test]
        public async Task UpdateNotificationContents_NotificationContentIdIsZero_ShouldThrowValidationException()
        {
            var command = new UpdateNotificationContentsCommand
            {
                HearingId = 1,
                NotificationContentSpecificationId = 1,
                NotificationContents = new List<NotificationContent>
                {
                    new NotificationContent
                    {
                        Id = 0,
                        TextContent = "Valid content"
                    }
                }
            };

            await ValidationTestFramework
                .For<UpdateNotificationContentsCommand, List<NotificationContent>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateNotificationContentsCommand.NotificationContents) + "[0]" + "." + nameof(NotificationContent.Id));
        }
    }
}
