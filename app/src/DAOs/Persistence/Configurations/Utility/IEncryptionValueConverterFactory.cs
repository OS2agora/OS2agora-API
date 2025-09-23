using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BallerupKommune.DAOs.Persistence.Configurations.Utility
{
    /// <summary>
    /// Factory service for generating <see cref="ValueConverter"/>s used for encrypting/decrypting database fields.
    /// </summary>
    public interface IEncryptionValueConverterFactory
    {
        /// <summary>
        /// Generates <see cref="ValueConverter"/> for encrypting/decrypting.
        /// </summary>
        /// <returns>The generated <see cref="ValueConverter"/>.</returns>
        ValueConverter<string, string> GetStringEncryptionConverter();

        /// <summary>
        /// Generates <see cref="ValueConverter"/> for encrypting/decrypting as well as converting to lower case
        /// </summary>
        /// <returns>The generated <see cref="ValueConverter"/>.</returns>
        ValueConverter<string, string> GetLowerCaseStringEncryptionConverter();

        /// <summary>
        /// Generates <see cref="ValueConverter"/> for encrypting/decrypting as well as removing all non-number
        /// characters.
        /// </summary>
        /// <returns>The generated <see cref="ValueConverter"/>.</returns>
        ValueConverter<string, string> GetStringEncryptionStrippingConverter();
    }
}