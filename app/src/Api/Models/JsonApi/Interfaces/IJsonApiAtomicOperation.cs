namespace Agora.Api.Models.JsonApi.Interfaces
{
    /// <summary>
    /// See https://jsonapi.org/ext/atomic/ for reference
    /// </summary>
    public interface IJsonApiAtomicOperation<T>
    {
        // TODO: Create as enum
        // Operation code
        string Op { get; set; }

        // Reference to the type/id the operation operates on
        IJsonApiBaseData Ref { get; set; }

        // The operations primary data
        T Data { get; set; }

        // Non standard meta-information about the operation
        object Meta { get; set; }
    }
}