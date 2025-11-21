using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class NotificationConfiguration : AuditableEntityTypeConfiguration<NotificationEntity>
    {
        public override void Configure(EntityTypeBuilder<NotificationEntity> builder)
        {
            base.Configure(builder);

            builder.HasOne(n => n.Comment)
                .WithMany(comment => comment.Notifications)
                .HasForeignKey(n => n.CommentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.Company)
                .WithMany(company => company.Notifications)
                .HasForeignKey(n => n.CompanyId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.User)
                .WithMany(user => user.Notifications)
                .HasForeignKey(n => n.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
