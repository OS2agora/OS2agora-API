using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
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
