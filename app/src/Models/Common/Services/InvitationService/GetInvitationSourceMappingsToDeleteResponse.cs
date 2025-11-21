using System.Collections.Generic;

namespace Agora.Models.Common.Services.InvitationService
{
    public class GetInvitationSourceMappingsToDeleteResponse
    {
        public List<int> InvitationSourceMappingIdsToRemove { get; set; }
        public List<int> UserHearingRoleIdsToRemove { get; set; }
        public List<int> CompanyHearingRoleIdsToRemove { get; set; }
        public List<int> InvitationSourceMappingsWithoutIndividualDeletion { get; set; }
    }
}