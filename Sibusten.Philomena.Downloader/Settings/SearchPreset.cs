using Newtonsoft.Json;

namespace Sibusten.Philomena.Downloader.Settings
{
    public class SearchPreset
    {
        public string Name { get; set; } = "";
        public SearchConfig Config { get; set; } = new SearchConfig();

        private SearchPreset() { }

        public SearchPreset(string name, SearchConfig config)
        {
            Name = name;
            Config = config;
        }
    }
}
