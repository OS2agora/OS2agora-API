using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class NotificationTypeConfiguration : AuditableEntityTypeConfiguration<NotificationTypeEntity>
    {
        public override void Configure(EntityTypeBuilder<NotificationTypeEntity> builder)
        {
            base.Configure(builder);

            builder.HasOne(nt => nt.HeaderTemplate)
                .WithMany(ntemplate => ntemplate.HeaderTemplateSpecifications)
                .HasForeignKey(nt => nt.HeaderTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(nt => nt.BodyTemplate)
                .WithMany(ntemplate => ntemplate.BodyTemplateSpecifications)
                .HasForeignKey(nt => nt.BodyTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(nt => nt.FooterTemplate)
                .WithMany(ntemplate => ntemplate.FooterTemplateSpecifications)
                .HasForeignKey(nt => nt.FooterTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(nt => nt.SubjectTemplate)
                .WithMany(ntemplate => ntemplate.SubjectTemplateSpecifications)
                .HasForeignKey(nt => nt.SubjectTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(content => content.Name).HasMaxLength(100);
        }
    }
}
