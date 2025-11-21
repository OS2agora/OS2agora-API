using Agora.Models.Models;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Resolvers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using HearingRoleEnum = Agora.Models.Enums.HearingRole;

namespace Agora.Operations.UnitTests.Resolvers
{
    internal class CompanyHearingRoleResolverTest
    {
        private CompanyHearingRoleResolver _resolver;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<ICompanyHearingRoleDao> _companyHearingRoleDaoMock;
        private Mock<IHearingRoleResolver> _hearingRoleResolverMock;

        [SetUp]
        public void SetUp()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _companyHearingRoleDaoMock = new Mock<ICompanyHearingRoleDao>();
            _hearingRoleResolverMock = new Mock<IHearingRoleResolver>();
            _resolver = new CompanyHearingRoleResolver(_currentUserServiceMock.Object, _companyHearingRoleDaoMock.Object, _hearingRoleResolverMock.Object);
        }

        [Test]
        public async Task CompanyHearingRoleExists_ReturnsCorrect(
            [Values(null, 5)] int? hearingId,
            [Values(HearingRoleEnum.HEARING_INVITEE, HearingRoleEnum.HEARING_RESPONDER)]
            HearingRoleEnum role,
            [Values] bool companyIdSpecified,
            [Values] bool roleExists)
        {
            const int hearingRoleId = 1;
            const int companyId = 7;
            const int currentUserId = 1;

            var hearingRole = new HearingRole
            {
                Id = hearingRoleId,
                Role = role
            };

            var companyHearingRoles = new List<CompanyHearingRole>
            {
                new CompanyHearingRole
                {
                    Id = 1,
                    HearingId = hearingId ?? 1,
                    HearingRoleId = roleExists ? hearingRoleId : hearingRoleId + 1,
                    CompanyId = companyId
                }
            };

            if (hearingId.HasValue)
            {
                companyHearingRoles.Add(new CompanyHearingRole
                {
                    Id = 2,
                    HearingId = hearingId.Value + 1,
                    HearingRoleId = hearingRoleId,
                    CompanyId = companyId
                });
            }

            _currentUserServiceMock.Setup(userService => userService.CompanyId)
                .Returns(companyId);
            _currentUserServiceMock.Setup(userService => userService.DatabaseUserId).Returns(currentUserId);
            _hearingRoleResolverMock.Setup(resolver => resolver.GetHearingRole(role)).ReturnsAsync(hearingRole);
            _companyHearingRoleDaoMock
                .Setup(dao => dao.GetAllAsync(null, companyHearingRole => companyHearingRole.CompanyId == companyId))
                .ReturnsAsync(companyHearingRoles);

            bool result = await _resolver.CompanyHearingRoleExist(hearingId, role, companyIdSpecified ? (int?)companyId : null);

            Assert.That(result, Is.EqualTo(roleExists));
        }
    }
}
