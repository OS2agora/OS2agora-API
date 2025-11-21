using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class EventConfiguration : AuditableEntityTypeConfiguration<EventEntity>
    {
        public override void Configure(EntityTypeBuilder<EventEntity> builder)
        {
            base.Configure(builder);

            builder.HasOne(e => e.User)
                .WithMany(user => user.Events)
                .HasForeignKey(e => e.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}