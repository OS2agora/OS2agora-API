using System;
using System.Collections.Generic;
using System.IO;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.TextResolverKeys;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Agora.Operations.Resolvers
{
    public interface ITextResolver
    {
        public string GetText(GroupKey groupKey, TextKey textKey);
    }

    /// <summary>
    /// This resolver is responsible for providing municipality specific static text.
    /// </summary>
    public class TextResolver : ITextResolver
    {
        public MunicipalityKey Municipality { get; }
        private IDictionary<string, object> _textObjects;
        private readonly ILogger<TextResolver> _logger;
        private readonly IOptions<AppOptions> _options;

        public TextResolver(ILogger<TextResolver> logger, IOptions<AppOptions> options)
        {
            _logger = logger;
            _options = options;
            Municipality = GetMunicipalityKey();
            InitializeTexts();

            logger.LogDebug("Successfully initialized TextResolver for municipality {MunicipalityKey}", Municipality.Value);
        }

        private MunicipalityKey GetMunicipalityKey()
        {
            MunicipalityKey municipalityKey;
            switch (_options.Value.Municipality)
            {
                case var _ when _options.Value.Municipality == MunicipalityKey.OS2.Value:
                    municipalityKey = MunicipalityKey.OS2;
                    break;
                case var _ when _options.Value.Municipality == MunicipalityKey.Novataris.Value:
                    municipalityKey = MunicipalityKey.Novataris;
                    break;
                case var _ when _options.Value.Municipality == MunicipalityKey.Ballerup.Value:
                    municipalityKey = MunicipalityKey.Ballerup;
                    break;
                case var _ when _options.Value.Municipality == MunicipalityKey.Kobenhavn.Value:
                    municipalityKey = MunicipalityKey.Kobenhavn;
                    break;
                default:
                    municipalityKey = MunicipalityKey.OS2;
                    break;

            }

            return municipalityKey;
        }

        private void InitializeTexts()
        {
            var dockerEnvironment = Primitives.Logic.Environment.IsRunningInDocker();
            var dockerPath = "/app/wwwroot/api/staticTexts.json";
            var normalPath = "..\\Api\\wwwroot\\api\\staticTexts.json";

            try
            {
                var textFile = File.ReadAllText(dockerEnvironment ? dockerPath : normalPath);
                var json = JsonConvert.DeserializeObject<IDictionary<string, object>>(textFile);

                var municipalityJson = (JObject)json?[Municipality.Value];

                _textObjects = municipalityJson!.ToObject<IDictionary<string, object>>();
            }
            catch (Exception e)
            {
                _logger.LogError("An error occured during TextResolver initialization for municipality-key {MunicipalityKey}", Municipality.Value);
                throw new TextResolverException("Failed to initialize TextResolver", e);
            }
        }

        public string GetText(GroupKey groupKey, TextKey textKey)
        {
            try
            {
                var group = (JObject)_textObjects[groupKey.Value];
                var groupAsDict = group!.ToObject<IDictionary<string, string>>();
                return groupAsDict![textKey.Value];
            }
            catch (Exception e)
            {
                _logger.LogError("An error occured while retrieving text for {MunicipalityKey}, {GroupKey}, {TextKey}", Municipality.Value, groupKey.Value, textKey.Value);
                throw new TextResolverException("Failed to retrieve text", e);
            }
        }
        
    }
}