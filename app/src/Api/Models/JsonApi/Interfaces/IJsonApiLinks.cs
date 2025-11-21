namespace Agora.Api.Models.JsonApi.Interfaces
{
    /// <summary>
    /// See https://jsonapi.org/format/#document-links for reference
    /// </summary>
    public interface IJsonApiLinks
    {
        string Self { get; set; }
    }
}