using Agora.Models.Extensions;

namespace Agora.Api.Models.JsonApi
{
    /// <summary>
    /// Custom DTO Relationship primarily used for 'Allowed Relationships' which
    /// can be used for validation of incoming requests if required
    /// </summary>
    public class DtoRelationship
    {
        public string RelationName { get; set; }
        public string Type { get; set; }

        public DtoRelationship(string relationName, string type = null)
        {
            RelationName = relationName;
            Type = type ?? relationName.ToTitelCase();
        }
    }
}