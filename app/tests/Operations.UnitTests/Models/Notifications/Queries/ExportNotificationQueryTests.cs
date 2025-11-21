using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.Notifications.Queries.ExportNotification;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using NotificationType = Agora.Models.Enums.NotificationType;

namespace Agora.Operations.UnitTests.Models.Notifications.Queries;

public class ExportNotificationQueryTests : ModelsTestBase<ExportNotificationQuery, FileDownload>
{
    [SetUp]
    public void SetUp()
    {
        RequestHandlerDelegateMock
            .Setup(x => x())
            .Returns(Task.FromResult(new FileDownload()));
    }

    [Test]
    public async Task ExportNotification_HearingOwner_Should_Not_Throw_Error()
    {
        SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId((It.IsAny<int>())))
            .Returns(true);

        var request = new ExportNotificationQuery
        {
            HearingId = 1,
            NotificationType = NotificationType.INVITED_TO_HEARING
        };

        var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

        Assert.IsNotNull(result);
    }

    [Test]
    public async Task ExportNotification_NotHearingOwner_Throws_Error()
    {
        SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId((It.IsAny<int>())))
            .Returns(false);

        var request = new ExportNotificationQuery
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