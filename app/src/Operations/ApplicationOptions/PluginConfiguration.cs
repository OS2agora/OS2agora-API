using System.Collections.Generic;

namespace Agora.Operations.ApplicationOptions
{
    public class PluginOptions
    {
        public const string Plugin = Primitives.Constants.AppSettingsEntries.ConfigurationSectionPlugins;

        public IEnumerable<string> LoadAllFromNameSpaces { get; set; }

        public Dictionary<string, PluginConfiguration> Configurations { get; set; } =
            new Dictionary<string, PluginConfiguration>();
    }

    public class PluginConfiguration
    {
        public bool Disabled { get; set; }
        public bool Mocked { get; set; }
        public List<EventConfigurations> EventConfigurations { get; set; } = new List<EventConfigurations>();
    }

    public class EventConfigurations
    {
        public string Name { get; set; }
        public bool Disabled { get; set; }
    }
}