using Agora.DAOs.Identity;
using Agora.DAOs.Persistence.Configurations.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    private readonly IEncryptionValueConverterFactory _encryptionValueConverterFactory;

    public ApplicationUserConfiguration(IEncryptionValueConverterFactory encryptionValueConverterFactory)
    {
        _encryptionValueConverterFactory = encryptionValueConverterFactory;
    }

    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(appUser => appUser.UserName)
            .HasConversion(_encryptionValueConverterFactory.GetLowerCaseStringEncryptionConverter());

        builder.Property(appUser => appUser.NormalizedUserName)
            .HasConversion(_encryptionValueConverterFactory.GetLowerCaseStringEncryptionConverter());
    }
}