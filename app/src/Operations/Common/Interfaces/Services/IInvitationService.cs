using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agora.Models.Common.Services.InvitationService;

namespace Agora.Operations.Common.Interfaces.Services
{
    public interface IInvitationService
    {
        // InviteeIdentifiers
        (bool isValid, List<Exception> errors) NormalizeAndValidateInviteeIdentifiers(List<InviteeIdentifiers> inviteeIdentifiers);

        (List<InviteeIdentifiers>, List<InviteeIdentifiers>) GetNewAndExistingInvitees(List<UserHearingRole> currentUserHearingRoles, List<CompanyHearingRole> currentCompanyHearingRoles, List<InviteeIdentifiers> inviteeIdentifiers);

        (List<string>, List<string>, List<string>) SplitInviteeIdentifierList(List<InviteeIdentifiers> inviteeIdentifiers);


        // UserHearingRoles
        Task<List<UserHearingRole>> GetUserHearingRolesToCreate(int hearingId, List<int> userIds);

        List<int> GetIdsForUserHearingRolesWithoutInvitationSourceMappings(List<UserHearingRole> userHearingRoles,
            List<string> cprIdentifiers, List<string> emailIdentifiers, int sourceId, string sourceName);

        List<int> GetInvitationSourceMappingIdsFromUserHearingRoles(IEnumerable<UserHearingRole> userHearingRoles, string invitationSourceName);

        List<int> GetUserHearingRoleIdsWithSingleInvitationSourceMapping(
            IEnumerable<UserHearingRole> userHearingRoles, string invitationSourceName);


        // CompanyHearingRoles
        Task<List<CompanyHearingRole>> GetCompanyHearingRolesToCreate(int hearingId, List<int> companyIds);

        List<int> GetIdsForCompanyHearingRolesWithoutInvitationSourceMappings(List<CompanyHearingRole> companyHearingRoles,
            List<string> cvrIdentifiers, int sourceId, string sourceName);

        List<int> GetInvitationSourceMappingIdsFromCompanyHearingRoles(IEnumerable<CompanyHearingRole> companyHearingRoles,
            string invitationSourceName);

        List<int> GetCompanyHearingRoleIdsWithSingleInvitationSourceMapping(
            IEnumerable<CompanyHearingRole> companyHearingRoles, string invitationSourceName);


        // Invitation Relations - UserHearingRoles, CompanyHearingRoles and InvitationSourceMappings
        (int[], int[]) GetUserInviteeRelationsToRemove(List<UserHearingRole> userHearingRoles,
            List<string> cprIdentifiers, List<string> emailIdentifiers, int sourceId, string sourceName);

        (int[], int[]) GetCompanyInviteeRelationsToRemove(List<CompanyHearingRole> companyHearingRoles,
            List<string> cvrIdentifiers, int sourceId, string sourceName);

        GetInvitationSourceMappingsToDeleteResponse GetInvitationSourceMappingsToDelete(
            List<InvitationSourceMapping> invitationSourceMappingsToDelete,
            List<UserHearingRole> allUserHearingRolesOnHearing,
            List<CompanyHearingRole> allCompanyHearingRolesOnHearing);

        GetInvitationSourceMappingsToDeleteResponse GetInvitationSourceMappingsToDeleteFromAllSources(
            List<InvitationSourceMapping> invitationSourceMappingsToDelete,
            List<UserHearingRole> allUserHearingRolesOnHearing,
            List<CompanyHearingRole> allCompanyHearingRolesOnHearing);

        // Users and Companies
        Task<List<User>> GetNewUsersToCreate(List<string> cprIdentifiers, List<string> emailIdentifiers,
            List<User> currentUsers);
        List<Company> GetNewCompaniesToCreate(List<string> cvrIdentifiers, List<Company> currentCompanies);
    }
}