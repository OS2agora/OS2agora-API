using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using Agora.Operations.Common.Extensions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Plugins;
using Agora.Operations.Common.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HearingRole = Agora.Models.Enums.HearingRole;

namespace Agora.Operations.Services
{
    public class InvitationHandler : IInvitationHandler
    {
        private readonly ICompanyDao _companyDao;
        private readonly ICompanyHearingRoleDao _companyHearingRoleDao;
        private readonly IHearingDao _hearingDao;
        private readonly IInvitationSourceMappingDao _invitationSourceMappingDao;
        private readonly IUserDao _userDao;
        private readonly IUserHearingRoleDao _userHearingRoleDao;

        private readonly IInvitationService _invitationService;
        private readonly IPluginService _pluginService;

        public InvitationHandler(
            ICompanyDao companyDao,
            ICompanyHearingRoleDao companyHearingRoleDao,
            IHearingDao hearingDao,
            IInvitationSourceMappingDao invitationSourceMappingDao,
            IUserDao userDao,
            IUserHearingRoleDao userHearingRoleDao,
            IInvitationService invitationService,
            IPluginService pluginService)
        {
            _companyDao = companyDao;
            _companyHearingRoleDao = companyHearingRoleDao;
            _hearingDao = hearingDao;
            _invitationSourceMappingDao = invitationSourceMappingDao;
            _userDao = userDao;
            _userHearingRoleDao = userHearingRoleDao;

            _invitationService = invitationService;
            _pluginService = pluginService;
        }

        public async Task<InvitationMetaData> CreateInviteesForHearing(Hearing hearing, int sourceId, string sourceName, List<InviteeIdentifiers> inviteeIdentifiers)
        {
            return await CreateNewInvitees(hearing, inviteeIdentifiers, sourceId, sourceName);
        }

        public async Task<InvitationMetaData> ReplaceInviteesForHearing(Hearing hearing, int sourceId, string sourceName, List<InviteeIdentifiers> inviteeIdentifiers)
        {
            var createdMetaData = await CreateNewInvitees(hearing, inviteeIdentifiers, sourceId, sourceName);
            var deletedMetaData = await DeleteInvitees(hearing, inviteeIdentifiers, sourceId, sourceName);

            return new InvitationMetaData
            {
                NewInvitees = createdMetaData.NewInvitees,
                ExistingInvitees = createdMetaData.ExistingInvitees,
                DeletedInvitees = deletedMetaData.DeletedInvitees,
                InviteesNotDeleted = deletedMetaData.InviteesNotDeleted
            };
        }

        private async Task<InvitationMetaData> DeleteInvitees(Hearing hearing, List<InviteeIdentifiers> inviteeIdentifiers, int sourceId, string sourceName)
        {
            var currentUserInvitees = hearing.UserHearingRoles.Where(x => x.HearingRole.Role == HearingRole.HEARING_INVITEE).ToList();
            var currentCompanyInvitees = hearing.CompanyHearingRoles
                .Where(c => c.HearingRole.Role == HearingRole.HEARING_INVITEE).ToList();

            var (cprIdentifiers, emailIdentifiers, cvrIdentifiers) = _invitationService.SplitInviteeIdentifierList(inviteeIdentifiers);

            var (userHearingRoleIdsToDelete, userInvitationSourceMappingIdsToDelete) = _invitationService.GetUserInviteeRelationsToRemove(currentUserInvitees, cprIdentifiers,
                emailIdentifiers, sourceId, sourceName);

            var (companyHearingRoleIdsToDelete, companyInvitationSourceMappingIdsToDelete) = _invitationService.GetCompanyInviteeRelationsToRemove(currentCompanyInvitees, cvrIdentifiers, sourceId, sourceName);

            var invitationSourceMappingsToDelete = userInvitationSourceMappingIdsToDelete
                .Union(companyInvitationSourceMappingIdsToDelete).ToArray();

            await _userHearingRoleDao.DeleteRangeAsync(userHearingRoleIdsToDelete);
            await _companyHearingRoleDao.DeleteRangeAsync(companyHearingRoleIdsToDelete);
            await _invitationSourceMappingDao.DeleteRangeAsync(invitationSourceMappingsToDelete);

            return new InvitationMetaData
            {
                DeletedInvitees = userHearingRoleIdsToDelete.Length + companyHearingRoleIdsToDelete.Length,
                InviteesNotDeleted = invitationSourceMappingsToDelete.Length
            };
        }

