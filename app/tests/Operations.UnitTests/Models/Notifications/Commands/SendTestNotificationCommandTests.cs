using Agora.Models.Enums;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.Notifications.Commands.SendTestNotification;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.Notifications.Commands;

public class SendTestNotificationCommandTests : ModelsTestBase<SendTestNotificationCommand, Unit>
{
    [SetUp]
    public void SetUp()
    {
        RequestHandlerDelegateMock
            .Setup(x => x())
            .Returns(Task.FromResult(Unit.Value));
    }

    [Test]
    public async Task SendTestNotification_HearingOwner_Should_Not_Throw_Error()
    {
        SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId((It.IsAny<int>())))
            .Returns(true);

        var request = new SendTestNotificationCommand
        {
            HearingId = 1,
            NotificationType = NotificationType.INVITED_TO_HEARING
        };

        var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

        Assert.IsNotNull(result);
    }

    [Test]
    public async Task SendTestNotification_NotHearingOwner_Throws_Error()
    {
        SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId((It.IsAny<int>())))
            .Returns(false);

        var request = new SendTestNotificationCommand
        {
            HearingId = 1,
            NotificationType = NotificationType.INVITED_TO_HEARING
        };

        FluentActions
            .Invoking(() =>
                SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
            .Should().Throw<ForbiddenAccessException>();
    }
}