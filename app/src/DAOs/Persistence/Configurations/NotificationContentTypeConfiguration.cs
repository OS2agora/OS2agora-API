using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class NotificationContentTypeConfiguration : AuditableEntityTypeConfiguration<NotificationContentTypeEntity>
    {
        public override void Configure(EntityTypeBuilder<NotificationContentTypeEntity> builder)
        {
            base.Configure(builder);
        }
    }
}