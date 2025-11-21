using System;
using MessagePack;
using MessagePack.Formatters;

namespace Agora.DAOs.Utility.CustomFormatters
{
    /// <summary>
    /// Custom MessagePackFormatter used with Redis for DB caching
    /// https://github.com/VahidN/EFCoreSecondLevelCacheInterceptor/blob/master/src/Tests/Issues/Issue123WithMessagePack/EFServiceProvider.cs
    /// </summary>
    public class DbNullFormatter : IMessagePackFormatter<DBNull>
    {
        public static DbNullFormatter Instance = new DbNullFormatter();

        private DbNullFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, DBNull value, MessagePackSerializerOptions options)
            => writer.WriteNil();

        public DBNull Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
            => DBNull.Value;
    }
}