using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.DAOs.Security;
using BallerupKommune.Models.Common;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using BallerupKommune.Operations.Common.Interfaces.Security;
using BallerupKommune.Operations.Models.Hearings.Queries.GetHearing;
using BallerupKommune.Operations.Resolvers;
using FluentAssertions;
using MediatR;
using Moq;
using NovaSec.Compiler;
using NUnit.Framework;
using CommentType = BallerupKommune.Models.Enums.CommentType;

namespace BallerupKommune.DAOs.UnitTests.Security
{
    public class SecurityExpressionsTest
    {
        private Mock<ICurrentUserService> _currentUserServiceMock;
        private Mock<IUserDao> _userDaoMock;
        private Mock<IHearingDao> _hearingDaoMock;
        private Mock<IHearingRoleDao> _hearingRoleDaoMock;
        private ISecurityExpressions _securityExpression;
        private Mock<ISender> _mediatRMock;
        private Mock<ICommentDao> _commentDaoMock;
        private Mock<IHearingRoleResolver> _hearingRoleResovler;
        private Mock<ISecurityExpressionRoot> _securityExpressionRootMock;
        private Mock<IHearingAccessResolver> _hearingAccessResolverMock;
        private Mock<IUserHearingRoleResolver> _userHearingRoleMock;
        private Mock<ICompanyHearingRoleResolver> _companyHearingRoleResolverMock;

        [SetUp]
        public void SetUp()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _userDaoMock = new Mock<IUserDao>();
            _hearingDaoMock = new Mock<IHearingDao>();
            _hearingRoleDaoMock = new Mock<IHearingRoleDao>();
            _mediatRMock = new Mock<ISender>();
            _commentDaoMock = new Mock<ICommentDao>();
            _hearingRoleResovler = new Mock<IHearingRoleResolver>();
            _securityExpressionRootMock = new Mock<ISecurityExpressionRoot>();
            _hearingAccessResolverMock = new Mock<IHearingAccessResolver>();
            _userHearingRoleMock = new Mock<IUserHearingRoleResolver>();
            _companyHearingRoleResolverMock = new Mock<ICompanyHearingRoleResolver>();
            _securityExpression = new SecurityExpressions(_currentUserServiceMock.Object, _userDaoMock.Object,
                _hearingDaoMock.Object, _hearingRoleDaoMock.Object, _commentDaoMock.Object, _hearingAccessResolverMock.Object,
                _userHearingRoleMock.Object, _companyHearingRoleResolverMock.Object);

            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns(string.Empty);
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void SecurityExpressions_IsCurrentUser_ExistingUser(int id)
        {
            var currentUserId = 10;
            var shouldFail = currentUserId != id;

            _userDaoMock.Setup(x => x.FindUserByIdentifier(It.IsAny<string>(), null))
                .Returns(Task.FromResult(new User
                {
                    Id = currentUserId
                }));

            var result = _securityExpression.IsCurrentUser(id);

            if (shouldFail)
            {
                Assert.IsFalse(result);
            }
            else
            {
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void SecurityExpressions_IsCurrentUser_NonExistingUser()
        {
            var id = 10;
            _userDaoMock.Setup(x => x.FindUserByIdentifier(It.IsAny<string>(), null))
                .Returns(Task.FromResult(default(User)));

            var result = _securityExpression.IsCurrentUser(id);

            Assert.IsFalse(result);
        }

        [Test]
        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(99,1)]
        [TestCase(null, 1)]
        public void SecurityExpressions_IsCommentOwnerByCommentId(int? userId, int commentUserId)
        {
            var shouldSucceed = userId == commentUserId;

            var comment = new Comment
            {
                Id = 0,
                User = new User
                {
                    Id = commentUserId
                }
            };
            
            _currentUserServiceMock.Setup(x => x.DatabaseUserId)
                .Returns(userId);
            
            _commentDaoMock.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IncludeProperties>()))
                .Returns(Task.FromResult(comment));

