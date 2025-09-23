using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using BallerupKommune.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BallerupKommune.DAOs.Persistence.Configurations
{
    public class CommentDeclineInfoConfiguration : AuditableEntityTypeConfiguration<CommentDeclineInfoEntity>
    {
        private readonly IEncryptionValueConverterFactory _encryptionValueConverterFactory;

        public CommentDeclineInfoConfiguration(IEncryptionValueConverterFactory encryptionValueConverterFactory)
        {
            _encryptionValueConverterFactory = encryptionValueConverterFactory;
        }

        public override void Configure(EntityTypeBuilder<CommentDeclineInfoEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.DeclineReason).HasMaxLength(500).HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());
            builder.Property(content => content.DeclinerInitials).HasMaxLength(500).HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());
                
        }
    }
}
