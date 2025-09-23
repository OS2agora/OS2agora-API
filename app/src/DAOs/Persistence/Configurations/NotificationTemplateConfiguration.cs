using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using BallerupKommune.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BallerupKommune.DAOs.Persistence.Configurations
{
    public class NotificationTemplateConfiguration : AuditableEntityTypeConfiguration<NotificationTemplateEntity>
    {
        public override void Configure(EntityTypeBuilder<NotificationTemplateEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.NotificationTemplateText).HasMaxLength(1000);

            builder.Property(content => content.SubjectTemplate).HasMaxLength(100);
        }
    }
}
