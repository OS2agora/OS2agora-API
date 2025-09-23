using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Constants;
using BallerupKommune.Operations.Models.Hearings.Queries.GetHearings;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using BallerupKommune.TestUtilities.Extensions;

namespace BallerupKommune.Operations.UnitTests.Models.Hearings.Queries
{
    public class GetHearingsQueryTests : ModelsTestBase<GetHearingsQuery, List<Hearing>>
    {
        public GetHearingsQueryTests()
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
        public async Task GetHearings_Administrator_Returns_All()
        {
            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string role) => role == Security.Roles.Administrator);

            var request = new GetHearingsQuery();

            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            Assert.IsTrue(result.Count == GetHandlerResult().Count);
            Assert.IsTrue(result.All(x => x.UserHearingRoles.Any() && 
                                          x.UserHearingRoles.All(uhr => uhr.HearingRole != null && uhr.User != null) &&
                                          x.HearingType.FieldTemplates != null &&
                                          x.HearingType.HearingTemplate.Fields.All(f => f.FieldTemplates.Any() && f.FieldType != null) &&
                                          x.KleHierarchy != null));
        }

        [Test]
        [TestCase(1, BallerupKommune.Models.Enums.HearingRole.HEARING_OWNER)]
        [TestCase(2, BallerupKommune.Models.Enums.HearingRole.HEARING_OWNER)]
        [TestCase(3, BallerupKommune.Models.Enums.HearingRole.HEARING_OWNER)]
        [TestCase(1, BallerupKommune.Models.Enums.HearingRole.HEARING_REVIEWER)]
        [TestCase(2, BallerupKommune.Models.Enums.HearingRole.HEARING_REVIEWER)]
        [TestCase(3, BallerupKommune.Models.Enums.HearingRole.HEARING_REVIEWER)]
        public async Task GetHearings_IsHearingOwner_Or_HearingReviewer(int userId, BallerupKommune.Models.Enums.HearingRole roleToTest)
        {
            var hearingResponse = GetHandlerResult();
            if (roleToTest == BallerupKommune.Models.Enums.HearingRole.HEARING_OWNER)
            {
                SecurityExpressionsMock.Setup(x => x.IsHearingOwner(It.IsAny<int>()))
                    .Returns((int hearingId) => hearingResponse.ContainsHearingInRole(hearingId,userId,roleToTest));
            }

            if (roleToTest == BallerupKommune.Models.Enums.HearingRole.HEARING_REVIEWER)
            {
                SecurityExpressionsMock.Setup(x => x.IsHearingReviewer(It.IsAny<int>()))
                    .Returns((int hearingId) => hearingResponse.ContainsHearingInRole(hearingId, userId, roleToTest));
            }
            

            var request = new GetHearingsQuery();

            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            var expectedResult = GetHandlerResult().Where(x => x.UserHearingRoles.Any(uhr =>
                uhr.User.Id == userId &&
                uhr.HearingRole.Role == roleToTest)).ToList();
            
            Assert.IsTrue(result.Count == expectedResult.Count);
            Assert.IsTrue(result.All(x => x.UserHearingRoles.Any() &&
                                          x.UserHearingRoles.All(uhr => uhr.HearingRole != null && uhr.User != null) &&
                                          x.HearingType.FieldTemplates != null &&
                                          x.HearingType.HearingTemplate.Fields.All(f => f.FieldTemplates.Any() && f.FieldType != null) &&
                                          x.KleHierarchy != null));
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task GetHearings_IsHearingInvitee(int userId)
        {
            var hearingResponse = GetHandlerResult();
            SecurityExpressionsMock.Setup(x => x.IsHearingInvitee(It.IsAny<Hearing>()))
                .Returns((Hearing hearing) => hearingResponse.ContainsHearingInRole(hearing.Id,userId,BallerupKommune.Models.Enums.HearingRole.HEARING_INVITEE));

            SecurityExpressionsMock.Setup(x => x.IsHearingPublished(It.IsAny<Hearing>()))
                .Returns((Hearing hearing) => hearingResponse.ContainsPublishedHearing(hearing.Id));

            var request = new GetHearingsQuery();

            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            var expectedResult = GetHandlerResult().Where(x => 
                x.HearingStatus.Status != BallerupKommune.Models.Enums.HearingStatus.DRAFT &&
                x.HearingStatus.Status != BallerupKommune.Models.Enums.HearingStatus.CREATED &&
                x.UserHearingRoles.Any(uhr =>
                uhr.User.Id == userId &&
                uhr.HearingRole.Role == BallerupKommune.Models.Enums.HearingRole.HEARING_INVITEE)).ToList();

            Assert.IsTrue(result.Count == expectedResult.Count);
            Assert.IsTrue(result.All(x => x.UserHearingRoles == null 
                                          && x.HearingType.FieldTemplates == null 
                                          && x.HearingType.HearingTemplate.Fields.All(f => f.FieldTemplates == null && f.FieldType != null) &&
                                          x.KleHierarchy != null));
        }

        [Test]
        public async Task GetHearings_Employee()
        {
            var hearingResponse = GetHandlerResult();
            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string role) => role == Security.Roles.Employee);

            SecurityExpressionsMock.Setup(x => x.IsHearingPublished(It.IsAny<Hearing>()))
                .Returns((Hearing hearing) => hearingResponse.ContainsPublishedHearing(hearing.Id));

            SecurityExpressionsMock.Setup(x => x.IsInternalHearing(It.IsAny<Hearing>()))
                .Returns((Hearing hearing) => hearingResponse.ContainsInternalHearing(hearing.Id));

            var request = new GetHearingsQuery();

            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            var expectedResult = GetHandlerResult().Where(x =>
                x.HearingStatus.Status != BallerupKommune.Models.Enums.HearingStatus.DRAFT &&
                x.HearingStatus.Status != BallerupKommune.Models.Enums.HearingStatus.CREATED &&
                !x.ClosedHearing &&
                x.HearingType.IsInternalHearing).ToList();
            Assert.IsTrue(result.Count == expectedResult.Count);
            Assert.IsTrue(result.All(x => x.UserHearingRoles == null
                                          && !x.ClosedHearing
                                          && x.HearingType.IsInternalHearing
                                          && x.HearingType.FieldTemplates == null
                                          && x.HearingType.HearingTemplate.Fields.All(f => f.FieldTemplates == null && f.FieldType != null) && 
                                          x.KleHierarchy != null));
        }

        [Test]
        public async Task GetHearings_NotEmployee()
        {
            var hearingResponse = GetHandlerResult();
            SecurityExpressionRoot.Setup(x => x.HasRole(It.IsAny<string>()))
                .Returns((string role) => role == Security.Roles.Anonymous || role == Security.Roles.Citizen);

            SecurityExpressionsMock.Setup(x => x.IsHearingPublished(It.IsAny<Hearing>()))
                .Returns((Hearing hearing) => hearingResponse.ContainsPublishedHearing(hearing.Id));

            SecurityExpressionsMock.Setup(x => x.IsInternalHearing(It.IsAny<Hearing>()))
                .Returns((Hearing hearing) => hearingResponse.ContainsInternalHearing(hearing.Id));

            var request = new GetHearingsQuery();

            var result =
                await SecurityBehaviour.Handle(request, CancellationToken.None, RequestHandlerDelegateMock.Object);

            var expectedResult = GetHandlerResult().Where(x =>
                x.HearingStatus.Status != BallerupKommune.Models.Enums.HearingStatus.DRAFT &&
                x.HearingStatus.Status != BallerupKommune.Models.Enums.HearingStatus.CREATED &&
                !x.ClosedHearing &&
                !x.HearingType.IsInternalHearing).ToList();
            Assert.IsTrue(result.Count == expectedResult.Count);
            Assert.IsTrue(result.All(x => x.UserHearingRoles == null
                                          && !x.ClosedHearing
                                          && !x.HearingType.IsInternalHearing
                                          && x.HearingType.FieldTemplates == null
                                          && x.HearingType.HearingTemplate.Fields.All(f => f.FieldTemplates == null && f.FieldType != null) &&
                                          x.KleHierarchy != null));
        }



        private List<Hearing> GetHandlerResult()
        {
            return new List<Hearing>
            {
                new Hearing
                {
                    Id = 1,
                    ClosedHearing = false,
                    HearingStatus = new HearingStatus
                    {
                        Status = BallerupKommune.Models.Enums.HearingStatus.DRAFT
                    },
                    KleHierarchy = new KleHierarchy(),
                    HearingType = new HearingType
                    {
                        IsInternalHearing = true,
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
                            HearingRole = new HearingRole
                            {
                                Role = BallerupKommune.Models.Enums.HearingRole.HEARING_OWNER
                            },
                            User = new User
                            {
                                Id = 1
                            }
                        },
                        new UserHearingRole
                        {
                            HearingRole = new HearingRole
                            {
                                Role = BallerupKommune.Models.Enums.HearingRole.HEARING_INVITEE
                            },
                            User = new User
                            {
                                Id = 2
                            }
                        },
                        new UserHearingRole
                        {
                            HearingRole = new HearingRole
                            {
                                Role = BallerupKommune.Models.Enums.HearingRole.HEARING_REVIEWER
                            },
                            User = new User
                            {
                                Id = 3
                            }
                        }
                    }
                },
                new Hearing
                {
                    Id = 2,
                    ClosedHearing = false,
                    HearingStatus = new HearingStatus
                    {
                        Status = BallerupKommune.Models.Enums.HearingStatus.ACTIVE
                    },
                    KleHierarchy = new KleHierarchy(),
                    HearingType = new HearingType
                    {
                        IsInternalHearing = true,
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
                            HearingRole = new HearingRole
                            {
                                Role = BallerupKommune.Models.Enums.HearingRole.HEARING_OWNER
                            },
                            User = new User
                            {
                                Id = 1
                            }
                        },
                        new UserHearingRole
                        {
                            HearingRole = new HearingRole
                            {
                                Role = BallerupKommune.Models.Enums.HearingRole.HEARING_INVITEE
                            },
                            User = new User
                            {
                                Id = 2
                            }
                        },
                        new UserHearingRole
                        {
                            HearingRole = new HearingRole
                            {
                                Role = BallerupKommune.Models.Enums.HearingRole.HEARING_REVIEWER
                            },
                            User = new User
                            {
                                Id = 3
                            }
                        }
                    }
                },
                new Hearing
                {
                    Id = 3,
                    ClosedHearing = true,
                    HearingStatus = new HearingStatus
                    {
                        Status = BallerupKommune.Models.Enums.HearingStatus.CONCLUDED
                    },
                    KleHierarchy = new KleHierarchy(),
                    HearingType = new HearingType
                    {
                        IsInternalHearing = false,
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
                            HearingRole = new HearingRole
                            {
                                Role = BallerupKommune.Models.Enums.HearingRole.HEARING_OWNER
                            },
                            User = new User
                            {
                                Id = 1
                            }
                        },
                        new UserHearingRole
                        {
                            HearingRole = new HearingRole
                            {
                                Role = BallerupKommune.Models.Enums.HearingRole.HEARING_INVITEE
                            },
                            User = new User
                            {
                                Id = 2
                            }
                        },
                        new UserHearingRole
                        {
                            HearingRole = new HearingRole
                            {
                                Role = BallerupKommune.Models.Enums.HearingRole.HEARING_REVIEWER
                            },
                            User = new User
                            {
                                Id = 3
                            }
                        }
                    }
                },
                new Hearing
                {
                    Id = 4,
                    ClosedHearing = true,
                    HearingStatus = new HearingStatus
                    {
                        Status = BallerupKommune.Models.Enums.HearingStatus.CONCLUDED
                    },
                    KleHierarchy = new KleHierarchy(),
                    HearingType = new HearingType
                    {
                        IsInternalHearing = false,
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
                            HearingRole = new HearingRole
                            {
                                Role = BallerupKommune.Models.Enums.HearingRole.HEARING_OWNER
                            },
                            User = new User
                            {
                                Id = 1
                            }
                        },
                        new UserHearingRole
                        {
                            HearingRole = new HearingRole
                            {
                                Role = BallerupKommune.Models.Enums.HearingRole.HEARING_INVITEE
                            },
                            User = new User
                            {
                                Id = 2
                            }
                        },
                        new UserHearingRole
                        {
                            HearingRole = new HearingRole
                            {
                                Role = BallerupKommune.Models.Enums.HearingRole.HEARING_REVIEWER
                            },
                            User = new User
                            {
                                Id = 3
                            }
                        }
                    }
                }
            };
        }
    }
}