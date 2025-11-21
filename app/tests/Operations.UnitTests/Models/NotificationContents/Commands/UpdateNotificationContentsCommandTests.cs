using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.NotificationContents.Commands;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.NotificationContents.Commands
{
    public class UpdateNotificationContentsCommandTests : ModelsTestBase<UpdateNotificationContentsCommand, List<NotificationContent>>
    {
        public UpdateNotificationContentsCommandTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResults()));
        }

        [Test]
        public async Task UpdateNotificationContents_HearingOwner_Should_Not_Throw_Error()
        {
            SecurityExpressionsMock
                .Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
                .Returns(true);

            var request = new UpdateNotificationContentsCommand();

            var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.AreEqual(result.First().Id, 0);
        }

        [Test]
        public async Task UpdateNotificationContents_NotHearingOwner_Should_Throw_Error()
        {
            SecurityExpressionsMock
                .Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
                .Returns(false);

            var request = new UpdateNotificationContentsCommand();

            await FluentActions.Invoking(async () => await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        private List<NotificationContent> GetHandlerResults()
        {
            return new List<NotificationContent>
            {
                new NotificationContent()
            };
        }
    }
}