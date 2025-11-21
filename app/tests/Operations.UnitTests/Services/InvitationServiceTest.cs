using Agora.Models.Common;
using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Resolvers;
using Agora.Operations.Services;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HearingRoleEnum = Agora.Models.Enums.HearingRole;
using UserCapacityEnum = Agora.Models.Enums.UserCapacity;

namespace Agora.Operations.UnitTests.Services
{
    [TestFixture]
    public class InvitationServiceTests
    {
        private Mock<IUserCapacityDao> _userCapacityDaoMock;
        private Mock<IHearingRoleResolver> _hearingRoleResolverMock;
        private InvitationService _service;

        [SetUp]
        public void SetUp()
        {
            _userCapacityDaoMock = new Mock<IUserCapacityDao>();
            _hearingRoleResolverMock = new Mock<IHearingRoleResolver>();
            _service = new InvitationService(_userCapacityDaoMock.Object, _hearingRoleResolverMock.Object);
        }

        #region NormalizeAndValidateInviteeIdentifiers

        [Test]
        public void NormalizeAndValidateInviteeIdentifiers_Normalizes_And_Validates_All_Types()
        {
            var data = new List<InviteeIdentifiers>
            {
                new() { Cpr = "01-01-701234" },
                new() { Cpr = "0202805678" },
                new() { Cpr = "0101701234" },
                new() { Email = "  USER@Example.dk  " },
                new() { Email = "user@example.dk"},
                new() { Cvr = "12 34 56 78" }
            };

            var (isValid, errors) = _service.NormalizeAndValidateInviteeIdentifiers(data);

            Assert.That(isValid, Is.True);
            Assert.That(errors.Any(), Is.False);
            Assert.That(data.Count, Is.EqualTo(4));
            Assert.That(data.Count(i => i.Cpr != null), Is.EqualTo(2));
            Assert.That(data.Single(i => i.Cpr == "0101701234"), Is.Not.Null);
            Assert.That(data.Single(i => i.Cpr == "0202805678"), Is.Not.Null);
            Assert.That(data.Single(i => i.Email != null).Email, Is.EqualTo("user@example.dk"));
            Assert.That(data.Single(i => i.Cvr != null).Cvr, Is.EqualTo("12345678"));
        }

        [Test]
        public void NormalizeAndValidateInviteeIdentifiers_WithInvalidInputs_ReturnsFalseAndCorrectErrors()
        {
            // CPR contains only 9 digits after normalization -> invalid
            // CVR contains only 7 digits after normalization -> invalid
            var data = new List<InviteeIdentifiers>
            { 
                new() { Cpr = "12-34-56789" },
                new() { Cvr = "1234 567" },
                new() { Email = "not-an-email" },
                new() { Email = "Peter Hansen <peter@example.dk>" }
            };

            var (isValid, errors) = _service.NormalizeAndValidateInviteeIdentifiers(data);

            Assert.That(isValid, Is.False);
            Assert.That(errors.Count, Is.EqualTo(4));

            Assert.That(errors.Any(e => e is InvalidCprException), Is.True);
            Assert.That(errors.Any(e => e is InvalidCvrException), Is.True);
            Assert.That(errors.Count(e => e is InvalidEmailException), Is.EqualTo(2));

            var emailErrors = errors.OfType<InvalidEmailException>().ToList();
            Assert.That(emailErrors.Count(e => e.InnerException != null), Is.EqualTo(1));
            Assert.That(emailErrors.Count(e => e.InnerException == null), Is.EqualTo(1));
        }

        [Test]
        public void NormalizeAndValidateInviteeIdentifiers_WithMixedInputs_NormalizesValidOnesAndCollectsErrors()
        {
            var data = new List<InviteeIdentifiers>
            {
                new() { Cpr = "0101801234" },       // valid
                new() { Email = "invalid-email" },  // invalid
                new() { Cvr = "11-22-33-44" },      // valid
                new() { Cpr = "123456" }            // invalid
            };

            var (isValid, errors) = _service.NormalizeAndValidateInviteeIdentifiers(data);

            Assert.That(isValid, Is.False);
            Assert.That(errors.Count, Is.EqualTo(2));
            Assert.That(errors.Any(e => e is InvalidEmailException), Is.True);
            Assert.That(errors.Any(e => e is InvalidCprException), Is.True);
            Assert.That(data.Count, Is.EqualTo(2));
            Assert.That(data.Single(i => i.Cpr == "0101801234"), Is.Not.Null);
            Assert.That(data.Single(i => i.Cvr == "11223344"), Is.Not.Null);
        }

        #endregion

        #region SplitInviteeIdentifierList

        [Test]
        public void SplitInviteeIdentifierList_Returns_Distinct_Lowercased()
        {
            var cpr = "0101701234";
            var email = "user@example.dk";
            var emailUpperCase = "USER@example.dk";
            var cvr = "12345678";

            var list = new List<InviteeIdentifiers>
            {
                new() { Cpr = cpr },
                new() { Cpr = cpr }, // duplicate
                new() { Email = email },
                new() { Email = emailUpperCase }, // duplicate and case-insensitive
                new() { Cvr = cvr },
                new() { Cvr = cvr } // duplicate
            };

            var (cprs, emails, cvrs) = _service.SplitInviteeIdentifierList(list);

            Assert.That(cprs, Is.EquivalentTo(new[] { cpr }));
            Assert.That(emails, Is.EquivalentTo(new[] { email }));
            Assert.That(cvrs, Is.EquivalentTo(new[] { cvr }));
        }

        #endregion

        #region GetNewAndExistingInvitees

