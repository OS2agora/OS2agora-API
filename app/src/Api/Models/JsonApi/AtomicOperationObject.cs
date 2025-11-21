using Agora.Api.Models.JsonApi.Interfaces;

namespace Agora.Api.Models.JsonApi
{
    ///<inheritdoc/>
    public class AtomicOperationObject<T> : IJsonApiAtomicOperation<T>
    {
        // TODO: Create as enum
        // Operation code
        public string Op { get; set; }

        // Reference to the type/id the operation operates on
        public IJsonApiBaseData Ref { get; set; }

        // The operations primary data
        public T Data { get; set; }

        // Non standard meta-information about the operation
        public object Meta { get; set; }
    }
}