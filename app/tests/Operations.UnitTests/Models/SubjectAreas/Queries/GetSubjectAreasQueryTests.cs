using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Constants;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Models.SubjectAreas.Queries.GetSubjectAreas;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BallerupKommune.Operations.UnitTests.Models.SubjectAreas.Queries
{
    public class GetSubjectAreasTests : ModelsTestBase<GetSubjectAreasQuery, List<SubjectArea>>
    {
        public GetSubjectAreasTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResults()));
        }

        [Test]                     
        [TestCase("Administrator")]
        [TestCase("HearingOwner")]
        [TestCase("")]
        [TestCase("RandomRole")]
        [TestCase(null)]
        [TestCase("Administrator","RandomRole")]
        [TestCase("HearingOwner", "RandomRole")]
        [TestCase("User", "RandomRole")]
        [TestCase("Administrator", "HearingOwner")]
        public async Task GetSubjectAreasQuery_HasRole(params string[] rolesToTest)
        {
            var shouldSucceed =
                rolesToTest.Any(role => role==Security.Roles.HearingOwner || role==Security.Roles.Administrator);

            SecurityExpressionRoot.Setup(x => x.HasAnyRole(It.Is<List<string>>(roles => roles.Count == 2
                && roles.Contains(Security.Roles.HearingOwner) && roles.Contains(Security.Roles.Administrator))))
                .Returns(shouldSucceed);

            var request = new GetSubjectAreasQuery();

            if (!shouldSucceed)
            {
                var result = await
                    SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);
                Assert.That(result, Is.Empty);
            }
            else
            {
                var result = await
                    SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);
                Assert.That(result.Count, Is.EqualTo(2));
                Assert.IsTrue(result.All(x =>
                    x.Hearings != null && x.IsActive && !string.IsNullOrEmpty(x.Name)));
            }
        }

        private List<SubjectArea> GetHandlerResults()
        {
            return new List<SubjectArea>
            {
                new SubjectArea()
                {
                    Id = 1,
                    Hearings = new List<Hearing>
                    {
                        new Hearing()
                    },
                    Name = "Name",
                    IsActive = true
                },
                new SubjectArea()
                {
                    Id = 2,
                    Hearings = new List<Hearing>
                    {
                        new Hearing()
                    },
                    Name = "Different Name",
                    IsActive = true
                }
            };

        }
    }
}