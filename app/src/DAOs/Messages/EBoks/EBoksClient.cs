using BallerupKommune.DAOs.Messages.EBoks.DTOs;
using BallerupKommune.Operations.ApplicationOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Messages.EBoks
{
    public class EBoksClient
    {
        private const string Sender = "ItSystem";
        private const string SystemIdentifier = "agora";
        private const string SendMethod = "bedst og billigst";
        private const string PdfFormat = "PDF";
        private const string HtmlFormat = "HTML";
        private const string MessageContentKey = "digitalpost:meddelelseIndholdData";
        private const string MessageFileFormatKey = "digitalpost:filformatNavn";
        private const string MessageResponseTypeKey = "digitalpost:meddelelseSvarTypeNavn";
        private const string MessageResponseTypeValue = "ikkeMuligt";
        private const string MessageContentIdentifierKey = "digitalpost:meddelelseIndholdstypeIdentifikator";
        private const string MessageContentIdentifierValue = "167783";

        protected readonly HttpClient HttpClient;
        private readonly ILogger<EBoksClient> _logger;
        private readonly IOptions<EBoksOptions> _options;

        public EBoksClient(IOptions<EBoksOptions> options, HttpClient httpClient, ILogger<EBoksClient> logger)
        {
            httpClient.BaseAddress = options.Value.Disabled ? null : new Uri(options.Value.BaseAddress);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

            var basicAuthValueAsByteArray =
                new UTF8Encoding().GetBytes($"{options.Value.ClientId}:{options.Value.ClientSecret}");
            var basicAuthValueAsBase64String = Convert.ToBase64String(basicAuthValueAsByteArray);

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", basicAuthValueAsBase64String);

            HttpClient = httpClient;
            _logger = logger;
            _options = options;
        }

        public async Task<bool> Send(string documentTitle, string personalIdentifier, string fileName, string fileContent)
        {
            if (_options.Value.Disabled)
            {
                return false;
            }

            var isCprNumber = personalIdentifier.Length == 10;

            var createDto = new OpretEboksDokumentDto
            {
                Overskrift = documentTitle,
                Afsender = new AfsenderDto
                {
                    AktoerTypeKode = Sender,
                    UrnIdentifikator = SystemIdentifier
                },
                Fil = new List<FileDto>
                {
                    // We can only send PDF since we are using "bedst og billigst"
                    new FileDto
                    {
                        Format = PdfFormat,
                        Navn = fileName,
                        Base64 = fileContent
                    }
                },
                LeveranceMetode = new List<LeveranceMetodeDto>
                {
                    new LeveranceMetodeDto
                    {
                        Metode = SendMethod,
                        Sort = 1
                    }
                },
                Leverandoer = new LeverandoerDto
                {
                    UrnIdentifikator = SystemIdentifier
                },
                Modtager = new List<ModtagerDto>
                {
                    new ModtagerDto
                    {
                        SlutbrugerIdentitet = new SlutbrugerIdentitetDto
                        {
                            CprNummerIdentifikator = isCprNumber ? personalIdentifier : null,
                            CvrNummerIdentifikator = !isCprNumber ? personalIdentifier : null
                        }
                    }
                },
                Udvidelse = new List<UdvidelseDto>
                {
                    // Content of "display message"
                    new UdvidelseDto
                    {
                        Key = MessageContentKey,
                        Value =
                            "<!DOCTYPE HTML\nPublic \"-//W3C//DTD HTML 4.01 Transitional//EN\"><html><head>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\">\n</head>\n<body>I vedlagte fil finder du informationen</body></html>"
                    },
                    // Format of "display message"
                    new UdvidelseDto
                    {
                        Key = MessageFileFormatKey,
                        Value = HtmlFormat
                    },
                    // Cannot write back to us
                    new UdvidelseDto
                    {
                        Key = MessageResponseTypeKey,
                        Value = MessageResponseTypeValue
                    },
                    // Given to us by the administrators of the system References an internal type
                    new UdvidelseDto
                    {
                        Key = MessageContentIdentifierKey,
                        Value = MessageContentIdentifierValue
                    }
                }
            };


            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(createDto, settings), Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.PostAsync(string.Empty, postContent);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"EBoksClient: Exception caught: {e.Message}");
                if (response?.Content != null)
                {
                    _logger.LogError($"request_body: {await response.Content.ReadAsStringAsync()}");
                }

                throw;
            }
        }
    }
}