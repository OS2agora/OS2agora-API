namespace BallerupKommune.Api.Models.JsonApi.Interfaces
{
    /// <summary>
    /// See https://jsonapi.org/format/#document-resource-object-related-resource-links for reference
    /// </summary>
    public interface IJsonApiRelationshipLinks : IJsonApiLinks
    {
        string Related { get; set; }
    }
}