            var result = _securityExpression.IsCommentOwnerByCommentId(commentUserId);
            Assert.IsTrue(result == shouldSucceed);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SecurityExpressions_IsHearingOwner_ReturnsCorrect(bool hasRole)
        {
            const int hearingId = 7;

            _userHearingRoleMock
                .Setup(resolver =>
                    resolver.IsHearingOwner(hearingId,null))
                .ReturnsAsync(hasRole);

            bool result = _securityExpression.IsHearingOwner(hearingId);
            
            Assert.That(result, Is.EqualTo(hasRole));
        }
    
        [Test]
        [TestCase(true)]
        [TestCase(false)]

        public void SecurityExpressions_IsHearingReviewer_ReturnsCorrect(bool hasRole)
        {
            const int hearingId = 7;

            _userHearingRoleMock
                .Setup(resolver =>
                    resolver.IsHearingReviewer(hearingId ,null))
                .ReturnsAsync(hasRole);

            bool result = _securityExpression.IsHearingReviewer( hearingId);
            
            Assert.That(result, Is.EqualTo(hasRole));
        }
        
        [Test]
        public void SecurityExpressions_HasRoleOnAnyHearing_ReturnsCorrect(
            [Values(
                BallerupKommune.Models.Enums.HearingRole.HEARING_OWNER,
                BallerupKommune.Models.Enums.HearingRole.HEARING_INVITEE,
                BallerupKommune.Models.Enums.HearingRole.HEARING_RESPONDER,
                BallerupKommune.Models.Enums.HearingRole.HEARING_REVIEWER
                )] BallerupKommune.Models.Enums.HearingRole role, 
            [Values] bool hasRole)
        {
            _userHearingRoleMock
                .Setup(resolver => resolver.UserHearingRoleExists(null, role ,null))
                .ReturnsAsync(hasRole);

            _companyHearingRoleResolverMock.Setup(resolver => resolver.CompanyHearingRoleExist(null, role, null))
                .ReturnsAsync(hasRole);

            bool result = _securityExpression.HasRoleOnAnyHearing(role);
            
            Assert.That(result, Is.EqualTo(hasRole));
        }
        
