using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Reflection;

namespace Agora.DAOs.Files.Csv.Mapping
{
    public class DynamicClassMap<T> : ClassMap<T> where T : class
    {
        public DynamicClassMap(Dictionary<string, string> columnMappings)
        {
            foreach (var mapping in columnMappings)
            {
                var propertyName = mapping.Key;
                var columnName = mapping.Value;

                var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    Map(typeof(T), property).Name(columnName);
                }
            }
        }
    }
}
