using System.Collections.Generic;
using Agora.Models.Extensions;
using Newtonsoft.Json;

namespace Agora.DTOs.Common
{
    public class BaseDto<T>
    {
        public int Id { get; set; }

        /// <summary>
        /// This is the type the JsonApiSerialization NuGet package will use when sending a JsonApi response from the Api layer
        /// </summary>
        public string Type => typeof(T).Name.Replace("Dto", string.Empty).ToLowerCamelCase();

        [JsonIgnore] public List<string> PropertiesUpdated = new List<string>();
    }
}