        [Test]
        public void GetNewAndExistingInvitees_Splits_By_Presence_In_Current_Roles()
        {
            var knownEmail = "Known@Example.dk";
            var newEmail = "new@example.dk";
            var knownCpr = "1111111111";
            var newCpr = "2222222222";
            var knownCvr = "87654321";
            var newCvr = "12345678";

            var knownEmailIdentifier = new InviteeIdentifiers { Email = knownEmail };
            var knownCprIdentifier = new InviteeIdentifiers { Cpr = knownCpr };
            var knownCvrIdentifier = new InviteeIdentifiers { Cvr = knownCvr };
            var newEmailIdentifier = new InviteeIdentifiers { Email = newEmail };
            var newCprIdentifier = new InviteeIdentifiers { Cpr = newCpr };
            var newCvrIdentifier = new InviteeIdentifiers { Cvr = newCvr };

            var invitees = new List<InviteeIdentifiers>
            {
                knownEmailIdentifier, 
                newEmailIdentifier,
                knownCprIdentifier, 
                newCprIdentifier,
                knownCvrIdentifier,
                newCvrIdentifier
            };

            var currentUsers = new List<UserHearingRole>
            {
                new() { User = new User { Email = knownEmail } },
                new() { User = new User { PersonalIdentifier = knownCpr }}
            };

            var currentCompanies = new List<CompanyHearingRole>
            {
                new() { Company = new Company { Cvr = knownCvr } }
            };

            var expectedNewIdentifiers = new List<InviteeIdentifiers> { newEmailIdentifier, newCprIdentifier, newCvrIdentifier };
            var expectedExistingIdentifiers = new List<InviteeIdentifiers> { knownEmailIdentifier, knownCprIdentifier, knownCvrIdentifier };

            var (newIdentifiers, existingIdentifiers) = _service.GetNewAndExistingInvitees(currentUsers, currentCompanies, invitees);

            Assert.That(existingIdentifiers.Count, Is.EqualTo(3));
            Assert.That(existingIdentifiers, Is.EquivalentTo(expectedExistingIdentifiers));
            Assert.That(newIdentifiers.Count, Is.EqualTo(3));
            Assert.That(newIdentifiers, Is.EquivalentTo(expectedNewIdentifiers));
        }

        #endregion

        #region GetUserHearingRolesToCreate

