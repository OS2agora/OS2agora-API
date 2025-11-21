using Agora.Models.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Agora.DAOs.Persistence.Configurations.Utility
{
    /// <inheritdoc />
    public class EncryptionValueConverterFactory : IEncryptionValueConverterFactory
    {
        private const string EncryptionPurpose = "encryptionPurpose";
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public EncryptionValueConverterFactory(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        /// <inheritdoc />
        public ValueConverter<string, string> GetStringEncryptionConverter()
        {
            IDataProtector protector = _dataProtectionProvider.CreateProtector(EncryptionPurpose);
            return new ValueConverter<string, string>(
                value => protector.Protect(value),
                value => protector.Unprotect(value));
        }

        /// <inheritdoc />
        public ValueConverter<string, string> GetLowerCaseStringEncryptionConverter()
        {
            IDataProtector protector = _dataProtectionProvider.CreateProtector(EncryptionPurpose);
            return new ValueConverter<string, string>(
                value => protector.Protect(value.ToLower()),
                value => protector.Unprotect(value));
        }

        /// <inheritdoc />
        public ValueConverter<string, string> GetStringEncryptionStrippingConverter()
        {
            IDataProtector protector = _dataProtectionProvider.CreateProtector(EncryptionPurpose);
            return new ValueConverter<string, string>(
                value => protector.Protect(value.RemoveAllNonNumbers()),
                value => protector.Unprotect(value).RemoveAllNonNumbers());
        }
    }
}