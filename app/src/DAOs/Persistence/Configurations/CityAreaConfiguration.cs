using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class CityAreaConfiguration : AuditableEntityTypeConfiguration<CityAreaEntity>
    {
        public override void Configure(EntityTypeBuilder<CityAreaEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.Name).HasMaxLength(200);
        }
    }
}