using Agora.Models.Common.Services.InvitationService;
using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using Agora.Operations.Common.Extensions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Services;
using Agora.Operations.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HearingRoleEnum = Agora.Models.Enums.HearingRole;
using UserCapacityEnum = Agora.Models.Enums.UserCapacity;

namespace Agora.Operations.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IUserCapacityDao _userCapacityDao;
        private readonly IHearingRoleResolver _hearingRoleResolver;


        public InvitationService(IUserCapacityDao userCapacityDao, IHearingRoleResolver hearingRoleResolver)
        {
            _userCapacityDao = userCapacityDao;
            _hearingRoleResolver = hearingRoleResolver;
        }

        #region InviteeIdentifiers


        public (bool isValid, List<Exception> errors) NormalizeAndValidateInviteeIdentifiers(List<InviteeIdentifiers> inviteeIdentifiers)
        {
            var errors = new List<Exception>();
            var distinctItems = new List<InviteeIdentifiers>();

            var distinctCprs = new HashSet<string>();
            var distinctCvrs = new HashSet<string>();
            var distinctEmails = new HashSet<string>();

            foreach (var identifier in inviteeIdentifiers)
            {
                try
                {
                    if (!string.IsNullOrEmpty(identifier.Cpr))
                    {
                        var normalizedCpr = identifier.Cpr.NormalizeAndValidateCpr();
                        if (distinctCprs.Add(normalizedCpr))
                        {
                            distinctItems.Add(new InviteeIdentifiers { Cpr = normalizedCpr });
                        }
                    }
                    else if (!string.IsNullOrEmpty(identifier.Cvr))
                    {
                        var normalizedCvr = identifier.Cvr.NormalizeAndValidateCvr();
                        if (distinctCvrs.Add(normalizedCvr))
                        {
                            distinctItems.Add(new InviteeIdentifiers { Cvr = normalizedCvr });
                        }
                    }
                    else if (!string.IsNullOrEmpty(identifier.Email))
                    {
                        var normalizedEmail = identifier.Email.NormalizeAndValidateEmail();
                        if (distinctEmails.Add(normalizedEmail))
                        {
                            distinctItems.Add(new InviteeIdentifiers { Email = normalizedEmail });
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }

            inviteeIdentifiers.Clear();
            inviteeIdentifiers.AddRange(distinctItems);

            return (!errors.Any(), errors);
        }

        public (List<string>, List<string>, List<string>) SplitInviteeIdentifierList(List<InviteeIdentifiers> inviteeIdentifiers)
        {
            var cvrIdentifiers = inviteeIdentifiers.Where(inviteeIdentifier =>
                !string.IsNullOrEmpty(inviteeIdentifier.Cvr)).Select(identifier => identifier.Cvr).Distinct().ToList();
            var emailIdentifiers = inviteeIdentifiers.Where(inviteeIdentifier =>
                !string.IsNullOrEmpty(inviteeIdentifier.Email)).Select(identifier => identifier.Email.ToLowerInvariant()).Distinct().ToList();
            var cprIdentifiers = inviteeIdentifiers.Where(inviteeIdentifier =>
                !string.IsNullOrEmpty(inviteeIdentifier.Cpr)).Select(identifier => identifier.Cpr).Distinct().ToList();

            return (cprIdentifiers, emailIdentifiers, cvrIdentifiers);
        }

        public (List<InviteeIdentifiers>, List<InviteeIdentifiers>) GetNewAndExistingInvitees(List<UserHearingRole> currentUserHearingRoles,
            List<CompanyHearingRole> currentCompanyHearingRoles, List<InviteeIdentifiers> inviteeIdentifiers)
        {
            var newInvitees = new List<InviteeIdentifiers>();
            var existingInvitees = new List<InviteeIdentifiers>();

            foreach (var identifier in inviteeIdentifiers)
            {
                var emailIsInCurrentUserHearingRoles = identifier.Email != null &&
                                                       currentUserHearingRoles.Any(x =>
                                                           string.Equals(x.User.Email, identifier.Email, StringComparison.CurrentCultureIgnoreCase));
                var cprIsInCurrentUserHearingRoles = identifier.Cpr != null &&
                                                     currentUserHearingRoles.Any(x =>
                                                         string.Equals(x.User.PersonalIdentifier, identifier.Cpr, StringComparison.CurrentCultureIgnoreCase));
                var cvrIsInCurrentUserHearingRoles = identifier.Cvr != null &&
                                                     currentCompanyHearingRoles.Any(x =>
                                                         string.Equals(x.Company.Cvr, identifier.Cvr, StringComparison.CurrentCultureIgnoreCase));
                var isNewIdentifier = !emailIsInCurrentUserHearingRoles && !cprIsInCurrentUserHearingRoles &&
                              !cvrIsInCurrentUserHearingRoles;

                if (isNewIdentifier)
                {
                    newInvitees.Add(identifier);
                    continue;
                }

                existingInvitees.Add(identifier);
            }

            return (newInvitees, existingInvitees);
        }

        #endregion

        #region UserHearingRoles
        public async Task<List<UserHearingRole>> GetUserHearingRolesToCreate(int hearingId, List<int> userIds)
        {
            if (!userIds.Any())
            {
                return new List<UserHearingRole>();
            }

            var hearingRole = await _hearingRoleResolver.GetHearingRole(HearingRoleEnum.HEARING_INVITEE);

            var userHearingRolesToCreate = userIds.Select(id =>
                new UserHearingRole
                {
                    HearingRoleId = hearingRole.Id,
                    UserId = id,
                    HearingId = hearingId
                }).ToList();

            return userHearingRolesToCreate;
        }

        public List<int> GetIdsForUserHearingRolesWithoutInvitationSourceMappings(List<UserHearingRole> userHearingRoles, List<string> cprIdentifiers, List<string> emailIdentifiers, int sourceId, string sourceName)
        {
            var userHearingRoleIdsWithoutMapping =
                userHearingRoles.Where(uhr => !uhr.InvitationSourceMappings.Any(ism => ism.SourceName == sourceName && ism.InvitationSourceId == sourceId)
                                              && (cprIdentifiers.Contains(uhr.User.Cpr) || emailIdentifiers.Contains(uhr.User.Email)))
                    .Select(uhr => uhr.Id).ToList();

            return userHearingRoleIdsWithoutMapping;
        }

        public List<int> GetInvitationSourceMappingIdsFromUserHearingRoles(IEnumerable<UserHearingRole> userHearingRoles, string invitationSourceName)
        {
            var mappingIds = userHearingRoles.SelectMany(uhr => uhr.InvitationSourceMappings)
                .Where(sourceMapping => sourceMapping.SourceName == invitationSourceName)
                .Select(sourceMapping => sourceMapping.Id)
                .ToList();
            return mappingIds;
        }

        public List<int> GetUserHearingRoleIdsWithSingleInvitationSourceMapping(
            IEnumerable<UserHearingRole> userHearingRoles, string invitationSourceName)
        {
            var userHearingRoleIds = userHearingRoles.Where(uhr => uhr.InvitationSourceMappings.All(sourceMapping =>
                    sourceMapping.SourceName == invitationSourceName)).Select(uhr => uhr.Id).ToList();
            return userHearingRoleIds;
        }
        #endregion 

        #region CompanyHearingRoles
        public async Task<List<CompanyHearingRole>> GetCompanyHearingRolesToCreate(int hearingId, List<int> companyIds)
        {
            if (!companyIds.Any())
            {
                return new List<CompanyHearingRole>();
            }

            var hearingRole = await _hearingRoleResolver.GetHearingRole(HearingRoleEnum.HEARING_INVITEE);

            var companyHearingRolesToCreate = companyIds.Select(id =>
                new CompanyHearingRole
                {
                    HearingRoleId = hearingRole.Id,
                    CompanyId = id,
                    HearingId = hearingId
                }).ToList();

            return companyHearingRolesToCreate;
        }

        public List<int> GetIdsForCompanyHearingRolesWithoutInvitationSourceMappings(List<CompanyHearingRole> companyHearingRoles,
            List<string> cvrIdentifiers, int sourceId, string sourceName)
        {
            var companyHearingRoleIdsWithoutMapping = companyHearingRoles
                .Where(chr => cvrIdentifiers.Contains(chr.Company.Cvr) && !chr.InvitationSourceMappings.Any(ism =>
                    ism.SourceName == sourceName && ism.InvitationSourceId == sourceId)).Select(chr => chr.Id).ToList();

            return companyHearingRoleIdsWithoutMapping;
        }

        public List<int> GetInvitationSourceMappingIdsFromCompanyHearingRoles(IEnumerable<CompanyHearingRole> companyHearingRoles, string invitationSourceName)
        {
            var mappingIds = companyHearingRoles.SelectMany(chr => chr.InvitationSourceMappings)
                .Where(sourceMapping => sourceMapping.SourceName == invitationSourceName)
                .Select(sourceMapping => sourceMapping.Id)
                .ToList();
            return mappingIds;
        }

        public List<int> GetCompanyHearingRoleIdsWithSingleInvitationSourceMapping(
            IEnumerable<CompanyHearingRole> companyHearingRoles, string invitationSourceName)
        {
            var companyHearingRoleIds = companyHearingRoles.Where(chr => chr.InvitationSourceMappings.All(sourceMapping =>
                    sourceMapping.SourceName == invitationSourceName)).Select(chr => chr.Id).ToList();
            return companyHearingRoleIds;
        }

        #endregion 

        #region Invitation relations - UserHearingRoles, CompanyHearingRoles and InvitationSourceMappings
        public (int[], int[]) GetUserInviteeRelationsToRemove(List<UserHearingRole> userHearingRoles,
            List<string> cprIdentifiers, List<string> emailIdentifiers, int sourceId, string sourceName)
        {
            var userHearingRolesToHandle =
                userHearingRoles.Where(uhr =>
                    (!uhr.InvitationSourceMappings.Any() || uhr.InvitationSourceMappings.Any(ism => ism.SourceName == sourceName && ism.InvitationSourceId == sourceId))
                    && !cprIdentifiers.Contains(uhr.User.Cpr)
                    && !emailIdentifiers.Contains(uhr.User.Email)).ToList();

            var userHearingRolesToDelete = userHearingRolesToHandle
                .Where(uhr => uhr.InvitationSourceMappings.Count <= 1)
                .Select(uhr => uhr.Id)
                .ToArray();

            var mappingsToDelete = userHearingRolesToHandle.Where(uhr => !userHearingRolesToDelete.Contains(uhr.Id))
                .Select(uhr => uhr.InvitationSourceMappings.Single(ism =>
                    ism.SourceName == sourceName && ism.InvitationSourceId == sourceId).Id)
                .ToArray();

            return (userHearingRolesToDelete, mappingsToDelete);
        }

        public (int[], int[]) GetCompanyInviteeRelationsToRemove(List<CompanyHearingRole> companyHearingRoles,
            List<string> cvrIdentifiers, int sourceId, string sourceName)
        {
            var companyHearingRolesToHandle =
                companyHearingRoles.Where(chr =>
                    (!chr.InvitationSourceMappings.Any() || chr.InvitationSourceMappings.Any(ism => ism.SourceName == sourceName && ism.InvitationSourceId == sourceId))
                    && !cvrIdentifiers.Contains(chr.Company.Cvr)).ToList();

            var companyHearingRolesToDelete = companyHearingRolesToHandle
                .Where(chr => chr.InvitationSourceMappings.Count <= 1)
                .Select(chr => chr.Id)
                .ToArray();

            var mappingsToDelete = companyHearingRolesToHandle.Where(chr => !companyHearingRolesToDelete.Contains(chr.Id))
                .Select(chr => chr.InvitationSourceMappings.Single(ism =>
                    ism.SourceName == sourceName && ism.InvitationSourceId == sourceId).Id)
                .ToArray();

            return (companyHearingRolesToDelete, mappingsToDelete);
        }

        public GetInvitationSourceMappingsToDeleteResponse GetInvitationSourceMappingsToDeleteFromAllSources(
            List<InvitationSourceMapping> invitationSourceMappingsToDelete,
            List<UserHearingRole> allUserHearingRolesOnHearing,
            List<CompanyHearingRole> allCompanyHearingRolesOnHearing)
        {

            var userHearingRoleIdsToRemove = invitationSourceMappingsToDelete
                .Where(ism => ism.UserHearingRoleId.HasValue).Select(ism => ism.UserHearingRoleId.Value).ToList();

            var companyHearingRoleIdsToRemove = invitationSourceMappingsToDelete
                .Where(ism => ism.CompanyHearingRoleId.HasValue).Select(ism => ism.CompanyHearingRoleId.Value).ToList();

            var allInvitationSourceMappingsToDelete = new List<InvitationSourceMapping>();

            allInvitationSourceMappingsToDelete.AddRange(allUserHearingRolesOnHearing
                .Where(uhr => userHearingRoleIdsToRemove.Contains(uhr.Id))
                .SelectMany(uhr => uhr.InvitationSourceMappings));

            allInvitationSourceMappingsToDelete.AddRange(allCompanyHearingRolesOnHearing
                .Where(chr => companyHearingRoleIdsToRemove.Contains(chr.Id))
                .SelectMany(chr => chr.InvitationSourceMappings));

            GetInvitationSourceMappingsWhichCanBeRemoved(allInvitationSourceMappingsToDelete,
                out var invitationSourceMappingIdsToRemove, out var invitationSourceMappingsWithoutIndividualDeletion);

            return new GetInvitationSourceMappingsToDeleteResponse
            {
                InvitationSourceMappingIdsToRemove = invitationSourceMappingIdsToRemove.ToList(),
                UserHearingRoleIdsToRemove = userHearingRoleIdsToRemove,
                CompanyHearingRoleIdsToRemove = companyHearingRoleIdsToRemove,
                InvitationSourceMappingsWithoutIndividualDeletion =
                    invitationSourceMappingsWithoutIndividualDeletion
            };
        }

        public GetInvitationSourceMappingsToDeleteResponse GetInvitationSourceMappingsToDelete(
                List<InvitationSourceMapping> invitationSourceMappingsToDelete,
                List<UserHearingRole> allUserHearingRolesOnHearing,
                List<CompanyHearingRole> allCompanyHearingRolesOnHearing)
        {

            GetInvitationSourceMappingsWhichCanBeRemoved(invitationSourceMappingsToDelete, out var invitationSourceMappingIdsToRemove, out var invitationSourceMappingsWithoutIndividualDeletion);

            if (invitationSourceMappingsWithoutIndividualDeletion.Any())
            {
                return new GetInvitationSourceMappingsToDeleteResponse
                {
                    InvitationSourceMappingsWithoutIndividualDeletion =
                        invitationSourceMappingsWithoutIndividualDeletion
                };
            }

            var userHearingRoleIdsToRemove = allUserHearingRolesOnHearing.Where(uhr =>
                uhr.InvitationSourceMappings.All(ism => invitationSourceMappingIdsToRemove.Contains(ism.Id))).Select(uhr => uhr.Id).ToList();

            var companyHearingRoleIdsToRemove = allCompanyHearingRolesOnHearing.Where(chr =>
                chr.InvitationSourceMappings.All(ism => invitationSourceMappingIdsToRemove.Contains(ism.Id))).Select(chr => chr.Id).ToList();

            return new GetInvitationSourceMappingsToDeleteResponse
            {
                InvitationSourceMappingIdsToRemove = invitationSourceMappingIdsToRemove.ToList(),
                UserHearingRoleIdsToRemove = userHearingRoleIdsToRemove,
                CompanyHearingRoleIdsToRemove = companyHearingRoleIdsToRemove,
                InvitationSourceMappingsWithoutIndividualDeletion =
                    invitationSourceMappingsWithoutIndividualDeletion
            };
        }

        private void GetInvitationSourceMappingsWhichCanBeRemoved(
            List<InvitationSourceMapping> invitationSourceMappingsToDelete, out HashSet<int> invitationSourceMappingIdsToRemove,
            out List<int> invitationSourceMappingsWithoutIndividualDeletion)
        {
            invitationSourceMappingsWithoutIndividualDeletion = new List<int>();
            invitationSourceMappingIdsToRemove = invitationSourceMappingsToDelete
                .Where(ism => ism.InvitationSource.CanDeleteIndividuals)
                .Select(ism => ism.Id).ToHashSet();

            if (invitationSourceMappingsToDelete.Count != invitationSourceMappingIdsToRemove.Count)
            {
                invitationSourceMappingsWithoutIndividualDeletion.AddRange(invitationSourceMappingsToDelete
                    .Where(ism => !ism.InvitationSource.CanDeleteIndividuals)
                    .Select(ism => ism.Id).ToList());
            }
        }

        #endregion

        #region Users and Companies
        public async Task<List<User>> GetNewUsersToCreate(List<string> cprIdentifiers, List<string> emailIdentifiers,
            List<User> currentUsers)
        {
            var userCapacities = await _userCapacityDao.GetAllAsync();

            var citizenUserCapacity = userCapacities.Single(userCapacity => userCapacity.Capacity == UserCapacityEnum.CITIZEN);
            var newCitizenUsers = GetNewCitizensToCreate(cprIdentifiers, currentUsers, citizenUserCapacity);
            
            var employeeUserCapacity = userCapacities.Single(userCapacity => userCapacity.Capacity == UserCapacityEnum.EMPLOYEE);
            var newEmployeeUsers = GetNewEmployeesToCreate(emailIdentifiers, currentUsers, employeeUserCapacity);
            

            return newCitizenUsers.Concat(newEmployeeUsers).ToList();
        }

        public List<Company> GetNewCompaniesToCreate(List<string> cvrIdentifiers, List<Company> currentCompanies)
        {
            var currentCvrIdentifiers = new HashSet<string>(
                currentCompanies.Where(company => !string.IsNullOrEmpty(company.Cvr))
                    .Select(company => company.Cvr.ToLower()));

            var newCompanies = cvrIdentifiers.Where(cvr => !currentCvrIdentifiers.Contains(cvr))
                .Select(cvr => new Company { Cvr = cvr }).ToList();

            return newCompanies;
        }

        private List<User> GetNewCitizensToCreate(List<string> cprIdentifiers, List<User> currentUsers, UserCapacity citizenUserCapacity)
        {
            var currentCprIdentifiers = new HashSet<string>(
                currentUsers.Where(user => !string.IsNullOrEmpty(user.Cpr) && !string.IsNullOrEmpty(user.PersonalIdentifier) && user.Cpr == user.PersonalIdentifier)
                    .Select(user => user.Cpr));

            var newCitizenUsers = cprIdentifiers.Where(cpr => !currentCprIdentifiers.Contains(cpr))
                .Select(cpr => new User
                {
                    PersonalIdentifier = cpr,
                    Cpr = cpr,
                    UserCapacityId = citizenUserCapacity.Id
                }).ToList();

            return newCitizenUsers;
        }

        private List<User> GetNewEmployeesToCreate(List<string> emailIdentifiers, List<User> currentUsers, UserCapacity employeeUserCapacity)
        {
            var currentEmailIdentifiers = new HashSet<string>(
                currentUsers.Where(user => !string.IsNullOrEmpty(user.Email))
                    .Select(user => user.Email.ToLowerInvariant()), StringComparer.OrdinalIgnoreCase);

            var newEmployeeUsers = emailIdentifiers.Where(email => !currentEmailIdentifiers.Contains(email.ToLowerInvariant()))
                .Select(email => new User
                {
                    Email = email.ToLowerInvariant(),
                    UserCapacityId = employeeUserCapacity.Id
                }).ToList();

            return newEmployeeUsers;
        }

        #endregion
    }
}

