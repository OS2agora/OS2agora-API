using BallerupKommune.Operations.Common.Constants;
using BallerupKommune.Operations.Models.SubjectAreas.Command.DeleteSubjectArea;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using System.Threading;
using BallerupKommune.Operations.Common.Exceptions;

namespace BallerupKommune.Operations.UnitTests.Models.SubjectAreas.Command
{
    public class DeleteSubjectAreaTests : ModelsTestBase<DeleteSubjectAreaCommand, Unit>
    {
        public DeleteSubjectAreaTests()
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
        [TestCase("null")]
        public void DeleteSubjectArea_HasRole(string roleToTest)
        {
            var shouldSucceed = roleToTest == Security.Roles.Administrator;

            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(shouldSucceed);

            var request = new DeleteSubjectAreaCommand();

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
