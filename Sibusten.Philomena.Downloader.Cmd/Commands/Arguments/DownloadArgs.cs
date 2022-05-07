using Sibusten.Philomena.Downloader.Settings;

namespace Sibusten.Philomena.Downloader.Cmd.Commands.Arguments
{
    public class DownloadArgs
    {
        public string Query { get; set; } = default!;
        public string? ApiKey { get; set; }
        public int? Filter { get; set; }
        public string? ImagePath { get; set; }
        public string? JsonPath { get; set; }
        public bool? SaveJson { get; set; }
        public string? Booru { get; set; }
        public SvgMode? SvgMode { get; set; }

        public SearchConfig GetSearchConfig()
        {
            SearchConfig baseConfig = new SearchConfig(Query);

            // Build a new search config based on the base config
            // Values are overridden if specified in the arguments
            return new SearchConfig(Query)
            {
                Filter = Filter is not null ? Filter.Value : baseConfig.Filter,
                ImagePathFormat = ImagePath is not null ? ImagePath : baseConfig.ImagePathFormat,
                JsonPathFormat = JsonPath is not null ? JsonPath : baseConfig.JsonPathFormat,
                Booru = Booru ?? baseConfig.Booru,
                SvgMode = SvgMode is not null ? SvgMode.Value : baseConfig.SvgMode,
            };
        }
    }
}
