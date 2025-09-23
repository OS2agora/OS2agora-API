using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using BallerupKommune.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BallerupKommune.DAOs.Persistence.Configurations
{
    public class NotificationConfiguration : AuditableEntityTypeConfiguration<NotificationEntity>
    {
        public override void Configure(EntityTypeBuilder<NotificationEntity> builder)
        {
            base.Configure(builder);

            builder.HasOne(n => n.NotificationQueue)
                .WithMany(nq => nq.Notifications)
                .HasForeignKey(c => c.NotificationQueueId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
