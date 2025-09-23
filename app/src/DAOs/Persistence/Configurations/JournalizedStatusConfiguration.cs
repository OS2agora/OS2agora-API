using System;
using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Entities.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BallerupKommune.DAOs.Persistence.Configurations
{
    public class JournalizedStatusConfiguration : AuditableEntityTypeConfiguration<JournalizedStatusEntity>
    {
        public override void Configure(EntityTypeBuilder<JournalizedStatusEntity> builder)
        {
            base.Configure(builder);

            // We need to seed it now as the data is needed to migrate from hearings isJournalized flag
            builder.HasData(
                new { Id = 1, Created = new DateTime(2024, 3, 20, 15, 48, 56, 396, DateTimeKind.Local), Status = JournalizedStatus.NOT_JOURNALIZED },
                new { Id = 2, Created = new DateTime(2024, 3, 20, 15, 48, 56, 396, DateTimeKind.Local), Status = JournalizedStatus.JOURNALIZED },
                new { Id = 3, Created = new DateTime(2024, 3, 20, 15, 48, 56, 396, DateTimeKind.Local), Status = JournalizedStatus.JOURNALIZED_WITH_ERROR });
        }
    }
}
