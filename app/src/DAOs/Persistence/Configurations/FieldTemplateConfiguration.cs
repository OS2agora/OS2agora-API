using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using BallerupKommune.Entities.Entities;
using BallerupKommune.Operations.Models.FieldTemplates.Commands.Rules;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BallerupKommune.DAOs.Persistence.Configurations
{
    public class FieldTemplateConfiguration : AuditableEntityTypeConfiguration<FieldTemplateEntity>
    {
        public override void Configure(EntityTypeBuilder<FieldTemplateEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.Name).HasMaxLength(100);

            builder.Property(content => content.Text).HasMaxLength(FieldTemplateConstants.TextFieldMaximumLength);
        }
    }
}
