using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Models;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Models.SubjectAreas.Command.UpdateSubjectArea;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Agora.Operations.UnitTests.Models.SubjectAreas.Command
{
    public class UpdateCityAreaTests : ModelsTestBase<UpdateSubjectAreaCommand, SubjectArea>
    {
        public UpdateCityAreaTests()
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
        public async Task UpdateSubjectArea_HasRole(string roleToTest)
        {
            var shouldSucceedPre = roleToTest == Security.Roles.Administrator;
            var shouldSucceedPost = roleToTest == Security.Roles.HearingOwner || roleToTest == Security.Roles.Administrator;

            SecurityExpressionRoot.Setup(x => x.HasRole(It.Is<string>(role => role == Security.Roles.Administrator)))
                .Returns(shouldSucceedPre);
            SecurityExpressionRoot.Setup(x => x.HasAnyRole(It.Is<List<string>>(roles => roles.Count == 2
                && roles.Contains(Security.Roles.HearingOwner) && roles.Contains(Security.Roles.Administrator))))
                .Returns(shouldSucceedPost);

            var request = new UpdateSubjectAreaCommand();
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