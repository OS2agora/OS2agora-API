using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class ContentConfiguration : AuditableEntityTypeConfiguration<ContentEntity>
    {
        private readonly IEncryptionValueConverterFactory _encryptionValueConverterFactory;

        public ContentConfiguration(IEncryptionValueConverterFactory encryptionValueConverterFactory)
        {
            _encryptionValueConverterFactory = encryptionValueConverterFactory;
        }

        public override void Configure(EntityTypeBuilder<ContentEntity> builder)
        {
            base.Configure(builder);

            builder.Property(content => content.FileName).HasMaxLength(600);

            builder.Property(content => content.FilePath).HasMaxLength(100);

            builder.Property(content => content.FileContentType).HasMaxLength(100);

            builder.Property(content => content.TextContent)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());

            builder.Property(content => content.FileName)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());

            builder.HasOne(c => c.Comment)
                .WithMany(comment => comment.Contents)
                .HasForeignKey(c => c.CommentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
