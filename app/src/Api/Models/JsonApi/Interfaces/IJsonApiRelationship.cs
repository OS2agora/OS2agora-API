namespace Agora.Api.Models.JsonApi.Interfaces
{
    /// <summary>
    /// See https://jsonapi.org/format/#document-resource-object-relationships for reference
    /// </summary>
    public interface IJsonApiRelationship
    {
        IJsonApiRelationshipLinks Links { get; set; }
        object Data { get; set; }
    }
}