using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using BallerupKommune.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BallerupKommune.DAOs.Persistence.Configurations
{
    public class NotificationTypeConfiguration : AuditableEntityTypeConfiguration<NotificationTypeEntity>
    {
        public override void Configure(EntityTypeBuilder<NotificationTypeEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.Name).HasMaxLength(100);
        }
    }
}
