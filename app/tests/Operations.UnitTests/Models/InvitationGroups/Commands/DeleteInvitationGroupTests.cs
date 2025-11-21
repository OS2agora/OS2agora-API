using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.InvitationGroups.Commands.DeleteInvitationGroup;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading;

namespace Agora.Operations.UnitTests.Models.InvitationGroups.Commands
{
    public class DeleteInvitationGroupTests : ModelsTestBase<DeleteInvitationGroupCommand, Unit>
    {
        public DeleteInvitationGroupTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Unit.Task);
        }

        [Test]
        public void DeleteInvitationGroup_Administrator_Should_Not_Throw_Error()
        {
            SecurityExpressionRoot
                .Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string param) => param == Security.Roles.Administrator);

            var request = new DeleteInvitationGroupCommand();

            FluentActions.Invoking(() => SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object)).Should().NotThrow<ForbiddenAccessException>();
        }

        [Test]
        [TestCase("HearingCreator, HearingOwner, HearingResponder, HearingInvitee, HearingReviewer, Anonymous, Citizen, Employee")]
        [TestCase("")]
        [TestCase(null)]
        public void DeleteInvitationGroup_NotAdministrator_Should_Throw_Error(params string[] roleToTest)
        {
            SecurityExpressionRoot
                .Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string param) => roleToTest.Contains(param));

            var request = new DeleteInvitationGroupCommand();

            FluentActions.Invoking(() => SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object)).Should().Throw<ForbiddenAccessException>();
        }
    }
}