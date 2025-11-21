using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class NotificationTemplateConfiguration : AuditableEntityTypeConfiguration<NotificationTemplateEntity>
    {
        public override void Configure(EntityTypeBuilder<NotificationTemplateEntity> builder)
        {
            base.Configure(builder);

            builder.Property(entity => entity.Name).HasMaxLength(100);
            builder.Property(entity => entity.TextContent).HasMaxLength(4000);
        }
    }
}
