using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class InvitationSourceMappingConfiguration : AuditableEntityTypeConfiguration<InvitationSourceMappingEntity>
    {
        public override void Configure(EntityTypeBuilder<InvitationSourceMappingEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.SourceName).HasMaxLength(500);

            builder.HasOne(ism => ism.CompanyHearingRole)
                .WithMany(source => source.InvitationSourceMappings)
                .HasForeignKey(c => c.CompanyHearingRoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ism => ism.UserHearingRole)
                .WithMany(source => source.InvitationSourceMappings)
                .HasForeignKey(c => c.UserHearingRoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}