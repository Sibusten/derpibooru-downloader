using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Downloader.Settings
{
    public class BooruConfig
    {
        [JsonIgnore]
        public ObjectId Id { get; set; } = ObjectId.NewObjectId();
        public string Name { get; set; } = "";
        public string BaseUrl { get; set; } = "";
        public string? ApiKey { get; set; }

        private BooruConfig() { }

        public BooruConfig(string baseUrl, string id)
        {
            BaseUrl = baseUrl;
            Name = id;
        }
    }
}
