using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Constants;
using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Models.SubjectAreas.Command.CreateSubjectArea;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.UnitTests.Models.SubjectAreas.Command
{
    public class CreateSubjectAreaTests : ModelsTestBase<CreateSubjectAreaCommand, SubjectArea>
    {
        public CreateSubjectAreaTests()
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
        public async Task CreateSubjectArea_HasRole(string roleToTest)
        {
            var shouldSucceedPre = roleToTest == Security.Roles.Administrator;
            var shouldSucceedPost = roleToTest == Security.Roles.HearingOwner || roleToTest == Security.Roles.Administrator;

            SecurityExpressionRoot.Setup(x => x.HasRole(It.Is<string>(role => role == Security.Roles.Administrator)))
                .Returns(shouldSucceedPre);
            SecurityExpressionRoot.Setup(x => x.HasAnyRole(It.Is<List<string>>(roles => roles.Count == 2
                && roles.Contains(Security.Roles.HearingOwner) && roles.Contains(Security.Roles.Administrator))))
                .Returns(shouldSucceedPost);

            var request = new CreateSubjectAreaCommand();
            if (!shouldSucceedPre)
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
                Assert.IsTrue(result.Id == 1 && result.IsActive && result.Hearings.Any() &&
                              !string.IsNullOrEmpty(result.Name));
            }
        }

        private SubjectArea GetHandlerResults()
        {
            return new SubjectArea
            {
                Id = 1,
                Name = "Test",
                Hearings = new List<Hearing>
                {
                    new Hearing()
                },
                IsActive = true
            };
        }
    }
}