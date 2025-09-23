using BallerupKommune.Api.Models.JsonApi.Interfaces;

namespace BallerupKommune.Api.Models.JsonApi
{
    ///<inheritdoc/>
    public class SelfLinkDto : IJsonApiLinks
    {
        public string Self { get; set; }
    }
}