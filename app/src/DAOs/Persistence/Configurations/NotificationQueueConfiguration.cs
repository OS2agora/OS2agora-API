using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using BallerupKommune.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BallerupKommune.DAOs.Persistence.Configurations
{
    public class NotificationQueueConfiguration : AuditableEntityTypeConfiguration<NotificationQueueEntity>
    {
        private readonly IEncryptionValueConverterFactory _encryptionValueConverterFactory;

        public NotificationQueueConfiguration(IEncryptionValueConverterFactory encryptionValueConverterFactory)
        {
            _encryptionValueConverterFactory = encryptionValueConverterFactory;
        }

        public override void Configure(EntityTypeBuilder<NotificationQueueEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.Content)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());
            
            builder.Property(content => content.ErrorTexts)
                .HasMaxLength(1000)
                .HasConversion(ValueConverters.StringArrayToString)
                .Metadata
                .SetValueComparer(ValueComparetors.StringArrayComparetor);

            builder.Property(content => content.Subject).HasMaxLength(100);
            
            builder.Property(content => content.RecipientAddress)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());
        }
    }
}
