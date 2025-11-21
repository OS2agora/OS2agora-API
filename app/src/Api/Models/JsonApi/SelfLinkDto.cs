using Agora.Api.Models.JsonApi.Interfaces;

namespace Agora.Api.Models.JsonApi
{
    ///<inheritdoc/>
    public class SelfLinkDto : IJsonApiLinks
    {
        public string Self { get; set; }
    }
}