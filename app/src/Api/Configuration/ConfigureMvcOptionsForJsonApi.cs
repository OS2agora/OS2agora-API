using System.Buffers;
using JsonApiSerializer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Agora.Api.Configuration
{
    // Shamelessly taken from this issue: https://github.com/codecutout/JsonApiSerializer/issues/115
    // AddJsonApi will insert a new output formatter for NewtonsoftJson
    // This have the effect that the Api layer will be able to send JsonApi from a simple POCO DTO
    public class ConfigureMvcOptionsForJsonApi : IConfigureOptions<MvcOptions>
    {
        public void Configure(MvcOptions options)
        {
            var jsonApiSerializerSettings = new JsonApiSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };

            ConfigureOutputFormatter(options, jsonApiSerializerSettings);
        }

        private static void ConfigureOutputFormatter(MvcOptions options,
            JsonApiSerializerSettings jsonApiSerializerSettings)
        {
            var jsonApiOutputFormatter =
                new NewtonsoftJsonOutputFormatter(
                    jsonApiSerializerSettings,
                    ArrayPool<char>.Shared,
                    options);

            options.OutputFormatters.Insert(0, jsonApiOutputFormatter);
        }
    }
}