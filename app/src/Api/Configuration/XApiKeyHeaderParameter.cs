using NSwag.Generation.Processors.Contexts;
using NSwag.Generation.Processors;
using NSwag;

namespace BallerupKommune.Api.Configuration
{
    public class XApiKeyHeaderParameter : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var parameter = new OpenApiParameter
            {
                Name = "X-Api-Key",
                Kind = OpenApiParameterKind.Header,
                Description = "Api key that identifies whcih front end is calling.",
                IsRequired = true,
                IsNullableRaw = true,
                Default = "Api key",
                Schema = new NJsonSchema.JsonSchema()
                {
                    Type = NJsonSchema.JsonObjectType.String,
                    Item = new NJsonSchema.JsonSchema()
                    {
                        Type = NJsonSchema.JsonObjectType.String
                    },
                },
            };
            parameter.Schema.Enumeration.Add("replacewiththekeyfrominternalfrontend");
            parameter.Schema.Enumeration.Add("replacewiththekeyfrompublicfrontend");
            context.OperationDescription.Operation.Parameters.Add(parameter);
            return true;
        }
    }
}
