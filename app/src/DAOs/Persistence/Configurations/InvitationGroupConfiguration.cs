using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class InvitationGroupConfiguration : AuditableEntityTypeConfiguration<InvitationGroupEntity>
    {
        public override void Configure(EntityTypeBuilder<InvitationGroupEntity> builder)
        {
            base.Configure(builder);
            builder.Property(content => content.Name).IsRequired().HasMaxLength(50);
        }
    }
}