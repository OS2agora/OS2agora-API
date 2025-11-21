using Agora.Api.Models.JsonApi.Interfaces;

namespace Agora.Api.Models.JsonApi
{
    ///<inheritdoc/>
    public class AtomicResponseObject<T> : IJsonApiAtomicResult<T>
    {
        // The primary data resulting from the operation
        public T Data { get; set; }

        // Non standard meta-information about the result
        public object Meta { get; set; }
    }
}