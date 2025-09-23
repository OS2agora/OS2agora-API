using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Constants;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Models.FieldTemplates.Queries.GetFieldTemplates;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BallerupKommune.Operations.UnitTests.Models.FieldTemplates.Queries
{
    public class GetFieldTemplatesTests : ModelsTestBase<GetFieldTemplatesQuery, List<FieldTemplate>>
    {
        public GetFieldTemplatesTests()
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
        public async Task GetFieldTemplates_HasRole(string roleToTest)
        {
            var shouldSucceed = roleToTest == Security.Roles.Administrator;
            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(shouldSucceed);
            var request = new GetFieldTemplatesQuery();
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
                    x.Field != null && x.HearingType != null && !string.IsNullOrEmpty(x.Name) &&
                    !string.IsNullOrEmpty(x.Text)));
            }
        }

        private List<FieldTemplate> GetHandlerResults()
        {
            return new List<FieldTemplate>
            {
                new FieldTemplate
                {
                    Id = 1,
                    Field = new Field(),
                    HearingType = new HearingType(),
                    Name = "Test",
                    Text = "To Test"
                },
                new FieldTemplate
                {
                    Id = 2,
                    Field = new Field(),
                    HearingType = new HearingType(),
                    Name = "Test",
                    Text = "To Test"
                }
            };
        }
    }
}