        private async Task<InvitationMetaData> CreateNewInvitees(Hearing hearing, List<InviteeIdentifiers> inviteeIdentifiers, int sourceId, string sourceName)
        {
            var currentUserInvitees = hearing.UserHearingRoles.Where(x => x.HearingRole.Role == HearingRole.HEARING_INVITEE).ToList();
            var currentCompanyInvitees = hearing.CompanyHearingRoles
                .Where(c => c.HearingRole.Role == HearingRole.HEARING_INVITEE).ToList();

            var (newInvitees, existingInvitees) =
                _invitationService.GetNewAndExistingInvitees(currentUserInvitees, currentCompanyInvitees, inviteeIdentifiers);

            var (newCprIdentifiers, newEmailIdentifiers, newCvrIdentifiers) =
                _invitationService.SplitInviteeIdentifierList(newInvitees);

            var newUserInviteeIds = await CreateAndGetNewUserIds(newCprIdentifiers, newEmailIdentifiers);
            var newUserHearingRoleIds = await CreateAndGetNewUserHearingRoleIds(hearing.Id, newUserInviteeIds);

            var newCompanyInviteeIds = await CreateAndGetNewCompanyIds(newCvrIdentifiers);
            var newCompanyHearingRoleIds = await CreateAndGetNewCompanyHearingRoleIds(hearing.Id, newCompanyInviteeIds);
            
            if (hearing.IsPublished())
            {
                await _pluginService.NotifyUsersAfterInvitedToHearing(hearing.Id, newUserInviteeIds);
                await _pluginService.NotifyCompaniesAfterInvitedToHearing(hearing.Id, newCompanyInviteeIds);
            }

            var sourceMappingsForNewInvitees = GetInvitationSourceMappingsForNewInvitees(newUserHearingRoleIds, newCompanyHearingRoleIds, sourceId, sourceName);

            var sourceMappingsForExistingInvitees = GetInvitationSourceMappingsForExistingInvitees(existingInvitees, currentUserInvitees,
                currentCompanyInvitees, sourceId, sourceName);

            var newInvitationSourceMappings =
                sourceMappingsForNewInvitees.Concat(sourceMappingsForExistingInvitees).ToList();

            await _invitationSourceMappingDao.CreateRangeAsync(newInvitationSourceMappings);

            return new InvitationMetaData
            {
                NewInvitees = newInvitees.Count,
                ExistingInvitees = existingInvitees.Count
            };
        }

        private List<InvitationSourceMapping> GetInvitationSourceMappingsForNewInvitees(List<int> newUserHearingRoleIds, List<int> newCompanyHearingRoleIds, int sourceId, string sourceName)
        {
            var newInviteeSourceMappingsForNewUsers = GetInviteeSourceMappingsForNewUsers(newUserHearingRoleIds, sourceId, sourceName);
            var newInviteeSourceMappingsForNewCompanies = GetInviteeSourceMappingsForNewCompanies(newCompanyHearingRoleIds, sourceId, sourceName);

            var newInviteeSourceMappingsForNewInvitees = newInviteeSourceMappingsForNewUsers.Concat(newInviteeSourceMappingsForNewCompanies).ToList();

            return newInviteeSourceMappingsForNewInvitees;
        }

        private List<InvitationSourceMapping> GetInvitationSourceMappingsForExistingInvitees(List<InviteeIdentifiers> existingInvitees, List<UserHearingRole> userHearingRoles, List<CompanyHearingRole> companyHearingRoles, int sourceId, string sourceName)
        {
            var (cprIdentifiers, emailIdentifiers, cvrIdentifiers) = _invitationService.SplitInviteeIdentifierList(existingInvitees);

            var newInviteeSourceMappingsForExistingUsers = GetInviteeSourceMappingsForExistingUsers(userHearingRoles, cprIdentifiers, emailIdentifiers, sourceId, sourceName);
            var newInviteeSourceMappingsForExistingCompanies = GetInviteeSourceMappingsForExistingCompanies(companyHearingRoles, cvrIdentifiers, sourceId, sourceName);

            var newInviteeSourceMappingsForExistingInvitees = newInviteeSourceMappingsForExistingUsers.Concat(newInviteeSourceMappingsForExistingCompanies).ToList();

            return newInviteeSourceMappingsForExistingInvitees;
        }

        private List<InvitationSourceMapping> GetInviteeSourceMappingsForNewUsers(List<int> newUserHearingRoleIds, int sourceId, string sourceName)
        {
            return newUserHearingRoleIds.Select(id => new InvitationSourceMapping
            {
                SourceName = sourceName,
                InvitationSourceId = sourceId,
                UserHearingRoleId = id
            }).ToList();
        }

