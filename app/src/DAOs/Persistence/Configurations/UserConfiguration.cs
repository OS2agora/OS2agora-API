using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
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

            builder.Property(user => user.Address)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());

            builder.Property(user => user.City)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());

            builder.Property(user => user.Municipality)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());

            builder.Property(user => user.PostalCode)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());

            builder.Property(user => user.Country)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());

            builder.Property(user => user.StreetName)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());

            builder.Property(content => content.Identifier).HasMaxLength(100);
        }
    }
}