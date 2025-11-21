using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class FieldTypeSpecificationConfiguration : AuditableEntityTypeConfiguration<FieldTypeSpecificationEntity>
    {
        public override void Configure(EntityTypeBuilder<FieldTypeSpecificationEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.Name).HasMaxLength(100);
        }
    }
}