        private List<InvitationSourceMapping> GetInviteeSourceMappingsForExistingUsers(List<UserHearingRole> userHearingRoles, List<string> cprIdentifiers, List<string> emailIdentifiers, int sourceId, string sourceName)
        {
            var userHearingRolesWithoutMapping =
                _invitationService.GetIdsForUserHearingRolesWithoutInvitationSourceMappings(userHearingRoles, cprIdentifiers,
                    emailIdentifiers, sourceId, sourceName);

            var newInviteeSourceMappings = userHearingRolesWithoutMapping.Select(id => new InvitationSourceMapping
            {
                SourceName = sourceName,
                InvitationSourceId = sourceId,
                UserHearingRoleId = id
            }).ToList();

            return newInviteeSourceMappings;
        }

        private List<InvitationSourceMapping> GetInviteeSourceMappingsForNewCompanies(List<int> newCompanyHearingRoleIds, int sourceId, string sourceName)
        {
            return newCompanyHearingRoleIds.Select(id => new InvitationSourceMapping
            {
                SourceName = sourceName,
                InvitationSourceId = sourceId,
                CompanyHearingRoleId = id
            }).ToList();
        }

        private List<InvitationSourceMapping> GetInviteeSourceMappingsForExistingCompanies(List<CompanyHearingRole> companyHearingRoles, List<string> cvrIdentifiers, int sourceId, string sourceName)
        {
            var companyHearingRolesWithoutMapping =
                _invitationService.GetIdsForCompanyHearingRolesWithoutInvitationSourceMappings(companyHearingRoles, cvrIdentifiers,
                    sourceId, sourceName);

            var newCompanyInviteeSourceMappings = companyHearingRolesWithoutMapping.Select(id => new InvitationSourceMapping
            {
                SourceName = sourceName,
                InvitationSourceId = sourceId,
                CompanyHearingRoleId = id
            }).ToList();

            return newCompanyInviteeSourceMappings;
        }

        private async Task<List<int>> CreateAndGetNewUserIds(List<string> cprIdentifiers, List<string> emailIdentifiers)
        {
            var currentUsers = await _userDao.GetAllAsync();
            var newUsers = await _invitationService.GetNewUsersToCreate(cprIdentifiers, emailIdentifiers, currentUsers);

            if (newUsers.Any())
            {
                currentUsers = await _userDao.CreateRangeAsync(newUsers);
            }

            var userIds = currentUsers
                .Where(user => cprIdentifiers.Contains(user.PersonalIdentifier) || emailIdentifiers.Contains(user.Email))
                .Select(user => user.Id).ToList();

            return userIds;
        }

        private async Task<List<int>> CreateAndGetNewCompanyIds(List<string> cvrIdentifiers)
        {
            var currentCompanies = await _companyDao.GetAllAsync();
            var newCompanies = _invitationService.GetNewCompaniesToCreate(cvrIdentifiers, currentCompanies);

            if (newCompanies.Any())
            {
                currentCompanies = await _companyDao.CreateRangeAsync(newCompanies);
            }

            var companyIds = currentCompanies.Where(company => cvrIdentifiers.Contains(company.Cvr))
                .Select(company => company.Id).ToList();

            return companyIds;
        }

        private async Task<List<int>> CreateAndGetNewUserHearingRoleIds(int hearingId, List<int> userIds)
        {
            var userHearingRolesToCreate = await _invitationService.GetUserHearingRolesToCreate(hearingId, userIds);
            var allUserHearingRoles = await _userHearingRoleDao.CreateRangeAsync(userHearingRolesToCreate);

            var createdUserHearingRoleIds = allUserHearingRoles
                .Where(uhr => userIds.Contains(uhr.UserId) && uhr.HearingId == hearingId)
                .Select(uhr => uhr.Id)
                .ToList();

            return createdUserHearingRoleIds;
        }

        private async Task<List<int>> CreateAndGetNewCompanyHearingRoleIds(int hearingId, List<int> companyIds)
        {
            var companyHearingRolesToCreate = await _invitationService.GetCompanyHearingRolesToCreate(hearingId, companyIds);
            var allCompanyHearingRoles = await _companyHearingRoleDao.CreateRangeAsync(companyHearingRolesToCreate);

            var createdCompanyHearingRoleIds = allCompanyHearingRoles
                .Where(chr => companyIds.Contains(chr.CompanyId) && chr.HearingId == hearingId)
                .Select(chr => chr.Id)
                .ToList();

            return createdCompanyHearingRoleIds;
        }
    }
}
