using System.Collections.Generic;
using System.Linq;
using IniParser;
using IniParser.Model;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Downloader.Settings.Legacy
{
    public class LegacyConfigFileLoader
    {
        public static BooruConfig LoadLegacyConfigFile(string configFilePath)
        {
            // Read the ini file and load the section where all values are stored
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(configFilePath);
            KeyDataCollection generalSection = data["General"];

            // TODO: Should the current options be ported?
            // string currentOptions = JsonConvert.DeserializeObject<string>(generalSection["currentOptions"]);
            // LegacyPreset currentOptionsPreset = JsonConvert.DeserializeObject<LegacyPreset>(currentOptions);

            // Get the legacy presets
            string presets = JsonConvert.DeserializeObject<string>(generalSection["presets"]);
            Dictionary<string, LegacyPreset> legacyPresets = JsonConvert.DeserializeObject<Dictionary<string, LegacyPreset>>(presets);

            // Convert the legacy presets to search configs
            Dictionary<string, SearchConfig> searchConfigs = new Dictionary<string, SearchConfig>();
            foreach (KeyValuePair<string, LegacyPreset> kvp in legacyPresets)
            {
                SearchConfig searchConfig = ConfigMigrations.LegacyPresetToSearchConfig(kvp.Value);
                searchConfigs.Add(kvp.Key, searchConfig);
            }

            // Ensure API key is null if it is an empty string
            string? apiKey = generalSection["apiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = null;
            }

            // If there is no booru URL, default to Derpibooru
            string? booruUrl = generalSection["booruUrl"];
            if (booruUrl is null)
            {
                booruUrl = "https://derpibooru.org/";
            }

            // Create the booru from legacy settings
            BooruConfig booruConfig = new BooruConfig(booruUrl);
            booruConfig.ApiKey = apiKey;
            booruConfig.Presets = searchConfigs;

            return booruConfig;
        }
    }
}
