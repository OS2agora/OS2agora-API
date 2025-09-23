using System.Threading;
using BallerupKommune.Operations.Common.Constants;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Models.HearingTypes.Commands.DeleteHearingType;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

namespace BallerupKommune.Operations.UnitTests.Models.HearingTypes.Commands
{
    public class DeleteHearingTypeTests : ModelsTestBase<DeleteHearingTypeCommand, Unit>
    {
        public DeleteHearingTypeTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Unit.Task);
        }

        [Test]
        [TestCase("Administrator")]
        [TestCase("HearingCreator")]
        [TestCase("")]
        [TestCase("RandomRole")]
        [TestCase(null)]
        public void DeleteHearingType_HasRole(string roleToTest)
        {
            var shouldSucceed = roleToTest == Security.Roles.Administrator;

            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(shouldSucceed);

            var request = new DeleteHearingTypeCommand();

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