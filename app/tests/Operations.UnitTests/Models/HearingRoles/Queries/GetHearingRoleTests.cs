using Agora.Models.Models;
using Agora.Operations.Models.HearingRoles.Queries.GetHearingRoles;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Operations.Common.Constants;
using NUnit.Framework.Constraints;
using HearingRoleEnum = Agora.Models.Enums.HearingRole;

namespace Agora.Operations.UnitTests.Models.HearingRoles.Queries
{
    public class GetHearingRoleTests : ModelsTestBase<GetHearingRolesQuery, List<HearingRole>>
    {
        [TearDown]
        public void TearDown()
        {
            SecurityExpressionRoot.Reset();
            SecurityExpressionsMock.Reset();
            RequestHandlerDelegateMock.Reset();
        }

        [Test]
        [TestCase("HearingOwner")]
        [TestCase("HearingOwner", "RandomRole")]
        [TestCase("Administrator")]
        [TestCase("Administrator", "RandomRole")]
        [TestCase("HearingOwner", "Administrator")]
        [TestCase("RandomRole")]
        public async Task GetHearingRoles_BasedOnlyOnRole_FiltersCorrect(params string[] roles)
        {
            var shouldFail = !roles.ToList().Any(value => value == "Administrator" || value == "HearingOwner");
            SetUpQueryResponse(new List<HearingRole> { new HearingRole() });

            SecurityExpressionRoot
                .Setup(x => x.HasAnyRole(It.IsAny<List<string>>()))
                .Returns(!shouldFail);

            var request = new GetHearingRolesQuery();
            var result = await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);
            Assert.That(result, shouldFail ? Is.Empty : Is.Not.Empty);
        }

        [Test]
        public async Task GetHearingRoles_BasedOnUserHearingRoles_FiltersCorrect([Values] bool isEmployee,
            [Values] bool hasRole,
            [Values(HearingRoleEnum.HEARING_OWNER, HearingRoleEnum.HEARING_REVIEWER, HearingRoleEnum.HEARING_INVITEE,
                HearingRoleEnum.HEARING_RESPONDER)]
            HearingRoleEnum role)
        {
            var hearingRole = new HearingRole
            {
                Id = 1,
                Role = role,
            };

            var alwaysFilteredHearingRole = new HearingRole
            {
                Id = 2,
                Role = HearingRoleEnum.NONE
            };
            
            SecurityExpressionRoot.Setup(expressions => expressions.HasRole(Security.Roles.Employee)).Returns(isEmployee);
            SecurityExpressionsMock.Setup(expressions => expressions.HasRoleOnAnyHearing(role)).Returns(hasRole);
            
            SetUpQueryResponse(new List<HearingRole> { hearingRole, alwaysFilteredHearingRole });

            List<HearingRole> result = await SecurityBehaviour.Handle(new GetHearingRolesQuery(),
                CancellationToken.None, RequestHandlerDelegateMock.Object);
            
            Assert.That(result, isEmployee && hasRole ? (IResolveConstraint)Has.One.EqualTo(hearingRole) : Is.Empty);
        }

        private void SetUpQueryResponse(List<HearingRole> response)
        {
            RequestHandlerDelegateMock
                .Setup(handlerDelegate => handlerDelegate())
                .ReturnsAsync(response);
        }
    }
}