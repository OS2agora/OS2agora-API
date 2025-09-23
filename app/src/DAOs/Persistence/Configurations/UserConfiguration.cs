using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using BallerupKommune.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BallerupKommune.DAOs.Persistence.Configurations
{
    public class UserConfiguration : AuditableEntityTypeConfiguration<UserEntity>
    {
        private readonly IEncryptionValueConverterFactory _encryptionValueConverterFactory;

        public UserConfiguration(IEncryptionValueConverterFactory encryptionValueConverterFactory)
        {
            _encryptionValueConverterFactory = encryptionValueConverterFactory;
        }

        public override void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            base.Configure(builder);
            
            builder.Property(user => user.PersonalIdentifier)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetLowerCaseStringEncryptionConverter());

            builder.Property(user => user.Name)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());

            builder.Property(user => user.EmployeeDisplayName)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());

            builder.Property(user => user.Email)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetLowerCaseStringEncryptionConverter());

            builder.Property(user => user.Cpr)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionStrippingConverter());

            builder.Property(user => user.Cvr)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionStrippingConverter());

            builder.Property(content => content.Identifier).HasMaxLength(100);
        }
    }
}