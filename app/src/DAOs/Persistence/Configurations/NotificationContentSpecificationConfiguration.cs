using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class NotificationContentSpecificationConfiguration : AuditableEntityTypeConfiguration<NotificationContentSpecificationEntity>
    {
        public override void Configure(EntityTypeBuilder<NotificationContentSpecificationEntity> builder)
        {
            base.Configure(builder);

            builder.HasOne(ncs => ncs.HeaderContent)
                .WithOne(nc => nc.HeaderForSpecification)
                .HasForeignKey<NotificationContentSpecificationEntity>(ncs => ncs.HeaderContentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ncs => ncs.BodyContent)
                .WithOne(nc => nc.BodyForSpecification)
                .HasForeignKey<NotificationContentSpecificationEntity>(ncs => ncs.BodyContentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ncs => ncs.FooterContent)
                .WithOne(nc => nc.FooterForSpecification)
                .HasForeignKey<NotificationContentSpecificationEntity>(ncs => ncs.FooterContentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ncs => ncs.SubjectContent)
                .WithOne(nc => nc.SubjectForSpecification)
                .HasForeignKey<NotificationContentSpecificationEntity>(ncs => ncs.SubjectContentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}