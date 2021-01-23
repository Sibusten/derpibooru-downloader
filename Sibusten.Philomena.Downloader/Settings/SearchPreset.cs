using LiteDB;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Downloader.Settings
{
    public class SearchPreset
    {
        [JsonIgnore]
        public ObjectId Id { get; set; } = ObjectId.NewObjectId();
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
