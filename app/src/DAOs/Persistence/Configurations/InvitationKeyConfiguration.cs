using Agora.DAOs.Persistence.Configurations.Utility;
using Agora.Entities.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agora.DAOs.Persistence.Configurations
{
    public class InvitationKeyConfiguration : AuditableEntityTypeConfiguration<InvitationKeyEntity>
    {
        private readonly IEncryptionValueConverterFactory _encryptionValueConverterFactory;

        public InvitationKeyConfiguration(IEncryptionValueConverterFactory encryptionValueConverterFactory)
        {
            _encryptionValueConverterFactory = encryptionValueConverterFactory;
        }

        public override void Configure(EntityTypeBuilder<InvitationKeyEntity> builder)
        {
            base.Configure(builder);

            builder.Property(invitationKey => invitationKey.Cvr)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionStrippingConverter());

            builder.Property(invitationKey => invitationKey.Cpr)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetStringEncryptionStrippingConverter());

            builder.Property(invitationKey => invitationKey.Email)
                .HasMaxLength(500)
                .HasConversion(_encryptionValueConverterFactory.GetLowerCaseStringEncryptionConverter());
        }
    }
}