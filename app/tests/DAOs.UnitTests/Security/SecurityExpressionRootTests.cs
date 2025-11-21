using System;
using System.Linq;
using System.Security.Claims;
using Agora.DAOs.Security;
using Agora.Operations.Common.Enums;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Resolvers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NovaSec.Compiler;
using NUnit.Framework;

namespace Agora.DAOs.UnitTests.Security
{
    public class SecurityExpressionRootTests
    {

        private class TestPrincipal : ClaimsPrincipal
        {
            public TestPrincipal(params Claim[] claims) : base(new TestIdentity(claims))
            {
            }
        }

        private class TestIdentity : ClaimsIdentity
        {
            public TestIdentity(params Claim[] claims) : base(claims)
            {
            }
        }

        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<IUserHearingRoleResolver> _userHearingRoleResolverMock;
        private Mock<ICompanyHearingRoleResolver> _companyHearingRoleResolverMock;
        private ISecurityExpressionRoot _securityExpressionRoot;

        [SetUp]
        public void SetUp()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _userHearingRoleResolverMock = new Mock<IUserHearingRoleResolver>();
            _companyHearingRoleResolverMock = new Mock<ICompanyHearingRoleResolver>();
            _securityExpressionRoot = new SecurityExpressionRoot(_currentUserServiceMock.Object, _httpContextAccessorMock.Object, 
                _userHearingRoleResolverMock.Object, _companyHearingRoleResolverMock.Object);

            _currentUserServiceMock.Setup(service => service.UserId)
                .Returns(string.Empty);
        }

        [Test]
        [TestCase("Administrator", "")]
        [TestCase("Administrator", "Administrator")]
        [TestCase("Administrator", "HearingCreator")]
        [TestCase("Administrator", "Citizen")]
        [TestCase("Administrator", "Administrator, Citizen")]
        [TestCase("Administrator", "Citizen, Administrator")]
        [TestCase("Administrator", "Citizen, Administrator, HearingCreator")]
        [TestCase("Administrator", "HearingCreator, Citizen, Administrator")]
        [TestCase("HearingCreator", "")]
        [TestCase("HearingCreator", "Administrator")]
        [TestCase("HearingCreator", "HearingCreator")]
        [TestCase("HearingCreator", "Citizen")]
        public void HasRole_IdpRoles_ExistingUser(string roleToTest, string userRoles)
        {
            var userRolesArray = userRoles.Split(',');
            var shouldSucceed = userRolesArray.Contains(roleToTest);
            var claims = userRolesArray.Select(role => new Claim(ClaimTypes.Role, role)).ToArray();

            TestPrincipal currentPrincipal = new TestPrincipal(claims);
            _httpContextAccessorMock.SetupProperty(httpContextAccessor => httpContextAccessor.HttpContext.User, currentPrincipal);
            var result = _securityExpressionRoot.HasRole(roleToTest);
            Assert.That(result, Is.EqualTo(shouldSucceed));
        }

        [Test]
        public void HasRole_HearingContextualRoles_ExistingUser(
            [Values("HearingOwner", "HearingResponder", "HearingInvitee", "HearingReviewer")] string roleToTest,
            [Values] bool isOwner, [Values] bool isInvitee, [Values] bool isReviewer, [Values] bool isResponder, [Values] bool isNone)
        {
            bool shouldSucceed = false;
            if (roleToTest == "HearingOwner")
            {
                shouldSucceed = isOwner;
            }
            else if (roleToTest == "HearingResponder")
            {
                shouldSucceed = isResponder;
            }
            else if (roleToTest == "HearingInvitee")
            {
                shouldSucceed = isInvitee;
            }
            else if (roleToTest == "HearingReviewer")
            {
                shouldSucceed = isReviewer;
            }

            _currentUserServiceMock.Setup(service => service.DatabaseUserId).Returns(73);
            _userHearingRoleResolverMock.Setup(resolver => resolver.IsHearingOwner(null, null)).ReturnsAsync(isOwner);
            _userHearingRoleResolverMock.Setup(resolver => resolver.IsHearingResponder(null, null)).ReturnsAsync(isResponder);
            _userHearingRoleResolverMock.Setup(resolver => resolver.IsHearingInvitee(null, null)).ReturnsAsync(isInvitee);
            _userHearingRoleResolverMock.Setup(resolver => resolver.IsHearingReviewer(null, null)).ReturnsAsync(isReviewer);

            _companyHearingRoleResolverMock.Setup(resolver => resolver.IsHearingResponder(null, null)).ReturnsAsync(isResponder);
            _companyHearingRoleResolverMock.Setup(resolver => resolver.IsHearingInvitee(null, null)).ReturnsAsync(isInvitee);

            var result = _securityExpressionRoot.HasRole(roleToTest);

            Assert.That(result, Is.EqualTo(shouldSucceed));
        }

        [Test]
        [TestCase("HearingOwner")]
        [TestCase("HearingResponder")]
        [TestCase("HearingInvitee")]
        [TestCase("HearingReviewer")]
        public void HasRole_HearingContextualRoles_NonExistingUser(string roleToTest)
        {
            var result = _securityExpressionRoot.HasRole(roleToTest);
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("Anonymous", null, true)]
        [TestCase("Anonymous", AuthenticationMethod.NONE, false)]
        [TestCase("Anonymous", AuthenticationMethod.MitIdCitizen, false)]
        [TestCase("Anonymous", AuthenticationMethod.MitIdErhverv, false)]
        [TestCase("Anonymous", AuthenticationMethod.AdfsEmployee, false)]
        [TestCase("Citizen", AuthenticationMethod.MitIdCitizen, true)]
        [TestCase("Citizen", AuthenticationMethod.MitIdErhverv, true)]
        [TestCase("Citizen", AuthenticationMethod.NONE, false)]
        [TestCase("Citizen", AuthenticationMethod.AdfsEmployee, false)]
        [TestCase("Citizen", null, false)]
        [TestCase("Employee", AuthenticationMethod.AdfsEmployee, true)]
        [TestCase("Employee", AuthenticationMethod.NONE, false)]
        [TestCase("Employee", AuthenticationMethod.MitIdCitizen, false)]
        [TestCase("Employee", AuthenticationMethod.MitIdErhverv, false)]
        [TestCase("Employee", null, false)]
        public void HasRole_LoginMethodRoles(string roleToTest, AuthenticationMethod? authMethod, bool shouldSucceed)
        {
            _currentUserServiceMock.Setup(service => service.AuthenticationMethod)
                .Returns(authMethod);

            var result = _securityExpressionRoot.HasRole(roleToTest);
            Assert.That(result, Is.EqualTo(shouldSucceed));
        }

        [Test]
        [TestCase("InvalidRole")]
        [TestCase("")]
        [TestCase(null)]
        public void HasRole_InvalidRole_ThrowsException(string invalidRole)
        {
            FluentActions.Invoking(() => _securityExpressionRoot.HasRole(invalidRole)).Should().Throw<Exception>();
        }
    }
}
