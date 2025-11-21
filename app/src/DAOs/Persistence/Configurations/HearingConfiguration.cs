using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class HearingConfiguration : AuditableEntityTypeConfiguration<HearingEntity>
    {
        public override void Configure(EntityTypeBuilder<HearingEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.ContactPersonDepartmentName).HasMaxLength(100);

            builder.Property(content => content.ContactPersonEmail).HasMaxLength(500);

            builder.Property(content => content.ContactPersonName).HasMaxLength(100);

            builder.Property(content => content.ContactPersonPhoneNumber).HasMaxLength(100);

            builder.Property(content => content.EsdhTitle).HasMaxLength(110);

            builder.Property(content => content.EsdhNumber).HasMaxLength(100);

            builder.Property(content => content.EsdhMetaData).HasMaxLength(500);
        }
    }
}
