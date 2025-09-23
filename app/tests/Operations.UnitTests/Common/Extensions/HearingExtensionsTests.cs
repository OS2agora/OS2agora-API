using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Extensions;
using BallerupKommune.Operations.Resolvers;
using Moq;
using NUnit.Framework;
using HearingRoleEnum = BallerupKommune.Models.Enums.HearingRole;
using ContentTypeEnum = BallerupKommune.Models.Enums.ContentType;
using FieldTypeEnum = BallerupKommune.Models.Enums.FieldType;

namespace BallerupKommune.Operations.UnitTests.Common.Extensions
{
    public class HearingExtensionsTests
    {
        private Mock<IHearingRoleResolver> _hearingRoleResolverMock;
        private Mock<IFieldSystemResolver> _fieldSystemResolverMock;

        private static readonly HearingRole HearingOwnerRole = CreateTestHearingRole(HearingRoleEnum.HEARING_OWNER);
        private static readonly HearingRole HearingInviteeRole = CreateTestHearingRole(HearingRoleEnum.HEARING_INVITEE);
        private static readonly HearingRole HearingResponderRole = CreateTestHearingRole(HearingRoleEnum.HEARING_RESPONDER);

        private static readonly ContentType TextContentType = CreateTestContentType(ContentTypeEnum.TEXT);
        private static readonly ContentType FileContentType = CreateTestContentType(ContentTypeEnum.FILE);

        private static readonly FieldType TitleFieldType = CreateTestFieldType(FieldTypeEnum.TITLE);
        private static readonly FieldType SummaryFieldType = CreateTestFieldType(FieldTypeEnum.SUMMARY);

        private static readonly Field TitleField =
            new Field {Id = 1, FieldTypeId = TitleFieldType.Id, FieldType = TitleFieldType};

        private static readonly Field SummaryField =
            new Field {Id = 2, FieldTypeId = SummaryFieldType.Id, FieldType = SummaryFieldType};
        
        [SetUp]
        public void SetUp()
        {
            _hearingRoleResolverMock = new Mock<IHearingRoleResolver>();
            _fieldSystemResolverMock = new Mock<IFieldSystemResolver>();
        }

        [Test]
        public async Task GetHearingOwner_GetsHearingOwner()
        {
            var hearingOwner = new User {Id = 1};
            var hearingResponder = new User {Id = 2};
            var hearing = new Hearing
            {
                Id = 1,
                UserHearingRoles = new List<UserHearingRole>
                {
                    new UserHearingRole {Id = 1, User = hearingOwner, HearingRoleId = HearingOwnerRole.Id},
                    new UserHearingRole {Id = 2, User = hearingResponder, HearingRoleId = HearingResponderRole.Id}
                }
            };

            _hearingRoleResolverMock
                .Setup(resolver => resolver.GetHearingRoles(It.Is<HearingRoleEnum[]>(roles =>
                    roles.SingleOrDefault() == HearingRoleEnum.HEARING_OWNER)))
                .ReturnsAsync(new List<HearingRole> {HearingOwnerRole})
                .Verifiable();

            User result = await hearing.GetHearingOwner(_hearingRoleResolverMock.Object);
            
            Assert.That(result, Is.EqualTo(hearingOwner));
            _hearingRoleResolverMock.Verify();
        }
        
