using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Models;
using Agora.Operations.Models.Hearings.Command.CreateHearing;
using Moq;
using NUnit.Framework;
using Agora.Operations.Common.Constants;
using FluentAssertions;
using System.Linq;
using Agora.Operations.Common.Exceptions;

namespace Agora.Operations.UnitTests.Models.Hearings.Commands
{
    public class CreateHearingCommandTests : ModelsTestBase<CreateHearingCommand, Hearing>
    {
        public CreateHearingCommandTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(new Hearing()));
        }

        [Test]
        [TestCase("HearingCreator")]
        [TestCase("HearingCreator", "RandomRole")]
        [TestCase("RandomRole")]
        [TestCase]
        public async Task CreateHearing_HasRole_Administrator(params string[] roles)
        {
            var shouldFail = roles.All(x => x != Security.Roles.HearingCreator);

            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(!shouldFail);

            var request = new CreateHearingCommand();

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
                Assert.IsNotNull(result);
            }
        }
    }
}