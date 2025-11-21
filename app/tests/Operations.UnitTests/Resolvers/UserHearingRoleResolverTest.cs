using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Resolvers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Models;
using Agora.Operations.Common.Interfaces;
using HearingRoleEnum = Agora.Models.Enums.HearingRole;

namespace Agora.Operations.UnitTests.Resolvers
{
    internal class UserHearingRoleResolverTest
    {
        private UserHearingRoleResolver _resolver;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<IUserHearingRoleDao> _userHearingRoleDaoMock;
        private Mock<IHearingRoleResolver> _hearingRoleResolverMock;

        [SetUp]
        public void SetUp()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _userHearingRoleDaoMock = new Mock<IUserHearingRoleDao>();
            _hearingRoleResolverMock = new Mock<IHearingRoleResolver>();
            _resolver = new UserHearingRoleResolver(_currentUserServiceMock.Object,_userHearingRoleDaoMock.Object,_hearingRoleResolverMock.Object);
        }

        [Test]
        [TestCase( 1, 1)]
        [TestCase( 1, null)]
        [TestCase( 0, 1)]
        [TestCase( 0, null)]
        public async Task IsHearingOwner( int hearingId, int? userId)
        {
            int userHearingRoleHearingId = 1;
            const int databaseUserId = 1;
            List<UserHearingRole> userHearingRoles = new List<UserHearingRole>
            {
                new UserHearingRole{
                    HearingId = userHearingRoleHearingId,
                    HearingRoleId = 1,
                    UserId = 1,
                }
            }; 
            HearingRole hearingOwnerRole = new HearingRole()
            {
                Id = 1,
                Role = HearingRoleEnum.HEARING_OWNER
            };
            _userHearingRoleDaoMock
                .Setup(userHearingRoleDaoMock => userHearingRoleDaoMock
                    .GetAllAsync(null, userHearingRole => userHearingRole.UserId == databaseUserId))
                .ReturnsAsync(userHearingRoles);
            _hearingRoleResolverMock
                .Setup(hearingRoleResolver => hearingRoleResolver.GetHearingRole(HearingRoleEnum.HEARING_OWNER)).ReturnsAsync(hearingOwnerRole);
            _currentUserServiceMock.Setup(userService => userService.DatabaseUserId).Returns(databaseUserId);
            bool result = await _resolver.IsHearingOwner(hearingId, userId);
            Assert.True(result.Equals(hearingId == userHearingRoleHearingId));
        }

        [Test]
        [TestCase( 1, 1)]
        [TestCase( 1, null)]
        [TestCase( 0, 1)]
        [TestCase( 0, null)]
        public async Task IsHearingReviewer( int hearingId, int? userId)
        {
            int userHearingRoleHearingId = 1;
            const int databaseUserId = 1;
            List<UserHearingRole> userHearingRoles = new List<UserHearingRole>
            {
                new UserHearingRole{
                    HearingId = userHearingRoleHearingId,
                    HearingRoleId = 1,
                    UserId = 1,
                }
            };
            HearingRole hearingReviewerRole = new HearingRole()
            {
                Id = 1,
                Role = HearingRoleEnum.HEARING_REVIEWER
            };
            _userHearingRoleDaoMock
                .Setup(userHearingRoleDaoMock => userHearingRoleDaoMock
                    .GetAllAsync(null, userHearingRole => userHearingRole.UserId == databaseUserId))
                .ReturnsAsync(userHearingRoles);
            _hearingRoleResolverMock
                .Setup(hearingRoleResolver => hearingRoleResolver.GetHearingRole(HearingRoleEnum.HEARING_REVIEWER)).ReturnsAsync(hearingReviewerRole);
            _currentUserServiceMock.Setup(userService => userService.DatabaseUserId).Returns(databaseUserId);
            bool result = await _resolver.IsHearingReviewer(hearingId, userId);
            Assert.True(result.Equals(hearingId == userHearingRoleHearingId));
        }

        [Test]
        public async Task UserHearingRoleExists_ReturnsCorrect(
            [Values(null, 5)] int? hearingId,
            [Values(HearingRoleEnum.HEARING_OWNER, HearingRoleEnum.HEARING_INVITEE, HearingRoleEnum.HEARING_REVIEWER,
                HearingRoleEnum.HEARING_RESPONDER)]
            HearingRoleEnum role,
            [Values] bool userIdSpecified,
            [Values] bool roleExists)
        {
            const int hearingRoleId = 1;
            const int userId = 7;
            
            var hearingRole = new HearingRole
            {
                Id = hearingRoleId,
                Role = role
            };

            var userHearingRoles = new List<UserHearingRole>
            {
                new UserHearingRole
                {
                    Id = 1,
                    HearingId = hearingId ?? 1,
                    HearingRoleId = roleExists ? hearingRoleId : hearingRoleId + 1,
                    UserId = userId
                }
            };

            if (hearingId.HasValue)
            {
                userHearingRoles.Add(new UserHearingRole
                {
                    Id = 2,
                    HearingId = hearingId.Value + 1,
                    HearingRoleId = hearingRoleId,
                    UserId = userId
                });
            }
            
            _currentUserServiceMock.Setup(userService => userService.DatabaseUserId).Returns(userId);
            _hearingRoleResolverMock.Setup(resolver => resolver.GetHearingRole(role)).ReturnsAsync(hearingRole);
            _userHearingRoleDaoMock
                .Setup(dao => dao.GetAllAsync(null, userHearingRole => userHearingRole.UserId == userId))
                .ReturnsAsync(userHearingRoles);

            bool result = await _resolver.UserHearingRoleExists(hearingId, role, userIdSpecified ? (int?)userId : null);
            
            Assert.That(result, Is.EqualTo(roleExists));
        }
    }
}
