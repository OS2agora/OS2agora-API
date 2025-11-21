using Agora.DTOs.Common.CustomResponseDto;
using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Agora.Api.Mappings
{
    public static class ResponseListDtoMapper<T>
    {
        /// <summary>
        /// Create a DocumentRoot object and manually add Meta-data from ResponseListDto to it. When serializing the DocumentRoot, JsonApiSerializer is
        /// forced to recognize and include the meta-data. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="responseListDto"></param>
        /// <returns>A DocumentRoot object to serialize via JsonApiSerializer</returns>
        public static DocumentRoot<ResponseListDto<T>> MapToDocumentRoot<T>(ResponseListDto<T> responseListDto)
        {
            if (responseListDto == null)
            {
                return null;
            }

            Meta meta = null;

            if (responseListDto.Meta != null)
            {
                meta = new Meta();
                foreach (var entry in responseListDto.Meta)
                {
                    meta.Add(entry.Key, JObject.FromObject(entry.Value, new JsonSerializer
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
                }
            }

            return new DocumentRoot<ResponseListDto<T>>
            {
                Data = responseListDto,
                Meta = meta
            };
        }

    }
}