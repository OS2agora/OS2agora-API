using System.Threading;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Models.Hearings.Command.DeleteHearing;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

namespace BallerupKommune.Operations.UnitTests.Models.Hearings.Commands
{
    public class DeleteHearingTests : ModelsTestBase<DeleteHearingCommand, Unit>
    {
        public DeleteHearingTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Unit.Task);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DeleteHearing_HasRole(bool shouldSucceed)
        {
            SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
                .Returns(shouldSucceed);

            var request = new DeleteHearingCommand();

            if (!shouldSucceed)
            {
                FluentActions
                    .Invoking(() =>
                        SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                    .Should().Throw<ForbiddenAccessException>();
            }
            else
            {
                FluentActions
                    .Invoking(() =>
                        SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                    .Should().NotThrow<ForbiddenAccessException>();
            }
        }
    }
}