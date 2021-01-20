using System.Collections.Generic;

namespace Sibusten.Philomena.Downloader.Settings
{
    public class BooruConfig
    {
        public BooruConfig(string baseUrl, string id)
        {
            BaseUrl = baseUrl;
            Id = id;
        }

        public string BaseUrl { get; set; }
        public string Id { get; set; }
        public string? ApiKey { get; set; }
    }
}
