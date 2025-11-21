using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class KleHierarchyConfiguration : AuditableEntityTypeConfiguration<KleHierarchyEntity>
    {
        public override void Configure(EntityTypeBuilder<KleHierarchyEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.Name).HasMaxLength(500);

            builder.Property(content => content.Number).HasMaxLength(100);
        }
    }
}
