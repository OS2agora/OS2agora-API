using NSwag.Generation.Processors.Contexts;
using NSwag.Generation.Processors;
using NSwag;

namespace Agora.Api.Configuration
{
    public class XApiKeyHeaderParameter : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var parameter = new OpenApiParameter
            {
                Name = "X-Api-Key",
                Kind = OpenApiParameterKind.Header,
                Description = "Api key that identifies which front end is calling.",
                IsRequired = true,
                IsNullableRaw = true,
                Default = "75f987dc-bb20-4062-99b9-756fdf110e0c",
                Schema = new NJsonSchema.JsonSchema()
                {
                    Type = NJsonSchema.JsonObjectType.String,
                    Item = new NJsonSchema.JsonSchema()
                    {
                        Type = NJsonSchema.JsonObjectType.String
                    },
                },
            };
            parameter.Schema.Enumeration.Add("75f987dc-bb20-4062-99b9-756fdf110e0c");
            parameter.Schema.Enumeration.Add("01f81261-91cd-4c8b-8038-346de2c51742");
            context.OperationDescription.Operation.Parameters.Add(parameter);
            return true;
        }
    }
}
