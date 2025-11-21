using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class HearingRoleConfiguration : AuditableEntityTypeConfiguration<HearingRoleEntity>
    {
        public override void Configure(EntityTypeBuilder<HearingRoleEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.Name).HasMaxLength(100);
        }
    }
}