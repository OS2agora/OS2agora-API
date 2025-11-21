using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.InvitationGroups.Queries.GetInvitationGroups;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.InvitationGroups.Queries
{
    public class GetInvitationGroupsTest : ModelsTestBase<GetInvitationGroupsQuery, List<InvitationGroup>>
    {
        [SetUp]
        public void SetUp()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResult()));
        }

        [Test]
        [TestCase("Administrator", "HearingOwner")]
        public async Task GetInvitationGroups_Administrator_Or_HearingOwner_Returns_All(params string[] rolesToTest)
        {
            SecurityExpressionRoot
                .Setup(x => x.HasAnyRole(It.IsAny<List<string>>()))
                .Returns((List<string> param) => rolesToTest.Any(param.Contains));

            var request = new GetInvitationGroupsQuery();

            var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.IsTrue(result.Count == 2);
        }

        [Test]
        [TestCase("HearingCreator", "HearingResponder", "HearingInvitee", "HearingReviewer", "Anonymous", "Citizen", "Employee")]
        [TestCase("")]
        [TestCase(null)]
        public void GetInvitationGroups_Not_Administrator_Or_HearingOwner_Throws_Error(params string[] rolesToTest)
        {
            SecurityExpressionRoot
                .Setup(x => x.HasAnyRole(It.IsAny<List<string>>()))
                .Returns((List<string> param) => rolesToTest.Any(param.Contains));

            var request = new GetInvitationGroupsQuery();

            FluentActions.Invoking(() => SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object)).Should().Throw<ForbiddenAccessException>();
        }

        private List<InvitationGroup> GetHandlerResult()
        {
            return new List<InvitationGroup>
            {
                new InvitationGroup
                {
                    Id = 1,
                    Name = "Group 1"
                },
                new InvitationGroup
                {
                    Id = 2,
                    Name = "Group 2"
                }
            };
        }
    }
}