        [Test]
        public async Task GetUserHearingRolesToCreate_Returns_Empty_When_No_Ids()
        {
            var result = await _service.GetUserHearingRolesToCreate(1, new List<int>());
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetUserHearingRolesToCreate_Uses_Resolver_And_Maps_Ids()
        {
            _hearingRoleResolverMock
                .Setup(r => r.GetHearingRole(HearingRoleEnum.HEARING_INVITEE))
                .ReturnsAsync(new HearingRole { Id = 1 });

            var userIds = new List<int> { 1, 2, 3 };

            var result = await _service.GetUserHearingRolesToCreate(1, userIds);

            Assert.That(result.Select(x => x.UserId), Is.EquivalentTo(userIds));
            Assert.That(result.All(x => x.HearingRoleId == 1 && x.HearingId == 1), Is.True);

            _hearingRoleResolverMock.VerifyAll();
        }

        #endregion

        #region GetCompanyHearingRolesToCreate

        [Test]
        public async Task GetCompanyHearingRolesToCreate_Returns_Empty_When_No_Ids()
        {
            var result = await _service.GetCompanyHearingRolesToCreate(1, new List<int>());
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetCompanyHearingRolesToCreate_Uses_Resolver_And_Maps_Ids()
        {
            _hearingRoleResolverMock
                .Setup(r => r.GetHearingRole(HearingRoleEnum.HEARING_INVITEE))
                .ReturnsAsync(new HearingRole { Id = 1 });

            var companyIds = new List<int> { 1, 2 };

            var result = await _service.GetCompanyHearingRolesToCreate(1, companyIds);

            Assert.That(result.Select(x => x.CompanyId), Is.EquivalentTo(companyIds));
            Assert.That(result.All(x => x.HearingRoleId == 1 && x.HearingId == 1), Is.True);

            _hearingRoleResolverMock.VerifyAll();
        }

        #endregion

        #region GetIdsForUserHearingRolesWithoutInvitationSourceMappings

        [Test]
        public void GetIdsForUserHearingRolesWithoutInvitationSourceMappings_Returns_Ids_With_Matching_Identifier_And_No_Mapping()
        {
            var sourceName = "SRC";
            var sourceId = 1;

            var (userHearingRolelist, expectedResult, cprs, emails) = GetTestData_GetIdsForUserHearingRolesWithoutInvitationSourceMappings(sourceName, sourceId);

            var ids = _service.GetIdsForUserHearingRolesWithoutInvitationSourceMappings(userHearingRolelist, cprs, emails, sourceId, sourceName);

            Assert.That(ids, Is.EquivalentTo(expectedResult));
        }

        private (List<UserHearingRole>, List<int>, List<string>, List<string>) GetTestData_GetIdsForUserHearingRolesWithoutInvitationSourceMappings(string sourceName, int sourceId)
        {
            var cpr = "0101701234";
            var cprNoMatch = "1234567899";
            var email = "match@example.dk";
            var emailNoMatch = "different@example.dk";

            var sourceNameNoMatch = "DIFFERENT-SRC";
            var sourceIdNoMatch = 99;

            var cprs = new List<string> { cpr };
            var emails = new List<string> { email };

            // Users
            var citizenUserWithMatch = new User { Cpr = cpr };
            var citizenUserWithoutMatch = new User { Cpr = cprNoMatch };
            var employeeUserWithMatch = new User { Email = email };
            var employeeUserWithoutMatch = new User { Email = emailNoMatch };

            // InvitationSourceMappings
            var matchingSourceMapping = new InvitationSourceMapping
            {
                Id = 1,
                SourceName = sourceName,
                InvitationSourceId = sourceId
            };
            var sourceMappingDifferentNameSameId = new InvitationSourceMapping
            {
                Id = 2,
                SourceName = sourceNameNoMatch,
                InvitationSourceId = sourceId
            };
            var sourceMappingSameNameDifferentId = new InvitationSourceMapping
            {
                Id = 3,
                SourceName = sourceName,
                InvitationSourceId = sourceIdNoMatch
            };

            // UserHearingRoles
            var citizenWithMatchUhrWithMapping = new UserHearingRole
            {
                Id = 1,
                User = citizenUserWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping }
            };
            var citizenWithMatchUhrWithoutMapping = new UserHearingRole
            {
                Id = 2,
                User = citizenUserWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping>()
            };
            var citizenWithMatchUhrWithMatchingSourceName = new UserHearingRole
            {
                Id = 3,
                User = citizenUserWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingSameNameDifferentId }
            };
            var citizenWithMatchUhrWithMatchingSourceId = new UserHearingRole
            {
                Id = 4,
                User = citizenUserWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingDifferentNameSameId }
            };
            var citizenWithoutMatchUhrWithMapping = new UserHearingRole
            {
                Id = 5,
                User = citizenUserWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping }
            };
            var citizenWithoutMatchUhrWithoutMapping = new UserHearingRole
            {
                Id = 6,
                User = citizenUserWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping>()
            };
            var citizenWithoutMatchUhrWithMatchingSourceName = new UserHearingRole
            {
                Id = 7,
                User = citizenUserWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingSameNameDifferentId }
            };
            var citizenWithoutMatchUhrWithMatchingSourceId = new UserHearingRole
            {
                Id = 8,
                User = citizenUserWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingDifferentNameSameId }
            };
            var employeeWithMatchUhrWithMapping = new UserHearingRole
            {
                Id = 9,
                User = employeeUserWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping }
            };
            var employeeWithMatchUhrWithoutMapping = new UserHearingRole
            {
                Id = 10,
                User = employeeUserWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping>()
            };
            var employeeWithMatchUhrWithMatchingSourceName = new UserHearingRole
            {
                Id = 11,
                User = employeeUserWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingSameNameDifferentId }
            };
            var employeeWithMatchUhrWithMatchingSourceId = new UserHearingRole
            {
                Id = 12,
                User = employeeUserWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingDifferentNameSameId }
            };
            var employeeWithoutMatchUhrWithMapping = new UserHearingRole
            {
                Id = 13,
                User = employeeUserWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping }
            };
            var employeeWithoutMatchUhrWithoutMapping = new UserHearingRole
            {
                Id = 14,
                User = employeeUserWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping>()
            };
            var employeeWithoutMatchUhrWithMatchingSourceName = new UserHearingRole
            {
                Id = 15,
                User = employeeUserWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingSameNameDifferentId }
            };
            var employeeWithoutMatchUhrWithMatchingSourceId = new UserHearingRole
            {
                Id = 16,
                User = employeeUserWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingDifferentNameSameId }
            };

            var userHearingRoles = new List<UserHearingRole>
            {
                citizenWithMatchUhrWithMapping,
                citizenWithMatchUhrWithoutMapping,
                citizenWithMatchUhrWithMatchingSourceName,
                citizenWithMatchUhrWithMatchingSourceId,
                citizenWithoutMatchUhrWithMapping,
                citizenWithoutMatchUhrWithoutMapping,
                citizenWithoutMatchUhrWithMatchingSourceName,
                citizenWithoutMatchUhrWithMatchingSourceId,
                employeeWithMatchUhrWithMapping,
                employeeWithMatchUhrWithoutMapping,
                employeeWithMatchUhrWithMatchingSourceName,
                employeeWithMatchUhrWithMatchingSourceId,
                employeeWithoutMatchUhrWithMapping,
                employeeWithoutMatchUhrWithoutMapping,
                employeeWithoutMatchUhrWithMatchingSourceName,
                employeeWithoutMatchUhrWithMatchingSourceId
            };

            var expectedResult = new List<int> { 2, 3, 4, 10, 11, 12 };

            return (userHearingRoles, expectedResult, cprs, emails);
        }

        #endregion

        #region GetIdsForCompanyHearingRolesWithoutInvitationSourceMappings
        [Test]
        public void GetIdsForCompanyHearingRolesWithoutInvitationSourceMappings_Returns_Ids_With_Cvr_Match_And_No_Mapping()
        {
            var sourceName = "SRC";
            var sourceId = 1;

            var (companyHearingRoles, expectedResult, cvrs) = GetTestData_GetIdsForCompanyHearingRolesWithoutInvitationSourceMappings(sourceName, sourceId);

            var ids = _service.GetIdsForCompanyHearingRolesWithoutInvitationSourceMappings(companyHearingRoles, cvrs, sourceId, sourceName);

            Assert.That(ids, Is.EquivalentTo(expectedResult));
        }

        private (List<CompanyHearingRole>, List<int>, List<string>) GetTestData_GetIdsForCompanyHearingRolesWithoutInvitationSourceMappings(string sourceName, int sourceId)
        {
            var cvr = "12345678";
            var cvrNoMatch = "87654321";

            var sourceNameNoMatch = "DIFFERENT-SRC";
            var sourceIdNoMatch = 99;

            var cvrs = new List<string> { cvr };

            var companyWithMatch = new Company { Cvr = cvr };
            var companyWithoutMatch = new Company { Cvr = cvrNoMatch };

            var matchingSourceMapping = new InvitationSourceMapping
            {
                Id = 1,
                SourceName = sourceName,
                InvitationSourceId = sourceId
            };
            var sourceMappingDifferentNameSameId = new InvitationSourceMapping
            {
                Id = 2,
                SourceName = sourceNameNoMatch,
                InvitationSourceId = sourceId
            };
            var sourceMappingSameNameDifferentId = new InvitationSourceMapping
            {
                Id = 3,
                SourceName = sourceName,
                InvitationSourceId = sourceIdNoMatch
            };

            var chrWithMatchWithMapping = new CompanyHearingRole
            {
                Id = 1,
                Company = companyWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping }
            };
            var chrWithMatchWithoutMapping = new CompanyHearingRole
            {
                Id = 2,
                Company = companyWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping>()
            };
            var chrWithMatchWithMatchingSourceName = new CompanyHearingRole
            {
                Id = 3,
                Company = companyWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingSameNameDifferentId }
            };
            var chrWithMatchWithMatchingSourceId = new CompanyHearingRole
            {
                Id = 4,
                Company = companyWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingDifferentNameSameId }
            };
            var chrWithoutMatchWithMapping = new CompanyHearingRole
            {
                Id = 5,
                Company = companyWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping }
            };
            var chrWithoutMatchWithoutMapping = new CompanyHearingRole
            {
                Id = 6,
                Company = companyWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping>()
            };
            var chrWithoutMatchWithMatchingSourceName = new CompanyHearingRole
            {
                Id = 7,
                Company = companyWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingSameNameDifferentId }
            };
            var chrWithoutMatchWithMatchingSourceId = new CompanyHearingRole
            {
                Id = 8,
                Company = companyWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingDifferentNameSameId }
            };

            var companyHearingRoles = new List<CompanyHearingRole>
            {
                chrWithMatchWithMapping,
                chrWithMatchWithoutMapping,
                chrWithMatchWithMatchingSourceName,
                chrWithMatchWithMatchingSourceId,
                chrWithoutMatchWithMapping,
                chrWithoutMatchWithoutMapping,
                chrWithoutMatchWithMatchingSourceName,
                chrWithoutMatchWithMatchingSourceId
            };

            var expectedResult = new List<int> { 2, 3, 4 };

            return (companyHearingRoles, expectedResult, cvrs);
        }


        #endregion

        #region GetUserInviteeRelationsToRemove

        [Test]
        public void GetUserInviteeRelationsToRemove_Splits_Into_RolesToDelete_And_MappingsToDelete()
        {
            var sourceName = "SRC";
            var sourceId = 1;

            var (userHearingRoles, expectedRolesToDelete, expectedMappingsToDelete, cprs, emails) =
                GetTestData_GetUserInviteeRelationsToRemove(sourceName, sourceId);

            var (rolesToDelete, mappingsToDelete) = _service.GetUserInviteeRelationsToRemove(
                userHearingRoles, cprs, emails, sourceId, sourceName);

            Assert.That(rolesToDelete, Is.EquivalentTo(expectedRolesToDelete));
            Assert.That(mappingsToDelete, Is.EquivalentTo(expectedMappingsToDelete));
        }

        private (List<UserHearingRole>, int[], int[], List<string>, List<string>) GetTestData_GetUserInviteeRelationsToRemove(string sourceName, int sourceId)
        {

            var sourceNameNoMatch = "OTHER";
            var sourceIdNoMatch = 99;

            var cpr = "1234567899";
            var cprNoMatch = "1122334455";
            var email = "keep@example.dk";
            var emailNoMatch = "delete@example.dk";

            var cprs = new List<string> { cpr };
            var emails = new List<string> { email };

            // Users
            var citizenWithoutMatch = new User { Cpr = cprNoMatch };
            var employeeWithoutMatch = new User { Email = emailNoMatch };
            var citizenWithMatch = new User { Cpr = cpr };
            var employeeWithMatch = new User { Email = email };

            // InvitationSourceMappings
            var sourceMappingSameNameDifferentId = new InvitationSourceMapping { Id = 101, SourceName = sourceName, InvitationSourceId = sourceIdNoMatch };
            var sourceMappingDifferentNameSameId = new InvitationSourceMapping { Id = 102, SourceName = sourceNameNoMatch, InvitationSourceId = sourceId };
            var sourceMappingNoMatch = new InvitationSourceMapping { Id = 103, SourceName = sourceNameNoMatch, InvitationSourceId = sourceIdNoMatch };

            var matchingSourceMapping1 = new InvitationSourceMapping { Id = 1, SourceName = sourceName, InvitationSourceId = sourceId };
            var matchingSourceMapping2 = new InvitationSourceMapping { Id = 2, SourceName = sourceName, InvitationSourceId = sourceId };
            var matchingSourceMapping3 = new InvitationSourceMapping { Id = 3, SourceName = sourceName, InvitationSourceId = sourceId };
            var matchingSourceMapping4 = new InvitationSourceMapping { Id = 4, SourceName = sourceName, InvitationSourceId = sourceId };
            var matchingSourceMapping5 = new InvitationSourceMapping { Id = 5, SourceName = sourceName, InvitationSourceId = sourceId };
            var matchingSourceMapping6 = new InvitationSourceMapping { Id = 6, SourceName = sourceName, InvitationSourceId = sourceId };
            var matchingSourceMapping7 = new InvitationSourceMapping { Id = 7, SourceName = sourceName, InvitationSourceId = sourceId };
            var matchingSourceMapping8 = new InvitationSourceMapping { Id = 8, SourceName = sourceName, InvitationSourceId = sourceId };


            // UserHearingRoles
            var matchCitizenNoSources_keep = new UserHearingRole
            {
                Id = 1,
                User = citizenWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping>()
            };
            var matchingEmployeeNoSources_keep = new UserHearingRole
            {
                Id = 2,
                User = employeeWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping>()
            };
            var matchCitizenOneMatchingSource_keep = new UserHearingRole
            {
                Id = 3,
                User = citizenWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping1 }
            };
            var matchingEmployeeOneMatchingSource_keep = new UserHearingRole
            {
                Id = 4,
                User = employeeWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping2 }
            };
            var matchCitizenMultipleSourcesOneMatching_keep = new UserHearingRole
            {
                Id = 5,
                User = citizenWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping3, sourceMappingNoMatch }
            };
            var matchingEmployeeMultipleSourcesOneMatching_keep = new UserHearingRole
            {
                Id = 6,
                User = employeeWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping4, sourceMappingDifferentNameSameId }
            };
            var matchCitizenNoMatchingSource_keep = new UserHearingRole
            {
                Id = 7,
                User = citizenWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingNoMatch }
            };
            var matchingEmployeeNoMatchingSource_keep = new UserHearingRole
            {
                Id = 8,
                User = employeeWithMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingSameNameDifferentId }
            };

            var noMatchCitizenNoSources_delete_uhr = new UserHearingRole
            {
                Id = 9,
                User = citizenWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping>()
            };
            var noMatchingEmployeeNoSources_delete_uhr = new UserHearingRole
            {
                Id = 10,
                User = employeeWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping>()
            };
            var noMatchCitizenOneMatchingSource_delete_uhr = new UserHearingRole
            {
                Id = 11,
                User = citizenWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping5 }
            };
            var noMatchingEmployeeOneMatchingSource_delete_uhr = new UserHearingRole
            {
                Id = 12,
                User = employeeWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping6 }
            };
            var noMatchCitizenMultipleSourcesOneMatching_delete_ism = new UserHearingRole
            {
                Id = 13,
                User = citizenWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping7, sourceMappingNoMatch }
            };
            var noMatchingEmployeeMultipleSourcesOneMatching_delete_ism = new UserHearingRole
            {
                Id = 14,
                User = employeeWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping8, sourceMappingDifferentNameSameId }
            };
            var noMatchCitizenNoMatchingSource_keep = new UserHearingRole
            {
                Id = 15,
                User = citizenWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingNoMatch }
            };
            var noMatchingEmployeeNoMatchingSource_keep = new UserHearingRole
            {
                Id = 16,
                User = employeeWithoutMatch,
                InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingSameNameDifferentId }
            };

            var userHearingRoles = new List<UserHearingRole>
            {
                matchCitizenNoSources_keep,
                matchingEmployeeNoSources_keep,
                matchCitizenOneMatchingSource_keep,
                matchingEmployeeOneMatchingSource_keep,
                matchCitizenMultipleSourcesOneMatching_keep,
                matchingEmployeeMultipleSourcesOneMatching_keep,
                matchCitizenNoMatchingSource_keep,
                matchingEmployeeNoMatchingSource_keep,
                noMatchCitizenNoSources_delete_uhr,
                noMatchingEmployeeNoSources_delete_uhr,
                noMatchCitizenOneMatchingSource_delete_uhr,
                noMatchingEmployeeOneMatchingSource_delete_uhr,
                noMatchCitizenMultipleSourcesOneMatching_delete_ism,
                noMatchingEmployeeMultipleSourcesOneMatching_delete_ism,
                noMatchCitizenNoMatchingSource_keep,
                noMatchingEmployeeNoMatchingSource_keep
            };

            var expectedUserHearingRolesToDelete = new[] { 9, 10, 11, 12 };
            var expectedSourceMappingsToDelete = new[] { 7, 8 };

            return (userHearingRoles, expectedUserHearingRolesToDelete, expectedSourceMappingsToDelete, cprs, emails);
        }
        #endregion

        #region GetCompanyInviteeRelationsToRemove
        [Test]
        public void GetCompanyInviteeRelationsToRemove_Splits_Into_RolesToDelete_And_MappingsToDelete_Minimal()
        {
            var sourceName = "SRC";
            var sourceId = 1;
            var (companyHearingRoles, expectedRolesToDelete, expectedMappingsToDelete, cvrs) =
                GetTestData_GetCompanyInviteeRelationsToRemove_Minimal(sourceName, sourceId);

            var (rolesToDelete, mappingsToDelete) =
                _service.GetCompanyInviteeRelationsToRemove(companyHearingRoles, cvrs, sourceId, sourceName);

            Assert.That(rolesToDelete, Is.EquivalentTo(expectedRolesToDelete));
            Assert.That(mappingsToDelete, Is.EquivalentTo(expectedMappingsToDelete));
        }

        private (List<CompanyHearingRole>, int[], int[], List<string>) GetTestData_GetCompanyInviteeRelationsToRemove_Minimal(string sourceName, int sourceId)
        {
            var sourceNameNoMatch = "OTHER";
            var sourceIdNoMatch = 99;

            var cvrMatch = "12345678";
            var cvrNoMatch = "87654321";

            var cvrs = new List<string> { cvrMatch };

            var sourceMappingSameNameDifferentId = new InvitationSourceMapping { Id = 101, SourceName = sourceName, InvitationSourceId = sourceIdNoMatch };
            var sourceMappingDifferentNameSameId = new InvitationSourceMapping { Id = 102, SourceName = sourceNameNoMatch, InvitationSourceId = sourceId };
            var sourceMappingNoMatch = new InvitationSourceMapping { Id = 103, SourceName = sourceNameNoMatch, InvitationSourceId = sourceIdNoMatch };

            var matchingSourceMapping1 = new InvitationSourceMapping { Id = 1, SourceName = sourceName, InvitationSourceId = sourceId };
            var matchingSourceMapping2 = new InvitationSourceMapping { Id = 2, SourceName = sourceName, InvitationSourceId = sourceId };
            var matchingSourceMapping3 = new InvitationSourceMapping { Id = 3, SourceName = sourceName, InvitationSourceId = sourceId };

            var companyMatch = new Company { Cvr = cvrMatch };
            var companyNoMatch = new Company { Cvr = cvrNoMatch };

            var keepMatch_NoSources = new CompanyHearingRole { Id = 1, Company = companyMatch, InvitationSourceMappings = new List<InvitationSourceMapping>() };
            var keepMatch_WithMatching = new CompanyHearingRole { Id = 2, Company = companyMatch, InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping1 } };
            var keepMatch_NoMatching = new CompanyHearingRole { Id = 3, Company = companyMatch, InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingDifferentNameSameId } };
            var keepNoMatch_NoMatching = new CompanyHearingRole { Id = 4, Company = companyNoMatch, InvitationSourceMappings = new List<InvitationSourceMapping> { sourceMappingSameNameDifferentId } };

            var deleteRole_NoSources = new CompanyHearingRole { Id = 5, Company = companyNoMatch, InvitationSourceMappings = new List<InvitationSourceMapping>() };
            var deleteRole_OneMatching = new CompanyHearingRole { Id = 6, Company = companyNoMatch, InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping2 } };
            var deleteMapping_Multiple = new CompanyHearingRole { Id = 7, Company = companyNoMatch, InvitationSourceMappings = new List<InvitationSourceMapping> { matchingSourceMapping3, sourceMappingNoMatch } };

            var companyHearingRoles = new List<CompanyHearingRole>
            {
                keepMatch_NoSources,
                keepMatch_WithMatching,
                keepMatch_NoMatching,
                keepNoMatch_NoMatching,
                deleteRole_NoSources,
                deleteRole_OneMatching,
                deleteMapping_Multiple
            };

            var expectedRolesToDelete = new[] { 5, 6 };
            var expectedMappingsToDelete = new[] { 3 };

            return (companyHearingRoles, expectedRolesToDelete, expectedMappingsToDelete, cvrs);
        }

        #endregion

        #region GetNewUsersToCreate

        [Test]
        public async Task GetNewUsersToCreate_Returns_New_Citizens_And_Employees_With_Correct_Capacities()
        {
            var citizenCapacity = new UserCapacity { Id = 1, Capacity = UserCapacityEnum.CITIZEN };
            var employeeCapacity = new UserCapacity { Id = 2, Capacity = UserCapacityEnum.EMPLOYEE };

            _userCapacityDaoMock
                .Setup(d => d.GetAllAsync(It.IsAny<IncludeProperties>()))
                .ReturnsAsync(new List<UserCapacity> { citizenCapacity, employeeCapacity });

            var newCpr = "1234567899";
            var existingCpr = "1234512345";
            var newEmail = "new@example.dk";
            var existingEmail1 = "existing1@example.dk";
            var existingEmail2 = "existing2@example.dk";
            var existingEmail2UpperCase = "EXISTING2@example.dk";


            var existingCitizen = new User
            {
                Id = 1,
                PersonalIdentifier = existingCpr,
                Cpr = existingCpr,
                UserCapacity = citizenCapacity
            };
            var existingEmployee1 = new User
            {
                Id = 2,
                Email = existingEmail1,
                UserCapacity = employeeCapacity
            };

            var existingEmployee2 = new User
            {
                Id = 3,
                Email = existingEmail2,
                UserCapacity = employeeCapacity
            };

            var currentUsers = new List<User>
            {
                existingCitizen,
                existingEmployee1,
                existingEmployee2
            };

            var cprs = new List<string> { newCpr, existingCpr };
            var emails = new List<string> { newEmail, existingEmail1, existingEmail2, existingEmail2UpperCase };

            var result = await _service.GetNewUsersToCreate(cprs, emails, currentUsers);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(u => u.Cpr == newCpr && u.UserCapacityId == citizenCapacity.Id), Is.True);
            Assert.That(result.Any(u => u.Email == newEmail && u.UserCapacityId == employeeCapacity.Id), Is.True);

            _userCapacityDaoMock.VerifyAll();
        }

        #endregion

        #region GetNewCompaniesToCreate

        [Test]
        public void GetNewCompaniesToCreate_Returns_Only_New_CVRs_CaseInsensitive()
        {
            var newCvr = "12345678";
            var existingCvr1 = "87654321";
            var existingCvr2 = "98765432";

            var current = new List<Company>
            {
                new() { Cvr = existingCvr1 },
                new() { Cvr = existingCvr2 }
            };

            var cvrs = new List<string> { newCvr, existingCvr1, existingCvr2 };

            var result = _service.GetNewCompaniesToCreate(cvrs, current);

            Assert.That(result.Select(c => c.Cvr), Is.EquivalentTo(new[] { newCvr }));
        }

        #endregion

        #region GetInvitationSoureMappingIds
        [Test]
        public void GetInvitationSourceMappings_CompanyHearingRole_ReturnsExpectedIds()
        {
            var companyHearingRoles = new List<CompanyHearingRole>
            {
                new CompanyHearingRole
                {
                    InvitationSourceMappings = new List<InvitationSourceMapping>
                    {
                        new InvitationSourceMapping { Id = 1, SourceName = "FetchMe" },
                        new InvitationSourceMapping { Id = 2, SourceName = "DontFetchMe" }
                    }
                },
                new CompanyHearingRole
                {
                    InvitationSourceMappings = new List<InvitationSourceMapping>
                    {
                        new InvitationSourceMapping { Id = 3, SourceName = "FetchMe" },
                        new InvitationSourceMapping { Id = 4, SourceName = "DontFetchMe" }
                    }
                }
            };

            var result = _service.GetInvitationSourceMappingIdsFromCompanyHearingRoles(companyHearingRoles, "FetchMe");
            Assert.AreEqual(new List<int> { 1, 3 }, result);
        }

        [Test]
        public void GetInvitationSourceMappings_CompanyHearingRole_EmptyWhenNotMatches()
        {
            var companyHearingRoles = new List<CompanyHearingRole>
            {
                new CompanyHearingRole
                {
                    InvitationSourceMappings = new List<InvitationSourceMapping>
                    {
                        new InvitationSourceMapping { Id = 1, SourceName = "DontFetchMe1" },
                        new InvitationSourceMapping { Id = 2, SourceName = "DontFetchMe2" }
                    }
                },
            };

            var result = _service.GetInvitationSourceMappingIdsFromCompanyHearingRoles(companyHearingRoles, "RandomSourceName");
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetInvitationSourceMappings_UserHearingRole_ReturnsExpectedIds()
        {
            var userHearingRoles = new List<UserHearingRole>
            {
                new UserHearingRole
                {
                    InvitationSourceMappings = new List<InvitationSourceMapping>
                    {
                        new InvitationSourceMapping { Id = 1, SourceName = "FetchMe" },
                        new InvitationSourceMapping { Id = 2, SourceName = "DontFetchMe" }
                    }
                },
                new UserHearingRole
                {
                    InvitationSourceMappings = new List<InvitationSourceMapping>
                    {
                        new InvitationSourceMapping { Id = 3, SourceName = "FetchMe" },
                        new InvitationSourceMapping { Id = 4, SourceName = "DontFetchMe" }
                    }
                }
            };

            var result = _service.GetInvitationSourceMappingIdsFromUserHearingRoles(userHearingRoles, "FetchMe");
            Assert.AreEqual(new List<int> { 1, 3 }, result);
        }

        [Test]
        public void GetInvitationSourceMappings_UserHearingRole_EmptyWhenNotMatches()
        {
            var userHearingRoles = new List<UserHearingRole>
            {
                new UserHearingRole
                {
                    InvitationSourceMappings = new List<InvitationSourceMapping>
                    {
                        new InvitationSourceMapping { Id = 1, SourceName = "DontFetchMe1" },
                        new InvitationSourceMapping { Id = 2, SourceName = "DontFetchMe2" }
                    }
                },
            };

            var result = _service.GetInvitationSourceMappingIdsFromUserHearingRoles(userHearingRoles, "RandomSourceName");
            Assert.IsEmpty(result);
        }


        #endregion

        #region GetCompanyHearingRoleIdsWithSingleInvitationSourceMapping
        [Test]
        public void GetCompanyHearingRoleIdsWithSingleInvitationSourceMapping_ReturnsExpectedIds()
        {
            var companyHearingRoles = new List<CompanyHearingRole>
            {
                new CompanyHearingRole
                {
                    Id = 1,
                    InvitationSourceMappings = new List<InvitationSourceMapping>
                    {
                        new InvitationSourceMapping { Id = 1, SourceName = "DeleteMe" }
                    }
                },
                new CompanyHearingRole
                {
                    Id = 2,
                    InvitationSourceMappings = new List<InvitationSourceMapping>
                    {
                        new InvitationSourceMapping { Id = 2, SourceName = "DeleteMe" },
                        new InvitationSourceMapping { Id = 3, SourceName = "NotDeleted" }
                    }
                },
                new CompanyHearingRole
                {
                    Id = 3,
                    InvitationSourceMappings = new List<InvitationSourceMapping>
                    {
                        new InvitationSourceMapping { Id = 4, SourceName = "NotDeleted" }
                    }
                },
                new CompanyHearingRole
                {
                    Id = 4,
                    InvitationSourceMappings = new List<InvitationSourceMapping>
                    {
                        new InvitationSourceMapping { Id = 5, SourceName = "DeleteMe" },
                        new InvitationSourceMapping { Id = 6, SourceName = "DeleteMe" }
                    }
                }
            };
            var result = _service.GetCompanyHearingRoleIdsWithSingleInvitationSourceMapping(companyHearingRoles, "DeleteMe");
            Assert.AreEqual(new List<int> { 1, 4 }, result);
        }
        #endregion

        #region GetUserHearingRoleIdsWithSingleInvitationSourceMapping
        [Test]
        public void GetUserHearingRoleIdsWithSingleInvitationSourceMapping_ReturnsExpectedIds()
        {
            var userHearingRoles = new List<UserHearingRole>
            {
                new UserHearingRole
                {
                    Id = 1,
                    InvitationSourceMappings = new List<InvitationSourceMapping>
                    {
                        new InvitationSourceMapping { Id = 1, SourceName = "DeleteMe" }
                    }
                },
                new UserHearingRole
                {
                    Id = 2,
                    InvitationSourceMappings = new List<InvitationSourceMapping>
                    {
                        new InvitationSourceMapping { Id = 2, SourceName = "DeleteMe" },
                        new InvitationSourceMapping { Id = 3, SourceName = "NotDeleted" }
                    }
                },
                new UserHearingRole
                {
                    Id = 3,
                    InvitationSourceMappings = new List<InvitationSourceMapping>
                    {
                        new InvitationSourceMapping { Id = 4, SourceName = "NotDeleted" }
                    }
                },
                new UserHearingRole
                {
                    Id = 4,
                    InvitationSourceMappings = new List<InvitationSourceMapping>
                    {
                        new InvitationSourceMapping { Id = 5, SourceName = "DeleteMe" },
                        new InvitationSourceMapping { Id = 6, SourceName = "DeleteMe" }
                    }
                }
            };
            var result = _service.GetUserHearingRoleIdsWithSingleInvitationSourceMapping(userHearingRoles, "DeleteMe");
            Assert.AreEqual(new List<int> { 1, 4 }, result);
        }
        #endregion

        #region GetInvitationSourceMappingsToDeleteFromAllSources
        [Test]
        public void GetInvitationSourceMappingsToDeleteFromAllSources_WithMixedInputs_ReturnsAllRelatedMappings()
        {
            var canDelete = new InvitationSource { CanDeleteIndividuals = true };
            var cannotDelete = new InvitationSource { CanDeleteIndividuals = false };

            var invitationSourceMappingsToDelete = new List<InvitationSourceMapping>
            {
                new() { Id = 1, UserHearingRoleId = 10, InvitationSource = canDelete },
                new() { Id = 2, CompanyHearingRoleId = 20, InvitationSource = canDelete },
                new() { Id = 3, UserHearingRoleId = 11, InvitationSource = cannotDelete },
            };

            var userHearingRole10 = new UserHearingRole
            {
                Id = 10,
                InvitationSourceMappings = new List<InvitationSourceMapping>
                {
                    new InvitationSourceMapping { Id = 1, UserHearingRoleId = 10, InvitationSource = canDelete },
                    new InvitationSourceMapping { Id = 4, UserHearingRoleId = 10, InvitationSource = canDelete }
                }
            };

            var userHearingRole11 = new UserHearingRole
            {
                Id = 11,
                InvitationSourceMappings = new List<InvitationSourceMapping>
                {
                    new InvitationSourceMapping { Id = 3, UserHearingRoleId = 11, InvitationSource = cannotDelete }
                }
            };

            var companyHearingRole20 = new CompanyHearingRole
            {
                Id = 20,
                InvitationSourceMappings = new List<InvitationSourceMapping>
                {
                    new InvitationSourceMapping { Id = 2, CompanyHearingRoleId = 20, InvitationSource = canDelete },
                    new InvitationSourceMapping { Id = 5, CompanyHearingRoleId = 20, InvitationSource = canDelete }
                }
            };

            var allUserHearingRolesOnHearing = new List<UserHearingRole> { userHearingRole10, userHearingRole11 };
            var allCompanyHearingRolesOnHearing = new List<CompanyHearingRole> { companyHearingRole20 };

            var result = _service.GetInvitationSourceMappingsToDeleteFromAllSources(
                invitationSourceMappingsToDelete,
                allUserHearingRolesOnHearing,
                allCompanyHearingRolesOnHearing
            );

            Assert.That(result.UserHearingRoleIdsToRemove.Count, Is.EqualTo(2));
            Assert.That(result.UserHearingRoleIdsToRemove, Does.Contain(10));
            Assert.That(result.UserHearingRoleIdsToRemove, Does.Contain(11));

            Assert.That(result.CompanyHearingRoleIdsToRemove.Count, Is.EqualTo(1));
            Assert.That(result.CompanyHearingRoleIdsToRemove, Does.Contain(20));

            Assert.That(result.InvitationSourceMappingIdsToRemove.Count, Is.EqualTo(4));
            Assert.That(result.InvitationSourceMappingIdsToRemove, Does.Contain(1));
            Assert.That(result.InvitationSourceMappingIdsToRemove, Does.Contain(4));
            Assert.That(result.InvitationSourceMappingIdsToRemove, Does.Contain(2));
            Assert.That(result.InvitationSourceMappingIdsToRemove, Does.Contain(5));
            Assert.That(result.InvitationSourceMappingIdsToRemove, Does.Not.Contain(3));

            Assert.That(result.InvitationSourceMappingsWithoutIndividualDeletion.Count, Is.EqualTo(1));
            Assert.That(result.InvitationSourceMappingsWithoutIndividualDeletion, Does.Contain(3));
        }

        [Test]
        public void GetInvitationSourceMappingsToDeleteFromAllSources_WithEmptyInput_ReturnsEmptyResponse()
        {
            var invitationSourceMappingsToDelete = new List<InvitationSourceMapping>();
            var allUserHearingRolesOnHearing = new List<UserHearingRole>();
            var allCompanyHearingRolesOnHearing = new List<CompanyHearingRole>();

            var result = _service.GetInvitationSourceMappingsToDeleteFromAllSources(
                invitationSourceMappingsToDelete,
                allUserHearingRolesOnHearing,
                allCompanyHearingRolesOnHearing
            );

            Assert.That(result.InvitationSourceMappingIdsToRemove, Is.Empty);
            Assert.That(result.UserHearingRoleIdsToRemove, Is.Empty);
            Assert.That(result.CompanyHearingRoleIdsToRemove, Is.Empty);
            Assert.That(result.InvitationSourceMappingsWithoutIndividualDeletion, Is.Empty);
        }


        #endregion

        #region GetInvitationSourceMappingsToDelete

        [Test]
        public void GetInvitationSourceMappingsToDelete_WhenIndividualDeletionIsNotAllowed_ReturnsResponseWithOnlyNonDeletableMappings()
        {
            var cannotDelete = new InvitationSource { CanDeleteIndividuals = false };
            var invitationSourceMappingsToDelete = new List<InvitationSourceMapping>
            {
                new() { Id = 1, InvitationSource = cannotDelete },
                new() { Id = 2, InvitationSource = cannotDelete }
            };
            var allUserHearingRolesOnHearing = new List<UserHearingRole>();
            var allCompanyHearingRolesOnHearing = new List<CompanyHearingRole>();

            var result = _service.GetInvitationSourceMappingsToDelete(
                invitationSourceMappingsToDelete,
                allUserHearingRolesOnHearing,
                allCompanyHearingRolesOnHearing
            );

            Assert.That(result.InvitationSourceMappingsWithoutIndividualDeletion.Count, Is.EqualTo(2));
            Assert.That(result.InvitationSourceMappingsWithoutIndividualDeletion, Does.Contain(1));
            Assert.That(result.InvitationSourceMappingsWithoutIndividualDeletion, Does.Contain(2));

            Assert.That(result.InvitationSourceMappingIdsToRemove, Is.Null);
            Assert.That(result.UserHearingRoleIdsToRemove, Is.Null);
            Assert.That(result.CompanyHearingRoleIdsToRemove, Is.Null);
        }

        [Test]
        public void GetInvitationSourceMappingsToDelete_WhenRolesHaveOneMapping_ReturnsCorrectRoleIdsForDeletion()
        {
            var canDelete = new InvitationSource { CanDeleteIndividuals = true };
            var invitationSourceMappingsToDelete = new List<InvitationSourceMapping>
            {
                new() { Id = 1, UserHearingRoleId = 10, InvitationSource = canDelete },
                new() { Id = 2, CompanyHearingRoleId = 20, InvitationSource = canDelete },
                new() { Id = 3, CompanyHearingRoleId = 20, InvitationSource = canDelete }
            };

            var allUserHearingRolesOnHearing = new List<UserHearingRole>
            {
                new() { Id = 10, InvitationSourceMappings = { new() { Id = 1 } } }
            };

            var allCompanyHearingRolesOnHearing = new List<CompanyHearingRole>
            {
                new() { Id = 20, InvitationSourceMappings = { new() { Id = 2 }, new() { Id = 3 } } }
            };

            var result = _service.GetInvitationSourceMappingsToDelete(
                invitationSourceMappingsToDelete,
                allUserHearingRolesOnHearing,
                allCompanyHearingRolesOnHearing
            );

            Assert.That(result.InvitationSourceMappingsWithoutIndividualDeletion, Is.Empty);

            Assert.That(result.InvitationSourceMappingIdsToRemove.Count, Is.EqualTo(3));
            Assert.That(result.InvitationSourceMappingIdsToRemove, Does.Contain(1));
            Assert.That(result.InvitationSourceMappingIdsToRemove, Does.Contain(2));
            Assert.That(result.InvitationSourceMappingIdsToRemove, Does.Contain(3));

            Assert.That(result.UserHearingRoleIdsToRemove.Count, Is.EqualTo(1));
            Assert.That(result.UserHearingRoleIdsToRemove, Does.Contain(10));

            Assert.That(result.CompanyHearingRoleIdsToRemove.Count, Is.EqualTo(1));
            Assert.That(result.CompanyHearingRoleIdsToRemove, Does.Contain(20));
        }

        [Test]
        public void GetInvitationSourceMappingsToDelete_WhenRolesHaveMoreThanOneMapping_DoesNotReturnRoleIdsForDeletion()
        {
            // Arrange
            var canDelete = new InvitationSource { CanDeleteIndividuals = true };
            var invitationSourceMappingsToDelete = new List<InvitationSourceMapping>
            {
                new() { Id = 1, UserHearingRoleId = 10, InvitationSource = canDelete },
                new() { Id = 2, CompanyHearingRoleId = 20, InvitationSource = canDelete }
            };

            var allUserHearingRolesOnHearing = new List<UserHearingRole>
            {
                new() { Id = 10, InvitationSourceMappings = { new() { Id = 1 }, new() { Id = 3 } } }
            };

            var allCompanyHearingRolesOnHearing = new List<CompanyHearingRole>
            {
                new() { Id = 20, InvitationSourceMappings = { new() { Id = 2 }, new() { Id = 4 } } }
            };

            var result = _service.GetInvitationSourceMappingsToDelete(
                invitationSourceMappingsToDelete,
                allUserHearingRolesOnHearing,
                allCompanyHearingRolesOnHearing
            );

            Assert.That(result.InvitationSourceMappingsWithoutIndividualDeletion, Is.Empty);

            Assert.That(result.InvitationSourceMappingIdsToRemove.Count, Is.EqualTo(2));
            Assert.That(result.InvitationSourceMappingIdsToRemove, Does.Contain(1));
            Assert.That(result.InvitationSourceMappingIdsToRemove, Does.Contain(2));

            Assert.That(result.UserHearingRoleIdsToRemove, Is.Empty);
            Assert.That(result.CompanyHearingRoleIdsToRemove, Is.Empty);
        }

        #endregion
    }
}
