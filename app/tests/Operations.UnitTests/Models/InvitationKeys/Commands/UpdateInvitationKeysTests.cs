using Agora.Models.Models;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.InvitationKeys.Commands.UpdateInvitationKeys;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.InvitationKeys.Commands
{
    public class UpdateInvitationKeysTests : ModelsTestBase<UpdateInvitationKeysCommand, List<InvitationKey>>
    {
        public UpdateInvitationKeysTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResults()));
        }

        [Test]
        public async Task UpdateInvitationInvitationKeys_Administrator_Should_Not_Throw_Error()
        {
            SecurityExpressionRoot
                .Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string param) => param == Security.Roles.Administrator);

            var request = new UpdateInvitationKeysCommand();

            var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.AreEqual(result.First().Id, 1);
        }

        [Test]
        [TestCase("HearingCreator, HearingOwner, HearingResponder, HearingInvitee, HearingReviewer, Anonymous, Citizen, Employee")]
        [TestCase("")]
        [TestCase(null)]
        public void UpdateInvitationKeys_NotAdministrator_Should_Throw_Error(params string[] roleToTest)
        {
            SecurityExpressionRoot
                .Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string param) => roleToTest.Contains(param));

            var request = new UpdateInvitationKeysCommand();

            FluentActions.Invoking(() => SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object)).Should().Throw<ForbiddenAccessException>();
        }

        private List<InvitationKey> GetHandlerResults()
        {
            return new List<InvitationKey>
            {
                new InvitationKey
                {
                    Id = 1,
                    InvitationGroup = new InvitationGroup(),
                }
            };
        }
    }
}