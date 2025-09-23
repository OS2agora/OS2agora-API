using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Constants;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Models.HearingTypes.Commands.CreateHearingType;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.UnitTests.Models.KleMappings.Commands
{
    public class CreateHearingTypeTests : ModelsTestBase<CreateHearingTypeCommand, HearingType>
    {
        public CreateHearingTypeTests()
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
        public async Task CreateHearingType_HasRole(string roleToTest)
        {
            var shouldSucceed = roleToTest == Security.Roles.Administrator;

            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(shouldSucceed);

            var request = new CreateHearingTypeCommand();

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
                Assert.IsTrue(result.IsActive && !string.IsNullOrEmpty(result.Name) && result.FieldTemplates.Any() &&
                              result.IsInternalHearing && result.KleMappings.Any() &&
                              result.Hearings.Any() && result.HearingTemplate != null);
            }
        }

        private HearingType GetHandlerResults()
        {
            return new HearingType
            {
                Id = 1,
                IsActive = true,
                Name = "test",
                HearingTemplate = new HearingTemplate(),
                IsInternalHearing = true,
                FieldTemplates = new List<FieldTemplate>
                {
                    new FieldTemplate()
                },
                Hearings = new List<Hearing>
                {
                    new Hearing()
                },
                KleMappings = new List<KleMapping>
                {
                    new KleMapping()
                }
            };
        }
    }
}