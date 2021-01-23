using System.Collections.Generic;
using System.Linq;
using Sibusten.Philomena.Api;
using Sibusten.Philomena.Downloader.Settings;

namespace Sibusten.Philomena.Downloader.Cmd.Commands.Common.Arguments
{
    public class SearchQueryArgs
    {
        public string? Query { get; set; }
        public int? Limit { get; set; }
        public int? Filter { get; set; }
        public SortField? SortField { get; set; }
        public SortDirection? SortDirection { get; set; }
        public string? ImagePath { get; set; }
        public string? JsonPath { get; set; }
        public bool? SkipImages { get; set; }
        public bool? SaveJson { get; set; }
        public bool? UpdateJson { get; set; }
        public List<string> Boorus { get; set; } = new List<string>();

        public SearchConfig GetSearchConfig(SearchConfig? baseConfig = null)
        {
            if (baseConfig is null)
            {
                baseConfig = new SearchConfig();
            }

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
                SortDirection = SortDirection is not null ? SortDirection.Value : baseConfig.SortDirection,
                SortField = SortField is not null ? SortField.Value : baseConfig.SortField,
                Boorus = Boorus.Any() ? Boorus : baseConfig.Boorus,
            };
        }
    }
}
