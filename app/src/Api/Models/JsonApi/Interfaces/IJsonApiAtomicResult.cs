namespace BallerupKommune.Api.Models.JsonApi.Interfaces
{
    /// <summary>
    /// See https://jsonapi.org/ext/atomic/ for reference
    /// </summary>
    public interface IJsonApiAtomicResult<T>
    {
        // The primary data resulting from the operation
        T Data { get; set; }

        // Non standard meta-information about the result
        object Meta { get; set; }
    }
}