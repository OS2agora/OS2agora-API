using Agora.Models.Models;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.InvitationGroupMappings.Commands.UpdateInvitationGroupMappings;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.InvitationGroupMappings.Commands
{
    public class UpdateInvitationGroupMappingsTests : ModelsTestBase<UpdateInvitationGroupMappingsCommand, List<InvitationGroupMapping>>
    {
        public UpdateInvitationGroupMappingsTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResults()));
        }

        [Test]
        public async Task UpdateInvitationGroupMappings_Administrator_Should_Not_Throw_Error()
        {
            SecurityExpressionRoot
                .Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string param) => param == Security.Roles.Administrator);

            var request = new UpdateInvitationGroupMappingsCommand();

            var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.AreEqual(result.First().Id, 1);
        }

        [Test]
        [TestCase("HearingCreator, HearingOwner, HearingResponder, HearingInvitee, HearingReviewer, Anonymous, Citizen, Employee")]
        [TestCase("")]
        [TestCase(null)]
        public void UpdateInvitationGroupMappings_NotAdministrator_Should_Throw_Error(params string[] roleToTest)
        {
            SecurityExpressionRoot
                .Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string param) => roleToTest.Contains(param));

            var request = new UpdateInvitationGroupMappingsCommand();

            FluentActions.Invoking(() => SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object)).Should().Throw<ForbiddenAccessException>();
        }

        private List<InvitationGroupMapping> GetHandlerResults()
        {
            return new List<InvitationGroupMapping>
            {
                new InvitationGroupMapping
                {
                    Id = 1,
                    InvitationGroup = new InvitationGroup(),
                    HearingType = new HearingType()
                }
            };
        }
    }
}