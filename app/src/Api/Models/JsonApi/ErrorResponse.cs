using Agora.Api.Models.JsonApi.Interfaces;

namespace Agora.Api.Models.JsonApi
{
    ///<inheritdoc/>
    public class ErrorResponse : IJsonApiError
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }
}