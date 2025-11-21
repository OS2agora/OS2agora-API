using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class EventMappingConfiguration : AuditableEntityTypeConfiguration<EventMappingEntity>
    {
        public override void Configure(EntityTypeBuilder<EventMappingEntity> builder)
        {
            base.Configure(builder);
        }
    }
}