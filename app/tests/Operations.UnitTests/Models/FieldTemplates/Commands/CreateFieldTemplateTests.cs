using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Constants;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Models.FieldTemplates.Commands.CreateFieldTemplate;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BallerupKommune.Operations.UnitTests.Models.FieldTemplates.Commands
{
    public class CreateFieldTemplateTests : ModelsTestBase<CreateFieldTemplateCommand, FieldTemplate>
    {
        public CreateFieldTemplateTests()
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
        public async Task CreateFieldTemplate_HasRole(string roleToTest)
        {
            var shouldSucceed = roleToTest == Security.Roles.Administrator;
            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(shouldSucceed);
            var request = new CreateFieldTemplateCommand();
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
                Assert.IsTrue(result.Field != null && result.HearingType != null &&
                              !string.IsNullOrEmpty(result.Name) && !string.IsNullOrEmpty(result.Text) &&
                              result.Id == 1);
            }
        }

        private FieldTemplate GetHandlerResults()
        {
            return new FieldTemplate
            {
                Id = 1,
                Field = new Field(),
                HearingType = new HearingType(),
                Name = "Test",
                Text = "Some text"
            };
        }
    }
}