        [Test]
        [TestCase(true, false)]
        [TestCase(false, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        public void SecurityExpressions_IsHearingInvitee_ReturnsCorrect(bool hasRole, bool companyHasRole)
        {
            const int hearingId = 7;
            
            var hearing = new Hearing {Id = hearingId};

            _userHearingRoleMock
                .Setup(resolver =>
                    resolver.UserHearingRoleExists(hearing.Id,
                        BallerupKommune.Models.Enums.HearingRole.HEARING_INVITEE, null)).ReturnsAsync(hasRole);
            _companyHearingRoleResolverMock
                .Setup(resolver => resolver.CompanyHearingRoleExist(hearing.Id,
                    BallerupKommune.Models.Enums.HearingRole.HEARING_INVITEE, null)).ReturnsAsync(companyHasRole);


            bool result = _securityExpression.IsHearingInvitee(hearing);
            
            Assert.That(result, Is.EqualTo(hasRole || companyHasRole));
            _hearingDaoMock.Verify();
            _hearingDaoMock.VerifyNoOtherCalls();
        }
        
        [Test]
        [TestCase(true, false)]
        [TestCase(false, false)]
        [TestCase(true, true)]
        [TestCase(false, true)]
        public void SecurityExpressions_IsHearingResponder_ReturnsCorrect(bool hasRole, bool companyHasRole)
        {
            const int hearingId = 7;
            
            var hearing = new Hearing {Id = hearingId};

            _hearingDaoMock
                .Setup(dao => dao.GetAsync(hearingId, It.IsAny<IncludeProperties>()))
                .ReturnsAsync(hearing)
                .Verifiable();

            _userHearingRoleMock
                .Setup(resolver =>
                    resolver.UserHearingRoleExists(hearing.Id,
                        BallerupKommune.Models.Enums.HearingRole.HEARING_RESPONDER, null)).ReturnsAsync(hasRole);
            _companyHearingRoleResolverMock
                .Setup(resolver => resolver.CompanyHearingRoleExist(hearing.Id,
                    BallerupKommune.Models.Enums.HearingRole.HEARING_RESPONDER, null)).ReturnsAsync(companyHasRole);

            bool result = _securityExpression.IsHearingResponder(hearingId);
            
            Assert.That(result, Is.EqualTo(hasRole || companyHasRole));
            _hearingDaoMock.Verify();
            _hearingDaoMock.VerifyNoOtherCalls();
        }

        [Test]
        [TestCase(BallerupKommune.Models.Enums.HearingStatus.ACTIVE)]
        [TestCase(BallerupKommune.Models.Enums.HearingStatus.DRAFT)]
        [TestCase(BallerupKommune.Models.Enums.HearingStatus.AWAITING_STARTDATE)]
        [TestCase(BallerupKommune.Models.Enums.HearingStatus.ACTIVE)]
        [TestCase(BallerupKommune.Models.Enums.HearingStatus.AWAITING_CONCLUSION)]
        [TestCase(BallerupKommune.Models.Enums.HearingStatus.CONCLUDED)]
        public void SecurityExpressions_IsHearingPublished_ExistingHearing(BallerupKommune.Models.Enums.HearingStatus statusToTest)
        {
            var hearing = new Hearing
            {
                HearingStatus = new BallerupKommune.Models.Models.HearingStatus
                {
                    Status = statusToTest
                }
            };

            var shouldFail = statusToTest == BallerupKommune.Models.Enums.HearingStatus.CREATED ||
                             statusToTest == BallerupKommune.Models.Enums.HearingStatus.DRAFT;

            var result = _securityExpression.IsHearingPublished(hearing);

            Assert.IsFalse(result == shouldFail);
        }

        [Test]
        public void SecurityExpressions_IsHearingPublished_NonExistingHearing()
        {
            var hearing = default(Hearing);
            var result = _securityExpression.IsHearingPublished(hearing);

            Assert.IsFalse(result);
        }

        [Test]
        [TestCase(BallerupKommune.Models.Enums.HearingRole.HEARING_INVITEE)]
        [TestCase(BallerupKommune.Models.Enums.HearingRole.HEARING_RESPONDER)]
        [TestCase(BallerupKommune.Models.Enums.HearingRole.HEARING_REVIEWER)]
        [TestCase(BallerupKommune.Models.Enums.HearingRole.HEARING_OWNER)]
        public void SecurityExpressions_IsHearingOwnerRole_ExistingHearingRole(BallerupKommune.Models.Enums.HearingRole roleToReturn)
        {
            _hearingRoleDaoMock.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IncludeProperties>()))
                .Returns(Task.FromResult(new HearingRole
                {
                    Role = roleToReturn
                }));
            
            var expectedResult = roleToReturn == BallerupKommune.Models.Enums.HearingRole.HEARING_OWNER;
            var result = _securityExpression.IsHearingOwnerRole(10);

            Assert.IsTrue(expectedResult == result);
        }

        [Test]
        public void SecurityExpressions_IsHearingOwnerRole_NonExistingHearingRole()
        {
            _hearingRoleDaoMock.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IncludeProperties>()))
                .Returns(Task.FromResult(default(HearingRole)));

            var result = _securityExpression.IsHearingOwnerRole(10);

