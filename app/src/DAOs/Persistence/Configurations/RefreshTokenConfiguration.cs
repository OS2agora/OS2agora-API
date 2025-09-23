using BallerupKommune.DAOs.Identity;
using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BallerupKommune.DAOs.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        private readonly IEncryptionValueConverterFactory _encryptionValueConverterFactory;

        public RefreshTokenConfiguration(IEncryptionValueConverterFactory encryptionValueConverterFactory)
        {
            _encryptionValueConverterFactory = encryptionValueConverterFactory;
        }

        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.Property(refreshToken => refreshToken.Token)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());
        }
    }
}