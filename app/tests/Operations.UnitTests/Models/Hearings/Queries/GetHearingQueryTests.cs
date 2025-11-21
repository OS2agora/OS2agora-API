using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Models;
using Agora.Operations.Common.Constants;
using Agora.Operations.Models.Hearings.Queries.GetHearing;
using Moq;
using NUnit.Framework;
using HearingRole = Agora.Models.Enums.HearingRole;
using HearingStatus = Agora.Models.Enums.HearingStatus;

namespace Agora.Operations.UnitTests.Models.Hearings.Queries
{
    public class GetHearingQueryTests : ModelsTestBase<GetHearingQuery, Hearing>
    {
        public GetHearingQueryTests()
        {
            SecurityExpressionRoot
                .Setup(x => x.HasAnyRole(It.IsAny<List<string>>()))
                .Returns((List<string> param) => param.Any(x => SecurityExpressionRoot.Object.HasRole(x)));
        }

        [SetUp]
        public void SetUp()
        {
            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(GetHandlerResult()));

            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns(false);

            SecurityExpressionsMock.Setup(x => x.IsHearingOwner(It.IsAny<int>()))
                .Returns(false);

            SecurityExpressionsMock.Setup(x => x.IsHearingInvitee(It.IsAny<Hearing>()))
                .Returns(false);

            SecurityExpressionsMock.Setup(x => x.IsHearingReviewer(It.IsAny<int>()))
                .Returns(false);

            SecurityExpressionsMock.Setup(x => x.IsHearingPublished(It.IsAny<Hearing>()))
                .Returns(false);
        }

