using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using BallerupKommune.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BallerupKommune.DAOs.Persistence.Configurations
{
    public class CommentConfiguration : AuditableEntityTypeConfiguration<CommentEntity>
    {
        public override void Configure(EntityTypeBuilder<CommentEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.OnBehalfOf).HasMaxLength(500);

            builder.HasOne(c => c.CommentParrent)
                .WithMany(commentParent => commentParent.CommentChildren)
                .HasForeignKey(c => c.CommentParrentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.CommentDeclineInfo)
                .WithMany()
                .HasForeignKey(c => c.CommentDeclineInfoId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
