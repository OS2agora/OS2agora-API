using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Models;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.FieldTemplates.Commands.UpdateFieldTemplate;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Agora.Operations.UnitTests.Models.FieldTemplates.Commands
{
    public class UpdateFieldTemplateTests : ModelsTestBase<UpdateFieldTemplateCommand, FieldTemplate>
    {
        public UpdateFieldTemplateTests()
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
        public async Task UpdateFieldTemplate_HasRole(string roleToTest)
        {
            var shouldSucceed = roleToTest == Security.Roles.Administrator;

            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(shouldSucceed);

            var request = new UpdateFieldTemplateCommand();

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
                              !string.IsNullOrEmpty(result.Name) && !string.IsNullOrEmpty(result.Text));
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
                Text = "To Test"
            };
        }
    }
}