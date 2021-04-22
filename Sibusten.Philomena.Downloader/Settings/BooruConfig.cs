using System;
using LiteDB;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Downloader.Settings
{
    public class BooruConfig
    {
        [JsonIgnore]
        public ObjectId Id { get; set; } = ObjectId.NewObjectId();
        public string Name { get; set; } = "";
        public string BaseUrl
        {
            get => _baseUrl;
            set
            {
                // If the URL is valid, use it
                if (Uri.TryCreate(value, UriKind.Absolute, out Uri? uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    _baseUrl = value;
                    return;
                }

                // If the URL was not valid, try adding the https scheme
                string valueWithHttps = $"https://{value}";
                if (Uri.TryCreate(valueWithHttps, UriKind.Absolute, out _))
                {
                    _baseUrl = valueWithHttps;
                    return;
                }

                throw new FormatException($"Could not parse URL '{value}'");
            }
        }
        private string _baseUrl = "";

        public string? ApiKey { get; set; }

        private BooruConfig() { }

        public BooruConfig(string name, string baseUrl)
        {
            Name = name;
            BaseUrl = baseUrl;
        }
    }
}
