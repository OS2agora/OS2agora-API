using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BallerupKommune.Api.Models.JsonApi;
using BallerupKommune.Api.Models.JsonApi.Converters;
using BallerupKommune.Api.Models.JsonApi.Interfaces;
using BallerupKommune.Models.Extensions;
using Newtonsoft.Json;

namespace BallerupKommune.Api.Models.Common
{
    public abstract class BaseModifyControlDto<T>
    {
        /// <summary>
        /// JsonApiType is used for validation incoming requests to see if matched type in Json API match DTO name
        /// </summary>
        [JsonIgnore] public string JsonApiType => typeof(T).Name.Replace("AttributeDto", string.Empty).ToLowerCamelCase();

        /// <summary>
        /// Allowed relationships are used to validate an incoming request.
        /// Example: A Comment updating its relation to a User is allowed, but User cannot update its relation to a Comment
        /// </summary>
        [JsonIgnore] public abstract List<DtoRelationship> AllowedRelationships { get; }

        [JsonIgnore] public IJsonApiLinks Links { get; set; } = null;

        [Required(ErrorMessage = "DTO type is required")]
        public string Type { get; set; }

        [JsonConverter(typeof(JsonApiRelationshipConverter))] public Dictionary<string, IJsonApiRelationship> Relationships { get; set; }
    }
}