            Assert.IsFalse(result);
        }

        [Test]
        public void SecurityExpressions_IsInternalHearing_NoHearing_ShouldNotThrow()
        {
            _hearingDaoMock.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IncludeProperties>()))
                .Returns(Task.FromResult(default(Hearing)));

            FluentActions.Invoking(() => _securityExpression.IsInternalHearing(null)).Should().NotThrow();
        }

        [Test]
        public void SecurityExpressions_IsInternalHearing_NoHearingType_ShouldNotThrow()
        {
            _hearingDaoMock.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IncludeProperties>()))
                .Returns(Task.FromResult(new Hearing()));

            var hearing = new Hearing();
            FluentActions.Invoking(() => _securityExpression.IsInternalHearing(hearing)).Should().NotThrow();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SecurityExpressions_IsInternalHearing(bool testResult)
        {
            var hearing = new Hearing
            {
                HearingType = new HearingType
                {
                    IsInternalHearing = testResult
                }
            };
            
            _hearingDaoMock.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IncludeProperties>()))
                .Returns(Task.FromResult(hearing));

            var result = _securityExpression.IsInternalHearing(hearing);
            Assert.IsTrue(result == testResult);
        }


        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanSeeHearing_Test(bool seesHearing)
        {
            var hearings = new List<Hearing>
            {
                new Hearing
                {
                    Id = 1,
                    UserHearingRoles = new List<UserHearingRole>
                    {
                        new UserHearingRole
                        {
                            Id = 1,
                            UserId = 1,
                            HearingId = 1,
                            HearingRoleId = 1
                        },
                        new UserHearingRole
                        {
                            Id = 2,
                            UserId = 1,
                            HearingId = 1,
                            HearingRoleId = 2
                        }
                    }
                }

            };
            int hearingId = 1;
            _mediatRMock.Setup(x => x.Send(It.IsAny<GetHearingQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(seesHearing ? new Hearing() : null));
            _hearingAccessResolverMock.Setup(resolver => resolver.CanSeeHearingById(hearingId)).ReturnsAsync(seesHearing);
            var result = _securityExpression.CanSeeHearing(hearingId);
            Assert.IsTrue(result == seesHearing);
        }

        [Test]
        [TestCase(BallerupKommune.Models.Enums.CommentStatus.APPROVED)]
        [TestCase(BallerupKommune.Models.Enums.CommentStatus.AWAITING_APPROVAL)]
        public void IsCommentApproved_Test(BallerupKommune.Models.Enums.CommentStatus commentStatus)
        {
            var newComment = new Comment
            {
                Id = 1,
                Hearing = new Hearing(),
                ContainsSensitiveInformation = false,
                CommentType = new BallerupKommune.Models.Models.CommentType
                {
                    Type = CommentType.HEARING_RESPONSE
                },
                CommentStatus = new CommentStatus
                {
                    Status = commentStatus
                }
            };
            
            _commentDaoMock.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<IncludeProperties>()))
                .Returns(Task.FromResult(newComment));

            var result = _securityExpression.IsCommentApproved(newComment);
            var isApproved = commentStatus == BallerupKommune.Models.Enums.CommentStatus.APPROVED;
            var expected = isApproved == result;

            Assert.IsTrue(expected);
        }

        [Test]
        public void IsCommentApproved_Test_NoCommentStatus_ShouldThrow()
        {
            var comment = new Comment
            {
                CommentStatus = null
            };
            FluentActions.Invoking(() => _securityExpression.IsCommentApproved(comment)).Should().Throw<ArgumentException>();
        }

        [Test]
        [TestCase(CommentType.HEARING_RESPONSE_REPLY)]
        [TestCase(CommentType.HEARING_RESPONSE)]
        [TestCase(CommentType.HEARING_REVIEW)]
        [TestCase(CommentType.NONE)]
        public void IsCommentResponseReply_Test(CommentType commentType)
        {
            var comment = new Comment
            {
                CommentType = new BallerupKommune.Models.Models.CommentType
                {
                    Type = commentType
                }
            };

            var result = _securityExpression.IsCommentResponseReply(comment);
            var expectedResult = commentType == CommentType.HEARING_RESPONSE_REPLY;
            Assert.IsTrue(result == expectedResult);
        }
        
        [TestCase]
        public void IsCommentResponseReply_Test_NoCommentType_ShouldThrow()
        {
            var comment = new Comment
            {
                CommentType = null
            };
            FluentActions.Invoking(() => _securityExpression.IsCommentResponseReply(comment)).Should().Throw<ArgumentException>();
        }
    }
}
