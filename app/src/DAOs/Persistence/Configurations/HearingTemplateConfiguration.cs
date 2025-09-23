using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using BallerupKommune.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BallerupKommune.DAOs.Persistence.Configurations
{
    public class HearingTemplateConfiguration : AuditableEntityTypeConfiguration<HearingTemplateEntity>
    {
        public override void Configure(EntityTypeBuilder<HearingTemplateEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.Name).HasMaxLength(100);
        }
    }
}
