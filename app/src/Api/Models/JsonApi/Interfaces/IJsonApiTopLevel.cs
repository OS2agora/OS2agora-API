using System.Collections.Generic;

namespace Agora.Api.Models.JsonApi.Interfaces
{
    /// <summary>
    /// See https://jsonapi.org/format/#document-structure for reference
    /// </summary>
    public interface IJsonApiTopLevel<TDto>
    {
        // Standard Json API specification https://jsonapi.org/
        TDto Data { get; set; }
        List<IJsonApiError> Errors { get; set; }
        List<IJsonApiResource> Included { get; set; }
        object Meta { get; set; }
    }
}