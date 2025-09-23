using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Models.Users.Queries.GetUsers;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Operations.Common.Exceptions;

namespace BallerupKommune.Operations.UnitTests.Models.Users.Queries
{
    public class GetUsersTest : ModelsTestBase<GetUsersQuery, List<User>>
    {
        public GetUsersTest()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(new List<User> { new User() }));
        }

        [Test]
        [TestCase("HearingOwner")]
        [TestCase("HearingOwner", "RandomRole")]
        [TestCase("Administrator")]
        [TestCase("Administrator", "RandomRole")]
        [TestCase("HearingOwner", "Administrator")]
        [TestCase("RandomRole")]
        [TestCase()]
        public async Task GetUsers_HasAnyRole(params string[] roles)
        {
            var shouldFail = !roles.ToList().Any(value => value == "Administrator" || value == "HearingOwner");

            SecurityExpressionRoot
                .Setup(x => x.HasAnyRole(It.IsAny<List<string>>()))
                .Returns(!shouldFail);

            var request = new GetUsersQuery();
            if (shouldFail)
            {
                FluentActions
                    .Invoking(() =>
                        SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                    .Should().Throw<ForbiddenAccessException>();
            }
            else
            {
                var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);
                Assert.IsTrue(result.Any());
            }
        }
    }
}