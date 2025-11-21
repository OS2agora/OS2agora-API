using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Agora.DAOs.Persistence.Configurations.Utility
{
    /// <summary>
    /// Implementation of <see cref="IEncryptionValueConverterFactory"/> with no encryption/decryption.
    /// </summary>
    public class DisabledEncryptionValueConverterFactory : IEncryptionValueConverterFactory
    {
        /// <summary>
        /// Generates trivial <see cref="ValueConverter"/>.
        /// </summary>
        /// <returns>Trivial <see cref="ValueConverter"/>.</returns>
        public ValueConverter<string, string> GetStringEncryptionConverter()
        {
            return ValueConverters.IdentityConverter;
        }

        /// <summary>
        /// Generates trivial <see cref="ValueConverter"/> converting to lower case.
        /// </summary>
        /// <returns>Trivial <see cref="ValueConverter"/>.</returns>
        public ValueConverter<string, string> GetLowerCaseStringEncryptionConverter()
        {
            return ValueConverters.LowerCaseConverter;
        }

        /// <summary>
        /// Generates trivial <see cref="ValueConverter"/>.
        /// </summary>
        /// <returns>Trivial <see cref="ValueConverter"/>.</returns>
        public ValueConverter<string, string> GetStringEncryptionStrippingConverter()
        {
            return ValueConverters.IdentityConverter;
        }
    }
}