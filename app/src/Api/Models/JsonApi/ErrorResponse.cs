using BallerupKommune.Api.Models.JsonApi.Interfaces;

namespace BallerupKommune.Api.Models.JsonApi
{
    ///<inheritdoc/>
    public class ErrorResponse : IJsonApiError
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }
}