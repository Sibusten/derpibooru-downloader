using Sibusten.Philomena.Downloader.Settings;

namespace Sibusten.Philomena.Downloader.Cmd.Commands.Arguments
{
    public class DownloadArgs
    {
        public string? ApiKey { get; set; }
        public string? Query { get; set; }
        public int? Limit { get; set; }
        public int? Filter { get; set; }
        public string? ImagePath { get; set; }
        public string? JsonPath { get; set; }
        public bool? SkipImages { get; set; }
        public bool? SaveJson { get; set; }
        public bool? UpdateJson { get; set; }
        public string? Booru { get; set; }
        public SvgMode? SvgMode { get; set; }

        public SearchConfig GetSearchConfig()
        {
            SearchConfig baseConfig = new SearchConfig();

            // Build a new search config based on the base config
            // Values are overridden if specified in the arguments
            return new SearchConfig()
            {
                Filter = Filter is not null ? Filter.Value : baseConfig.Filter,
                ImageLimit = Limit is not null ? Limit.Value : baseConfig.ImageLimit,
                ImagePathFormat = ImagePath is not null ? ImagePath : baseConfig.ImagePathFormat,
                JsonPathFormat = JsonPath is not null ? JsonPath : baseConfig.JsonPathFormat,
                Query = Query is not null ? Query : baseConfig.Query,
                ShouldSaveImages = SkipImages is not null ? !SkipImages.Value : baseConfig.ShouldSaveImages,
                ShouldSaveJson = SaveJson is not null ? SaveJson.Value : baseConfig.ShouldSaveJson,
                ShouldUpdateJson = UpdateJson is not null ? UpdateJson.Value : baseConfig.ShouldUpdateJson,
                Booru = Booru ?? baseConfig.Booru,
                SvgMode = SvgMode is not null ? SvgMode.Value : baseConfig.SvgMode,
            };
        }
    }
}
