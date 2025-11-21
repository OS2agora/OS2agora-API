using System.Threading;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.FieldTemplates.Commands.DeleteFieldTemplate;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

namespace Agora.Operations.UnitTests.Models.FieldTemplates.Commands
{
    public class DeleteFieldTemplateTests : ModelsTestBase<DeleteFieldTemplateCommand, Unit>
    {
        public DeleteFieldTemplateTests()
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
        public void DeleteFieldTemplate_HasRole(string roleToTest)
        {
            var shouldSucceed = roleToTest == Security.Roles.Administrator;
            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(shouldSucceed);
            var request = new DeleteFieldTemplateCommand();
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