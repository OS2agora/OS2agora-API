using Agora.Models.Models;
using Agora.Operations.Models.UserHearingRoles.Commands.CreateUserHearingRole;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using Agora.Operations.Common.Exceptions;

namespace Agora.Operations.UnitTests.Models.UserHearingRoles.Commands
{
    public class CreateUserHearingRoleTests : ModelsTestBase<CreateUserHearingRoleCommand, UserHearingRole>
    {
        public CreateUserHearingRoleTests()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(new UserHearingRole()));
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, false, true)]
        [TestCase(true, true, false)]
        [TestCase(false, true, false)]
        [TestCase(true, false, false)]
        [TestCase(false, false, false)]
        public async Task CreateUserHearingRole_AllCases(bool isHearingOwnerResult, bool isHearingOwnerRoleResult, bool isAdministratorResult)
        {
            SecurityExpressionsMock.Setup(x => x.IsHearingOwnerRole(It.IsAny<int>()))
                .Returns(isHearingOwnerRoleResult);

            SecurityExpressionsMock.Setup(x => x.IsHearingOwnerByHearingId(It.IsAny<int>()))
                .Returns(isHearingOwnerResult);

            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(isAdministratorResult);

            var request = new CreateUserHearingRoleCommand
            {
                HearingId = 1,
                UserHearingRole = new UserHearingRole
                {
                    HearingRole = new HearingRole()
                }
            };

            var shouldSucceed = isHearingOwnerResult && !isHearingOwnerRoleResult || isAdministratorResult && isHearingOwnerRoleResult;

            if (!shouldSucceed)
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