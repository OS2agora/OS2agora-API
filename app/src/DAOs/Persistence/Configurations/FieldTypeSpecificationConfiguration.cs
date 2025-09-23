using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using BallerupKommune.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BallerupKommune.DAOs.Persistence.Configurations
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