        [Test]
        public async Task GetHearing_Administrator_Returns_All()
        {
            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string role) => role == Security.Roles.Administrator);

            var request = new GetHearingQuery();

            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.IsTrue(result.UserHearingRoles.Any() &&
                          result.UserHearingRoles.All(uhr => uhr.HearingRole != null && uhr.User != null) &&
                          result.HearingType.FieldTemplates != null &&
                          result.HearingType.HearingTemplate.Fields.All(f =>
                              f.FieldTemplates.Any() && f.FieldType != null));

        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task GetHearing_IsHearingOwner_Returns_All(bool isHearingOwner)
        {
            SecurityExpressionsMock.Setup(x => x.IsHearingOwner(It.IsAny<int>()))
                    .Returns(isHearingOwner);

            var request = new GetHearingQuery();

            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);
            if (!isHearingOwner)
            {
                Assert.IsNull(result);
            }
            else
            {
                Assert.IsTrue(result.UserHearingRoles.Any() &&
                              result.UserHearingRoles.All(uhr => uhr.HearingRole != null && uhr.User != null) &&
                              result.HearingType.FieldTemplates != null &&
                              result.HearingType.HearingTemplate.Fields.All(f =>
                                  f.FieldTemplates.Any() && f.FieldType != null));
            }

        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task GetHearing_IsHearingReviewer_Returns_All(bool isHearingReviewer)
        {
            SecurityExpressionsMock.Setup(x => x.IsHearingOwner(It.IsAny<int>()))
                .Returns(isHearingReviewer);

            var request = new GetHearingQuery();

            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);
            if (!isHearingReviewer)
            {
                Assert.IsNull(result);
            }
            else
            {
                Assert.IsTrue(result.UserHearingRoles.Any() &&
                              result.UserHearingRoles.All(uhr => uhr.HearingRole != null && uhr.User != null) &&
                              result.HearingType.FieldTemplates != null &&
                              result.HearingType.HearingTemplate.Fields.All(f =>
                                  f.FieldTemplates.Any() && f.FieldType != null));
            }

        }

        [Test]
        [TestCase(true,true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public async Task GetHearing_IsHearingInvitee_IsHearingPublished(bool isInvitee, bool hearingIsPublished)
        {
            SecurityExpressionsMock.Setup(x => x.IsHearingInvitee(It.IsAny<Hearing>()))
                .Returns(isInvitee);

            SecurityExpressionsMock.Setup(x => x.IsHearingPublished(It.IsAny<Hearing>()))
                .Returns(hearingIsPublished);

            var request = new GetHearingQuery();

            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            if (!isInvitee || !hearingIsPublished)
            {
                Assert.IsNull(result);
            }
            else
            {
                Assert.IsTrue(result.UserHearingRoles == null
                              && result.HearingType.FieldTemplates == null
                              && result.KleHierarchy != null
                              && result.HearingType.HearingTemplate.Fields.All(f =>
                                  f.FieldTemplates == null && f.FieldType != null));
            }
        }

        [Test]
        [TestCase(false, false, false, false)]
        [TestCase(true, false, false, false)]
        [TestCase(false, false, false, true)]
        [TestCase(true, false, false, true)]
        [TestCase(false, false, true, false)]
        [TestCase(true, false, true, false)]
        [TestCase(false, false, true, true)]
        [TestCase(true, false, true, true)]
        [TestCase(false, true, false, false)]
        [TestCase(true, true, false, false)]
        [TestCase(false, true, false, true)]
        [TestCase(true, true, false, true)]
        [TestCase(false, true, true, false)]
        [TestCase(true, true, true, false)]
        [TestCase(false, true, true, true)]
        [TestCase(true, true, true, true)]
        public async Task GetHearing_Employee(bool isEmployee, bool isPublished, bool isInternal, bool isClosed)
        {
            var hearingResponse = GetHandlerResult(isClosed,isInternal,isPublished);
            if (isEmployee)
            {
                SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                    .Returns((string role) => role == Security.Roles.Employee);
            }
            SecurityExpressionsMock.Setup(x => x.IsHearingPublished(It.IsAny<Hearing>()))
                .Returns(isPublished);

            SecurityExpressionsMock.Setup(x => x.IsInternalHearing(It.IsAny<Hearing>()))
                .Returns(isInternal);

            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(hearingResponse));

            var request = new GetHearingQuery();
            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            if (!isEmployee || !isPublished || isClosed)
            {
                Assert.IsNull(result);
            }
            else
            {
                Assert.IsTrue(result.UserHearingRoles == null
                              && !result.ClosedHearing
                              && result.HearingType.FieldTemplates == null
                              && result.HearingType.HearingTemplate.Fields.All(f =>
                                  f.FieldTemplates == null && f.FieldType != null));
            }

        }

        [Test]
        [TestCase(false, false, false, false)]
        [TestCase(true, false, false, false)]
        [TestCase(false, false, false, true)]
        [TestCase(true, false, false, true)]
        [TestCase(false, false, true, false)]
        [TestCase(true, false, true, false)]
        [TestCase(false, false, true, true)]
        [TestCase(true, false, true, true)]
        [TestCase(false, true, false, false)]
        [TestCase(true, true, false, false)]
        [TestCase(false, true, false, true)]
        [TestCase(true, true, false, true)]
        [TestCase(false, true, true, false)]
        [TestCase(true, true, true, false)]
        [TestCase(false, true, true, true)]
        [TestCase(true, true, true, true)]
        public async Task GetHearing_Anonymous(bool isAnonymous, bool isPublished, bool isInternal, bool isClosed)
        {
            var hearingResponse = GetHandlerResult(isClosed, isInternal, isPublished);
            if (isAnonymous)
            {
                SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                    .Returns((string role) => role == Security.Roles.Anonymous);
            }
            SecurityExpressionsMock.Setup(x => x.IsHearingPublished(It.IsAny<Hearing>()))
                .Returns(isPublished);

            SecurityExpressionsMock.Setup(x => x.IsInternalHearing(It.IsAny<Hearing>()))
                .Returns(isInternal);

            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(hearingResponse));

            var request = new GetHearingQuery();
            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            if (!isAnonymous || !isPublished || isInternal || isClosed)
            {
                Assert.IsNull(result);
            }
            else
            {
                Assert.IsTrue(result.UserHearingRoles == null
                              && !result.ClosedHearing
                              && !result.HearingType.IsInternalHearing
                              && result.HearingType.FieldTemplates == null
                              && result.HearingType.HearingTemplate.Fields.All(f =>
                                  f.FieldTemplates == null && f.FieldType != null));
            }
        }

        [Test]
        [TestCase(false, false, false, false)]
        [TestCase(true, false, false, false)]
        [TestCase(false, false, false, true)]
        [TestCase(true, false, false, true)]
        [TestCase(false, false, true, false)]
        [TestCase(true, false, true, false)]
        [TestCase(false, false, true, true)]
        [TestCase(true, false, true, true)]
        [TestCase(false, true, false, false)]
        [TestCase(true, true, false, false)]
        [TestCase(false, true, false, true)]
        [TestCase(true, true, false, true)]
        [TestCase(false, true, true, false)]
        [TestCase(true, true, true, false)]
        [TestCase(false, true, true, true)]
        [TestCase(true, true, true, true)]
        public async Task GetHearing_Citizen(bool isCitizen, bool isPublished, bool isInternal, bool isClosed)
        {
            var hearingResponse = GetHandlerResult(isClosed, isInternal, isPublished);
            if (isCitizen)
            {
                SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                    .Returns((string role) => role == Security.Roles.Citizen);
            }
            SecurityExpressionsMock.Setup(x => x.IsHearingPublished(It.IsAny<Hearing>()))
                .Returns(isPublished);

            SecurityExpressionsMock.Setup(x => x.IsInternalHearing(It.IsAny<Hearing>()))
                .Returns(isInternal);

            RequestHandlerDelegateMock
                .Setup(x => x())
                .Returns(Task.FromResult(hearingResponse));

            var request = new GetHearingQuery();
            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            if (!isCitizen || !isPublished || isInternal || isClosed)
            {
                Assert.IsNull(result);
            }
            else
            {
                Assert.IsTrue(result.UserHearingRoles == null
                              && !result.ClosedHearing
                              && !result.HearingType.IsInternalHearing
                              && result.HearingType.FieldTemplates == null
                              && result.HearingType.HearingTemplate.Fields.All(f =>
                                  f.FieldTemplates == null && f.FieldType != null));
            }
        }

        private Hearing GetHandlerResult(bool isClosed=false, bool isInternal=true, bool isPublished=true)
        {
            return new Hearing
            {
                Id = 1,
                ClosedHearing = isClosed,
                KleHierarchy = new KleHierarchy(),
                HearingStatus = new Agora.Models.Models.HearingStatus
                {
                    Status = isPublished?HearingStatus.ACTIVE:HearingStatus.DRAFT
                },
                HearingType = new HearingType
                {
                    IsInternalHearing = isInternal,
                    FieldTemplates = new List<FieldTemplate>
                    {
                        new FieldTemplate()
                    },
                    HearingTemplate = new HearingTemplate
                    {
                        Fields = new List<Field>
                        {
                            new Field
                            {
                                FieldTemplates = new List<FieldTemplate>
                                {
                                    new FieldTemplate()
                                },
                                FieldType = new FieldType()
                            }
                        }
                    }
                },
                UserHearingRoles = new List<UserHearingRole>
                {
                    new UserHearingRole
                    {
                        HearingRole = new Agora.Models.Models.HearingRole
                        {
                            Role = HearingRole.HEARING_OWNER
                        },
                        User = new User
                        {
                            Id = 1
                        }
                    },
                    new UserHearingRole
                    {
                        HearingRole = new Agora.Models.Models.HearingRole
                        {
                            Role = HearingRole.HEARING_INVITEE
                        },
                        User = new User
                        {
                            Id = 2
                        }
                    },
                    new UserHearingRole
                    {
                        HearingRole = new Agora.Models.Models.HearingRole
                        {
                            Role = HearingRole.HEARING_REVIEWER
                        },
                        User = new User
                        {
                            Id = 3
                        }
                    }
                }
            };
        }
    }
}