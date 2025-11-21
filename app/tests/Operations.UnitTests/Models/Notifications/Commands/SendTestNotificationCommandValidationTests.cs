using Agora.Models.Enums;
using Agora.Operations.Models.Notifications.Commands.SendTestNotification;
using Agora.Operations.UnitTests.Common.Behaviours;
using MediatR;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.Notifications.Commands;

public class SendTestNotificationCommandValidationTests
{
    private readonly SendTestNotificationCommandValidator _validator = new();

    [Test]
    public async Task SendTestNotification_ValidRequest_ShouldNotThrowException()
    {
        var command = new SendTestNotificationCommand
        {
            HearingId = 1,
            NotificationType = NotificationType.INVITED_TO_HEARING
        };

        await ValidationTestFramework.For<SendTestNotificationCommand, Unit>()
            .WithValidators(_validator)
            .ShouldPassValidation(command);
    }

    [Test]
    public async Task SendTestNotification_EmptyHearingId_ShouldThrowValidationException()
    {
        var command = new SendTestNotificationCommand
        {
            NotificationType = NotificationType.INVITED_TO_HEARING
        };

        await ValidationTestFramework
            .For<SendTestNotificationCommand, Unit>()
            .WithValidators(_validator)
            .ShouldFailValidationWithError(command, nameof(SendTestNotificationCommand.HearingId));
    }

    [Test]
    public async Task SendTestNotification_EmptyNotificationType_ShouldThrowValidationException()
    {
        var command = new SendTestNotificationCommand
        {
            HearingId = 1
        };

        await ValidationTestFramework
            .For<SendTestNotificationCommand, Unit>()
            .WithValidators(_validator)
            .ShouldFailValidationWithError(command, nameof(SendTestNotificationCommand.NotificationType));
    }
}