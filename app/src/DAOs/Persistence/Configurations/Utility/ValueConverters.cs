using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Agora.DAOs.Persistence.Configurations.Utility
{
    internal static class ValueConverters
    {
        internal static ValueConverter<string[], string> StringArrayToString =
            new ValueConverter<string[], string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions)null));

        internal static ValueConverter<string, string> IdentityConverter =
            new ValueConverter<string, string>(value => value, value => value);

        internal static ValueConverter<string, string> LowerCaseConverter =
            new ValueConverter<string, string>(value => value.ToLower(), value => value);

    }
}
