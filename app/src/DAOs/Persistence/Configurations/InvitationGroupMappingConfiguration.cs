using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class InvitationGroupMappingConfiguration : AuditableEntityTypeConfiguration<InvitationGroupMappingEntity>
    {
        public override void Configure(EntityTypeBuilder<InvitationGroupMappingEntity> builder)
        {
            base.Configure(builder);
        }
    }
}