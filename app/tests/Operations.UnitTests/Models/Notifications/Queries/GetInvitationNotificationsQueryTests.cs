using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.Notifications.Queries.GetInvitationNotifications;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.Notifications.Queries
{
    public class GetInvitationNotificationsQueryTests : ModelsTestBase<GetInvitationNotificationsQuery, List<Notification>>
    {
        [SetUp]
        public void SetUp()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResult()));
        }

        [Test]
        public async Task GetInvitationNotifications_HearingOwner_Should_Not_Throw_Error()
        {
            SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
                .Returns(true);

            var request = new GetInvitationNotificationsQuery();

            var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.IsTrue(result.Count == 2);
        }

        [Test]
        public void UpdateInviteesFromInvitationGroup_NotHearingOwner_Throws_Error()
        {
            SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
                .Returns(false);

            var request = new GetInvitationNotificationsQuery();

            FluentActions
                .Invoking(() =>
                    SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                .Should().Throw<ForbiddenAccessException>();
        }

        private List<Notification> GetHandlerResult()
        {
            return new List<Notification>
            {
                new Notification
                {
                    Id = 1
                },
                new Notification
                {
                    Id = 2
                }
            };
        }

    }
}
