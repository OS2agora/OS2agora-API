using Agora.Models.Models;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.KleMappings.Commands.UpdateKleMappings;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.KleMappings.Commands
{
    public class UpdateKleMappingsTest : ModelsTestBase<UpdateKleMappingsCommand, List<KleMapping>>
    {
        public UpdateKleMappingsTest()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(new List<KleMapping>
                {
                    new KleMapping
                    {
                        Id = 1,
                        HearingType = new HearingType(),
                        KleHierarchy = new KleHierarchy()
                    }
                }));
        }

        [Test]
        [TestCase("Administrator")]
        [TestCase("HearingCreator")]
        [TestCase("")]
        [TestCase("RandomRole")]
        [TestCase(null)]
        public async Task UpdateKleMappings_HasRole(string roleToTest)
        {
            var shouldSucceed = roleToTest == Security.Roles.Administrator;

            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(shouldSucceed);

            var request = new UpdateKleMappingsCommand();

            if (!shouldSucceed)
            {
                FluentActions
                    .Invoking(() =>
                        SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object))
                    .Should().Throw<ForbiddenAccessException>();
            }
            else
            {
                var result = await SecurityBehaviour.Handle(request, CancellationToken.None,
                    RequestHandlerDelegateMock.Object);

                Assert.IsTrue(result.TrueForAll(kleMapping => kleMapping.HearingType != null && kleMapping.KleHierarchy != null && kleMapping.Id != 0));
            }
        }
    }
}