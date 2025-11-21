using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class ValidationRuleConfiguration : AuditableEntityTypeConfiguration<ValidationRuleEntity>
    {
        public override void Configure(EntityTypeBuilder<ValidationRuleEntity> builder)
        {
            base.Configure(builder);

            builder.Property(validationRule => validationRule.AllowedFileTypes)
                .HasMaxLength(500)
                .HasConversion(ValueConverters.StringArrayToString)
                .Metadata
                .SetValueComparer(ValueComparetors.StringArrayComparetor);
        }
    }
}
