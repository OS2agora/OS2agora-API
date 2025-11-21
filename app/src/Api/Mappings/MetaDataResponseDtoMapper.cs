using Agora.DTOs.Common.CustomResponseDto;
using JsonApiSerializer.JsonApi;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;
using Newtonsoft.Json;

namespace Agora.Api.Mappings;

public static class MetaDataResponseDtoMapper<TDto, TMetaDto>
{
    /// <summary>
    /// Create DocumentRoot object and serialize metadata manually. JsonApiSerializer will still handle extraction of includes from the entity in the data property
    /// </summary>
    /// <param name="dto">MetaDataResponse dto containing data and meta data</param>
    /// <returns>DocumentRoot object ready to be serialized via JsonApiSerializer</returns>
    public static DocumentRoot<TDto> MapToDocumentRoot(
        MetaDataResponseDto<TDto, TMetaDto> dto)
    {
        if (dto == null)
        {
            return null;
        }
        Meta meta = null;
        if (dto.Meta != null)
        {
            meta = new Meta();
            var metaProps = dto.Meta.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in metaProps)
            {
                var value = property.GetValue(dto.Meta);

                // Only include primitives, strings, decimals, DateTime, etc.
                if (value != null && (
                        property.PropertyType.IsPrimitive ||
                        property.PropertyType.IsEnum ||
                        property.PropertyType == typeof(string) ||
                        property.PropertyType == typeof(decimal) ||
                        property.PropertyType == typeof(DateTime) ||
                        property.PropertyType == typeof(Guid)))

                    meta.Add(property.Name, JToken.FromObject(value, new JsonSerializer
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
            }
        }

        return new DocumentRoot<TDto>
        {
            Data = dto.ResponseData,
            Meta = meta,
        };
    }
}