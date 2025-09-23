using System.Collections.Generic;

namespace BallerupKommune.Api.Models.JsonApi.Interfaces
{
    /// <summary>
    /// See http://jsonapi.org/format/#document-resource-objects for reference
    /// </summary>
    public interface IJsonApiResource : IJsonApiBaseData
    {
        Dictionary<string, IJsonApiRelationship> Relationships { get; set; }
        IJsonApiLinks Links { get; set; }
        
        /// <summary>
        /// Allowed relationships are used to validate an incoming request.
        /// Example: A Comment updating its relation to a User is allowed, but User cannot update its relation to a Comment
        /// </summary>
        List<DtoRelationship> AllowedRelationships { get; }
    }
}