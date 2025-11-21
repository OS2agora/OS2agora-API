namespace Agora.Api.Models.JsonApi.Interfaces
{
    /// <summary>
    /// See https://jsonapi.org/format/#errors for reference
    /// </summary>
    public interface IJsonApiError
    {
        int ErrorCode { get; set; }
        string Message { get; set; }
    }
}