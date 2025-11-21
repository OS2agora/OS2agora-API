using Agora.DAOs.Identity;
using Agora.DAOs.Persistence.Configurations.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
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