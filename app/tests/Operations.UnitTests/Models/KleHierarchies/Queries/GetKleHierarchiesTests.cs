using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Models;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.KleHierarchies.Queries.GetKleHierarchies;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Agora.Operations.UnitTests.Models.KleHierarchies.Queries
{
    public class GetKleHierarchiesTests : ModelsTestBase<GetKleHierarchiesQuery, List<KleHierarchy>>
    {
        public GetKleHierarchiesTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResults()));
        }

        [Test]
        [TestCase("Administrator")]
        [TestCase("HearingCreator")]
        [TestCase("")]
        [TestCase("RandomRole")]
        [TestCase(null)]
        public async Task GetKleHierarchies_HasRole(string roleToTest)
        {
            var shouldSucceed = roleToTest == Security.Roles.Administrator;

            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(shouldSucceed);

            var request = new GetKleHierarchiesQuery();
            if (!shouldSucceed)
            {
                FluentActions
                    .Invoking(() =>
                        SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                    .Should().Throw<ForbiddenAccessException>();
            }
            else
            {
                var result = await
                    SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);
                Assert.IsTrue(result.Count == 2);
                Assert.IsTrue(result.All(x =>
                    x.IsActive && !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.Number) &&
                    x.Hearings.Any() && x.KleHierarchyChildren.Any() && x.KleMappings.Any() &&
                    x.KleHierarchyParrent != null));
            }
        }

        private List<KleHierarchy> GetHandlerResults()
        {
            return new List<KleHierarchy>
            {
                new KleHierarchy
                {
                    Id = 1,
                    Name = "Test",
                    Number = "1.03.2",
                    IsActive = true,
                    KleHierarchyChildren = new List<KleHierarchy>
                    {
                        new KleHierarchy()
                    },
                    KleHierarchyParrent = new KleHierarchy(),
                    Hearings = new List<Hearing>
                    {
                        new Hearing()
                    },
                    KleMappings = new List<KleMapping>
                    {
                        new KleMapping()
                    }
                },
                new KleHierarchy
                {
                    Id = 2,
                    Name = "Test2",
                    Number = "1.03.3",
                    IsActive = true,
                    KleHierarchyChildren = new List<KleHierarchy>
                    {
                        new KleHierarchy()
                    },
                    KleHierarchyParrent = new KleHierarchy(),
                    Hearings = new List<Hearing>
                    {
                        new Hearing()
                    },
                    KleMappings = new List<KleMapping>
                    {
                        new KleMapping()
                    }
                }
            };
        }
    }
}