namespace Agora.Api.Models.JsonApi.Interfaces
{
    /// <summary>
    /// See https://jsonapi.org/format/#document-resource-objects for reference
    /// </summary>
    public interface IJsonApiBaseData
    {
        string Type { get; set; }
        string Id { get; set; }
    }
}
