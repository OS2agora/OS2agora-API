using Agora.Models.Models;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.GlobalContents.Commands;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GlobalContentType = Agora.Models.Enums.GlobalContentType;

namespace Agora.Operations.UnitTests.Models.GlobalContents.Commands
{
    public class CreateGlobalContentTests : ModelsTestBase<CreateGlobalContentCommand, GlobalContent>
    {
        public CreateGlobalContentTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResult()));
        }

        [Test]
        [TestCase("Administrator")]
        [TestCase("HearingCreator")]
        [TestCase("")]
        [TestCase("RandomRole")]
        [TestCase(null)]
        public async Task CreateGlobalContentCommand_HasRole(string roleToTest)
        {
            var shouldSucceed = roleToTest == Security.Roles.Administrator;

            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(shouldSucceed);

            var request = new CreateGlobalContentCommand();

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
                Assert.IsTrue(result.Consents.Any() && result.Version == 404 && !string.IsNullOrEmpty(result.Content) 
                              && result.GlobalContentType.Type == GlobalContentType.TERMS_AND_CONDITIONS);
            }
        }

        private GlobalContent GetHandlerResult()
        {
            return new GlobalContent()
            {
                Consents = new List<Consent>
                {
                    new Consent()
                },
                Content = "Some string",
                Version = 404,
                GlobalContentType = new Agora.Models.Models.GlobalContentType
                {
                    Type = GlobalContentType.TERMS_AND_CONDITIONS
                }
            };
        }
    }
}