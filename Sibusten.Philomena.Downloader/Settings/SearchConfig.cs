using System.Collections.Generic;
using System.IO;
using Sibusten.Philomena.Api;
using Sibusten.Philomena.Client.Options;

namespace Sibusten.Philomena.Downloader.Settings
{
    public class SearchConfig
    {
        public const int NoFilter = -1;

        public string Query { get; set; }
        public int Filter { get; set; } = NoFilter;
        public string ImagePathFormat { get; set; } = Path.Join("Downloads", "{id}.{ext}");
        public string? JsonPathFormat { get; set; }
        public string Booru { get; set; } = "https://derpibooru.org";
        public SvgMode SvgMode { get; set; } = SvgMode.RasterOnly;

        public SearchConfig(string query) =>
            Query = query;
    }
}
