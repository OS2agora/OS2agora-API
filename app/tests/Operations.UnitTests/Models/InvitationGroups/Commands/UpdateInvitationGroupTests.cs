using Agora.Models.Models;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.InvitationGroups.Commands.UpdateInvitationGroup;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.InvitationGroups.Commands
{
    public class UpdateInvitationGroupTests : ModelsTestBase<UpdateInvitationGroupCommand, InvitationGroup>
    {
        public UpdateInvitationGroupTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResults()));
        }

        [Test]
        public async Task UpdateInvitationGroup_Administrator_Should_Not_Throw_Error()
        {
            SecurityExpressionRoot
                .Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string param) => param == Security.Roles.Administrator);

            var request = new UpdateInvitationGroupCommand();

            var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.AreEqual(result.Id, 1);
            Assert.AreEqual(result.Name, "Group 1");
        }

        [Test]
        [TestCase("HearingCreator, HearingOwner, HearingResponder, HearingInvitee, HearingReviewer, Anonymous, Citizen, Employee")]
        [TestCase("")]
        [TestCase(null)]
        public void UpdateInvitationGroup_NotAdministrator_Should_Throw_Error(params string[] roleToTest)
        {
            SecurityExpressionRoot
                .Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string param) => roleToTest.Contains(param));

            var request = new UpdateInvitationGroupCommand();
            FluentActions.Invoking(() => SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object)).Should().Throw<ForbiddenAccessException>();
        }

        private InvitationGroup GetHandlerResults()
        {
            return new InvitationGroup
            {
                Id = 1,
                Name = "Group 1"
            };
        }
    }
}