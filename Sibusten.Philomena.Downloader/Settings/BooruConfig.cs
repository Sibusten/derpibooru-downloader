using System.Collections.Generic;
using LiteDB;

namespace Sibusten.Philomena.Downloader.Settings
{
    public class BooruConfig
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string? ApiKey { get; set; }

        public BooruConfig(string baseUrl, string id)
        {
            Id = ObjectId.NewObjectId();
            BaseUrl = baseUrl;
            Name = id;
        }

    }
}
