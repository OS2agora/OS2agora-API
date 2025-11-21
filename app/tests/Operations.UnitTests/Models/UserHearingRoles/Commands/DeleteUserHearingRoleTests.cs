using Agora.Operations.Models.UserHearingRoles.Commands.DeleteUserHearingRole;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Agora.Operations.Common.Exceptions;

namespace Agora.Operations.UnitTests.Models.UserHearingRoles.Commands
{
    public class DeleteUserHearingRoleTests : ModelsTestBase<DeleteUserHearingRoleCommand, Unit>
    {
        public DeleteUserHearingRoleTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Unit.Task);
        }

        [Test]
        [TestCase(10)]
        [TestCase(1)]
        public async Task DeleteUserHearingRole_IsHearingOwner(int hearingId)
        {
            var correctHearingId = 1;

            SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
                .Returns((int id) => id == correctHearingId);

            var request = new DeleteUserHearingRoleCommand
            {
                HearingId = hearingId
            };

            if (hearingId != correctHearingId)
            {
                FluentActions
                    .Invoking(() =>
                        SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                    .Should().Throw<ForbiddenAccessException>();
            }
            else
            {
                var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);
                Assert.IsNotNull(result);
            }
        }
    }
}
