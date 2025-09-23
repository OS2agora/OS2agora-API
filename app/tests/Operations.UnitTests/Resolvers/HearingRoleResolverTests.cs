using System.Collections.Generic;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.Operations.Resolvers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using HearingRoleEnum = BallerupKommune.Models.Enums.HearingRole;

namespace BallerupKommune.Operations.UnitTests.Resolvers
{
    public class HearingRoleResolverTests
    {
        private HearingRoleResolver _resolver;
        private Mock<IHearingRoleDao> _hearingRoleDaoMock;

        [SetUp]
        public void SetUp()
        {
            _hearingRoleDaoMock = new Mock<IHearingRoleDao>();
            _resolver = new HearingRoleResolver(_hearingRoleDaoMock.Object, NullLogger<HearingRoleResolver>.Instance);
        }

        [Test]
        public async Task GetHearingRole_GetsHearingRole()
        {
            var hearingOwnerRole = new HearingRole
            {
                Id = 1, 
                Role = HearingRoleEnum.HEARING_OWNER, 
                Name = nameof(HearingRoleEnum.HEARING_OWNER)
            };
            var hearingInviteeRole = new HearingRole
            {
                Id = 2, 
                Role = HearingRoleEnum.HEARING_INVITEE, 
                Name = nameof(HearingRoleEnum.HEARING_INVITEE)
            };
            var hearingRoles = new List<HearingRole> {hearingOwnerRole, hearingInviteeRole};

            _hearingRoleDaoMock
                .Setup(dao => dao.GetAllAsync(It.IsAny<IncludeProperties>()))
                .ReturnsAsync(hearingRoles);

            HearingRole hearingOwnerResult = await _resolver.GetHearingRole(HearingRoleEnum.HEARING_OWNER);
            HearingRole hearingInviteeResult = await _resolver.GetHearingRole(HearingRoleEnum.HEARING_INVITEE);
            
            Assert.Multiple(() =>
            {
                Assert.That(hearingOwnerResult, Is.EqualTo(hearingOwnerRole));
                Assert.That(hearingInviteeResult, Is.EqualTo(hearingInviteeRole));
            });
        }
        
        [Test]
        public async Task GetHearingRoles_GetsHearingRoles()
        {
            var hearingOwnerRole = new HearingRole
            {
                Id = 1, 
                Role = HearingRoleEnum.HEARING_OWNER, 
                Name = nameof(HearingRoleEnum.HEARING_OWNER)
            };
            var hearingInviteeRole = new HearingRole
            {
                Id = 2, 
                Role = HearingRoleEnum.HEARING_INVITEE, 
                Name = nameof(HearingRoleEnum.HEARING_INVITEE)
            };
            var hearingResponderRole = new HearingRole
            {
                Id = 3, 
                Role = HearingRoleEnum.HEARING_RESPONDER, 
                Name = nameof(HearingRoleEnum.HEARING_RESPONDER)
            };
            var hearingRoles = new List<HearingRole> {hearingOwnerRole, hearingInviteeRole, hearingResponderRole};

            _hearingRoleDaoMock
                .Setup(dao => dao.GetAllAsync(It.IsAny<IncludeProperties>()))
                .ReturnsAsync(hearingRoles);

            List<HearingRole> result =
                await _resolver.GetHearingRoles(HearingRoleEnum.HEARING_OWNER, HearingRoleEnum.HEARING_RESPONDER);
            
            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(2));
                Assert.That(result, Has.Exactly(1).Items.EqualTo(hearingOwnerRole));
                Assert.That(result, Has.Exactly(1).Items.EqualTo(hearingResponderRole));
            });
        }
        
        [Test]
        public async Task GetHearingRole_MultipleCalls_OnlyCallsDataAccessLayerOnce()
        {
            var hearingOwnerRole = new HearingRole
            {
                Id = 1, 
                Role = HearingRoleEnum.HEARING_OWNER, 
                Name = nameof(HearingRoleEnum.HEARING_OWNER)
            };
            var hearingInviteeRole = new HearingRole
            {
                Id = 2, 
                Role = HearingRoleEnum.HEARING_INVITEE, 
                Name = nameof(HearingRoleEnum.HEARING_INVITEE)
            };
            var hearingRoles = new List<HearingRole> {hearingOwnerRole, hearingInviteeRole};

            _hearingRoleDaoMock
                .Setup(dao => dao.GetAllAsync(It.IsAny<IncludeProperties>()))
                .ReturnsAsync(hearingRoles)
                .Verifiable();

            await _resolver.GetHearingRole(HearingRoleEnum.HEARING_OWNER);
            await _resolver.GetHearingRole(HearingRoleEnum.HEARING_INVITEE);
            
            _hearingRoleDaoMock.Verify();
            _hearingRoleDaoMock.VerifyNoOtherCalls();
        }
    }
}