using Agora.Models.Models;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.InvitationSources.Queries.GetInvitationSources;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.InvitationSources.Queries
{
    public class GetInvitationSourcesTest : ModelsTestBase<GetInvitationSourcesQuery, List<InvitationSource>>
    {
        [SetUp]
        public void SetUp()
        {
            RequestHandlerDelegateMock.Setup(x => x()).Returns(Task.FromResult(GetHandlerResult()));
        }

        [Test]
        public async Task GetInvitationSources_HearingCreator_Should_Not_Throw_Error()
        {
            SecurityExpressionRoot
                .Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string param) => param == Security.Roles.HearingCreator);

            var request = new GetInvitationSourcesQuery();

            var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.IsTrue(result.Count == 2);
        }

        [Test]
        [TestCase("Administrator, HearingOwner, HearingResponder, HearingInvitee, HearingReviewer, Anonymous, Citizen, Employee")]
        [TestCase("")]
        [TestCase(null)]
        public void GetInvitationSources_NotHearingCreator_Throws_Error(params string[] roleToTest)
        {
            SecurityExpressionRoot
                .Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string param) => roleToTest.Contains(param));

            var request = new GetInvitationSourcesQuery();

            FluentActions.Invoking(() => SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object)).Should().Throw<ForbiddenAccessException>();
        }

        private List<InvitationSource> GetHandlerResult()
        {
            return new List<InvitationSource>
            {
                new InvitationSource
                {
                    Id = 1,
                    Name = "Source 1"
                },
                new InvitationSource
                {
                    Id = 2,
                    Name = "Source 2"
                }
            };
        }
    }
}
