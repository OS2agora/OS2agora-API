using System.Collections.Generic;
using Agora.Api.Models.JsonApi.Interfaces;

namespace Agora.Api.Models.JsonApi
{
    ///<inheritdoc/>
    public class JsonApiTopLevelDto<TDto> : IJsonApiTopLevel<TDto>
    {
        // Standard Json API specification https://jsonapi.org/
        public TDto Data { get; set; }
        public List<IJsonApiError> Errors { get; set; }
        public List<IJsonApiResource> Included { get; set; }
        public object Meta { get; set; }
    }
}
