using LiteDB;

namespace Sibusten.Philomena.Downloader.Settings
{
    public class SearchPreset
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public SearchConfig Config { get; set; }

        public SearchPreset(string name, SearchConfig config)
        {
            Id = ObjectId.NewObjectId();
            Name = name;
            Config = config;
        }
    }
}
