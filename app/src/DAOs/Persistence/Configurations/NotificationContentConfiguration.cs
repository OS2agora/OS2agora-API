using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class NotificationContentConfiguration : AuditableEntityTypeConfiguration<NotificationContentEntity>
    {
        private readonly IEncryptionValueConverterFactory _encryptionValueConverterFactory;

        public NotificationContentConfiguration(IEncryptionValueConverterFactory encryptionValueConverterFactory)
        {
            _encryptionValueConverterFactory = encryptionValueConverterFactory;
        }

        public override void Configure(EntityTypeBuilder<NotificationContentEntity> builder)
        {
            base.Configure(builder);

            builder.Property(entity => entity.TextContent)
                .HasMaxLength(4000)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());
        }
    }
}