using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class SubjectAreaConfiguration : AuditableEntityTypeConfiguration<SubjectAreaEntity>
    {
        public override void Configure(EntityTypeBuilder<SubjectAreaEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.Name).HasMaxLength(100);
        }
    }
}
