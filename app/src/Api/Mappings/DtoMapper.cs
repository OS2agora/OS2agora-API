using Agora.Api.Models.Common;
using Agora.Api.Models.JsonApi;
using JsonApiSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Agora.Api.Mappings
{
    public static class DtoMapper
    {
        /// <summary>
        /// Converts a JsonApiTopLevelDto to a POCO DTO using the JsonApiSerializer NuGet package
        /// The conversion is done by converting the JsonApiTopLevelDto to a serialized JSON string, and deserializing it back to a POCO DTO
        /// We have opted not to use a simple InputFormatter, so that we can instead achieve strongly types in our API layer
        /// </summary>
        public static TDto MapFromJsonApiDtoToDto<TDto, TJsonApiTopLevelDto, TJsonApiDto, TJsonApiAttributeDto>(
            this TJsonApiTopLevelDto jsonApiTopLevelDto)
            where TDto : DTOs.Common.BaseDto<TDto>
            where TJsonApiTopLevelDto : JsonApiTopLevelDto<TJsonApiDto>
            where TJsonApiDto : BaseDto<TJsonApiAttributeDto>
            where TJsonApiAttributeDto : BaseAttributeDto
        {
            // Serializer which uses camelCasing naming strategy(which matches JsonApi specifications)
            var serializer = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        OverrideSpecifiedNames = false
                    }
                },
                NullValueHandling = NullValueHandling.Ignore
            };

            var dtoAsString = JsonConvert.SerializeObject(jsonApiTopLevelDto, serializer);

            // Convert to DTO
            var dto = JsonConvert.DeserializeObject<TDto>(dtoAsString, new JsonApiSerializerSettings());
            if (dto == null)
            {
                return null;
            }

            dto.PropertiesUpdated = jsonApiTopLevelDto.Data.Attributes.PropertiesUpdated;
            return dto;
        }
    }
}