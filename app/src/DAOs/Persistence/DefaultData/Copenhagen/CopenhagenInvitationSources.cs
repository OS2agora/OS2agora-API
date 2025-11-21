using Agora.Entities.Entities;
using Agora.Entities.Enums;
using System.Collections.Generic;

namespace Agora.DAOs.Persistence.DefaultData.Copenhagen;

public static class CopenhagenInvitationSources
{
    public static List<InvitationSourceEntity> GetEntities()
    {
        return new List<InvitationSourceEntity>
        {
            new InvitationSourceEntity
            {
                Name = "KK Kort",
                CanDeleteIndividuals = false,
                InvitationSourceType = InvitationSourceType.EXCEL,
                CprColumnHeader = "personnr",
                EmailColumnHeader = "Email",
                CvrColumnHeader = "CVR"
            }
        };
    }
}