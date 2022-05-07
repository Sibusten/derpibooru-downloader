using System.Collections.Generic;
using System.IO;
using Sibusten.Philomena.Api;
using Sibusten.Philomena.Client.Options;

namespace Sibusten.Philomena.Downloader.Settings
{
    public class SearchConfig
    {
        public const int NoLimit = -1;
        public const int NoFilter = -1;

        public string Query { get; set; } = "*";
        public int ImageLimit { get; set; } = NoLimit;
        public int Filter { get; set; } = NoFilter;
        public string ImagePathFormat { get; set; } = Path.Join("Downloads", "{id}.{ext}");
        public string JsonPathFormat { get; set; } = Path.Join("Downloads", "Json", "{id}.json");
        public bool ShouldSaveJson { get; set; } = false;
        public bool ShouldSaveComments { get; set; } = false;
        public bool ShouldSaveFavorites { get; set; } = false;
        public string Booru { get; set; } = "https://derpibooru.org";
        public SvgMode SvgMode { get; set; } = SvgMode.RasterOnly;
    }
}
