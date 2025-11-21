using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class InvitationSourceConfiguration : AuditableEntityTypeConfiguration<InvitationSourceEntity>
    {
        public override void Configure(EntityTypeBuilder<InvitationSourceEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.Name).HasMaxLength(100);
            builder.Property(content => content.CprColumnHeader).HasMaxLength(100);
            builder.Property(content => content.EmailColumnHeader).HasMaxLength(100);
            builder.Property(content => content.CvrColumnHeader).HasMaxLength(100);
        }
    }
}

