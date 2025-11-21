using Agora.Api.Models.JsonApi.Interfaces;

namespace Agora.Api.Models.JsonApi
{
    ///<inheritdoc/>
    public class RelationshipResourceIdentifierDto : IJsonApiBaseData
    {
        public string Id { get; set; }
        public string Type { get; set; }

        public RelationshipResourceIdentifierDto(string id, string type)
        {
            Id = id;
            Type = type;
        }
    }
}