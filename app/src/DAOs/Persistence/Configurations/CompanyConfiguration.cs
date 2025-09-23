using BallerupKommune.DAOs.Persistence.Configurations.Utility;
using BallerupKommune.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BallerupKommune.DAOs.Persistence.Configurations
{
    public class CompanyConfiguration : AuditableEntityTypeConfiguration<CompanyEntity>
    {
        private readonly IEncryptionValueConverterFactory _encryptionValueConverterFactory;

        public CompanyConfiguration(IEncryptionValueConverterFactory encryptionValueConverterFactory)
        {
            _encryptionValueConverterFactory = encryptionValueConverterFactory;
        }

        public override void Configure(EntityTypeBuilder<CompanyEntity> builder)
        {
            base.Configure(builder);

            builder.Property(company => company.Cvr)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionStrippingConverter());

            builder.Property(company => company.Name)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionConverter());
        }
    }
}
