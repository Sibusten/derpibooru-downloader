using System.Collections.Generic;

namespace Sibusten.Philomena.Downloader.Settings
{
    public class BooruConfig
    {
        public BooruConfig(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; set; }

        public string? ApiKey { get; set; }

        public Dictionary<string, SearchConfig> Presets { get; set; } = new Dictionary<string, SearchConfig>();
    }
}