        [Test]
        public async Task GetUsersWithRole_GetsUsersWithRole()
        {
            var hearingOwner = new User {Id = 1};
            var hearingResponder1 = new User {Id = 2};
            var hearingResponder2 = new User {Id = 3};
            var hearing = new Hearing
            {
                Id = 1,
                UserHearingRoles = new List<UserHearingRole>
                {
                    new UserHearingRole {Id = 1, User = hearingOwner, HearingRoleId = HearingOwnerRole.Id},
                    new UserHearingRole {Id = 2, User = hearingResponder1, HearingRoleId = HearingResponderRole.Id},
                    new UserHearingRole {Id = 3, User = hearingResponder2, HearingRoleId = HearingResponderRole.Id}
                }
            };

            _hearingRoleResolverMock
                .Setup(resolver => resolver.GetHearingRoles(It.Is<HearingRoleEnum[]>(roles =>
                    roles.SingleOrDefault() == HearingRoleEnum.HEARING_RESPONDER)))
                .ReturnsAsync(new List<HearingRole> {HearingResponderRole})
                .Verifiable();

            List<User> result =
                await hearing.GetUsersWithRole(_hearingRoleResolverMock.Object, HearingRoleEnum.HEARING_RESPONDER);
            
            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(2));
                Assert.That(result, Has.Exactly(1).Items.EqualTo(hearingResponder1));
                Assert.That(result, Has.Exactly(1).Items.EqualTo(hearingResponder2));
            });
            _hearingRoleResolverMock.Verify();
        }
        
        [Test]
        public async Task GetUsersWithRoles_GetsUsersWithRole()
        {
            var hearingOwner = new User {Id = 1};
            var hearingResponder1 = new User {Id = 2};
            var hearingResponder2 = new User {Id = 3};
            var hearingInvitee = new User {Id = 4};
            var hearing = new Hearing
            {
                Id = 1,
                UserHearingRoles = new List<UserHearingRole>
                {
                    new UserHearingRole {Id = 1, User = hearingOwner, HearingRoleId = HearingOwnerRole.Id},
                    new UserHearingRole {Id = 2, User = hearingResponder1, HearingRoleId = HearingResponderRole.Id},
                    new UserHearingRole {Id = 3, User = hearingResponder2, HearingRoleId = HearingResponderRole.Id},
                    new UserHearingRole {Id = 4, User = hearingInvitee, HearingRoleId = HearingInviteeRole.Id}
                }
            };

            _hearingRoleResolverMock
                .Setup(resolver => resolver.GetHearingRoles(It.Is<HearingRoleEnum[]>(roles =>
                    roles.Length == 2 && 
                    roles.Contains(HearingRoleEnum.HEARING_RESPONDER) &&
                    roles.Contains(HearingRoleEnum.HEARING_INVITEE))))
                .ReturnsAsync(new List<HearingRole> {HearingResponderRole, HearingInviteeRole})
                .Verifiable();

            List<User> result = await hearing.GetUsersWithAnyOfTheRoles(_hearingRoleResolverMock.Object,
                HearingRoleEnum.HEARING_RESPONDER, HearingRoleEnum.HEARING_INVITEE);
            
            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(3));
                Assert.That(result, Has.Exactly(1).Items.EqualTo(hearingResponder1));
                Assert.That(result, Has.Exactly(1).Items.EqualTo(hearingResponder2));
                Assert.That(result, Has.Exactly(1).Items.EqualTo(hearingInvitee));
            });
            _hearingRoleResolverMock.Verify();
        }

        [Test]
        public async Task GetTextContentOfFieldType_GetsTextContent()
        {
            var titleTextContent = new Content
            {
                Id = 1, 
                FieldId = TitleField.Id, 
                Field = TitleField, 
                ContentTypeId = TextContentType.Id,
                ContentType = TextContentType
            };
            var titleFileContent = new Content
            {
                Id = 2, 
                FieldId = TitleField.Id, 
                Field = TitleField, 
                ContentTypeId = FileContentType.Id,
                ContentType = FileContentType
            };
            var summaryTextContent = new Content
            {
                Id = 3, 
                FieldId = SummaryField.Id, 
                Field = SummaryField, 
                ContentTypeId = TextContentType.Id,
                ContentType = TextContentType
            };
            var hearing = new Hearing
                {Id = 1, Contents = new List<Content> {titleTextContent, titleFileContent, summaryTextContent}};

            _fieldSystemResolverMock
                .Setup(resolver => resolver.GetFieldsIds(FieldTypeEnum.TITLE))
                .ReturnsAsync(new List<int> {TitleField.Id})
                .Verifiable();

            Content result =
                await hearing.GetTextContentOfFieldType(_fieldSystemResolverMock.Object, FieldTypeEnum.TITLE);
            
            Assert.That(result, Is.EqualTo(titleTextContent));
            _fieldSystemResolverMock.Verify();
        }
        
        [Test]
        public async Task GetFileContentsOfFieldType_GetsFileContents()
        {
            var titleTextContent = new Content
            {
                Id = 1, 
                FieldId = TitleField.Id, 
                Field = TitleField, 
                ContentTypeId = TextContentType.Id,
                ContentType = TextContentType
            };
            var titleFileContent = new Content
            {
                Id = 2, 
                FieldId = TitleField.Id, 
                Field = TitleField, 
                ContentTypeId = FileContentType.Id,
                ContentType = FileContentType
            };
            var summaryFileContent = new Content
            {
                Id = 3, 
                FieldId = SummaryField.Id, 
                Field = SummaryField, 
                ContentTypeId = FileContentType.Id,
                ContentType = FileContentType
            };
            var hearing = new Hearing
                {Id = 1, Contents = new List<Content> {titleTextContent, titleFileContent, summaryFileContent}};

            _fieldSystemResolverMock
                .Setup(resolver => resolver.GetFieldsIds(FieldTypeEnum.TITLE))
                .ReturnsAsync(new List<int> {TitleField.Id})
                .Verifiable();

            List<Content> result =
                await hearing.GetFileContentsOfFieldType(_fieldSystemResolverMock.Object, FieldTypeEnum.TITLE);
            
            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(1));
                Assert.That(result, Has.Exactly(1).Items.EqualTo(titleFileContent));
            });
            _fieldSystemResolverMock.Verify();
        }

        private static HearingRole CreateTestHearingRole(HearingRoleEnum role)
        {
            return new HearingRole
            {
                Id = (int) role,
                Role = role
            };
        }

        private static ContentType CreateTestContentType(ContentTypeEnum type)
        {
            return new ContentType
            {
                Id = (int) type,
                Type = type
            };
        }

        private static FieldType CreateTestFieldType(FieldTypeEnum type)
        {
            return new FieldType
            {
                Id = (int) type,
                Type = type
            };
        }
    }
}