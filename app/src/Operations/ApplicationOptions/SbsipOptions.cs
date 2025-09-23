using System.Collections.Generic;

namespace BallerupKommune.Operations.ApplicationOptions
{
    public class SbsipOptions
    {
        public const string Sbsip = "Sbsip";

        public string ClientId { get; set; }
        public string GrantType { get; set; }

        public string ClientSecret { get; set; }

        public string BaseAddress { get; set; }

        public string TokenServiceBaseAddress { get; set; }

        public string TokenRequestUri { get; set; }

        public Dictionary<string,int> SbsysTemplateIds { get; set; }
    }
}