using System.Collections.Generic;
using System.Linq;
using BallerupKommune.Api.Models.JsonApi.Interfaces;
using Newtonsoft.Json;

namespace BallerupKommune.Api.Models.JsonApi
{
    ///<inheritdoc/>
    public class RelationshipDto : IJsonApiRelationship
    {
        public IJsonApiRelationshipLinks Links { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public object Data { get; set; }

        public RelationshipDto()
        {
        }

        public RelationshipDto(IEnumerable<int> ids, string type)
        {
            Data = ids.Select(id => new RelationshipResourceIdentifierDto(id.ToString(), type)).ToList();
        }

        public RelationshipDto(int id, string type)
        {
            Data = new RelationshipResourceIdentifierDto(id.ToString(), type);
        }
    }
}