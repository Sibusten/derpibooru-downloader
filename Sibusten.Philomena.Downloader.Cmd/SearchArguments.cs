using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Sibusten.Philomena.Api;
using Sibusten.Philomena.Downloader.Settings;

namespace Sibusten.Philomena.Downloader.Cmd
{
    // TODO: Defaults
    public class SearchArguments
    {
        [Option('p', "preset", Required = false, HelpText = "The preset to use. If no preset is given, the default is used.")]
        public string? Preset { get; set; }

        [Option('q', "query", Required = false, HelpText = "The search query.")]
        public string? Query { get; set; }

        [Option('l', "limit", Required = false, HelpText = "The maximum number of images to download. Defaults to all images.")]
        public int? Limit { get; set; }

        [Option('f', "filter", Required = false, HelpText = "The ID of the filter to use.")]
        public int? Filter { get; set; }

        [Option('S', "sort-field", Required = false, HelpText = "How to sort images.")]
        public SortField? SortField { get; set; }

        [Option('D', "sort-direction", Required = false, HelpText = "The direction to sort images.")]
        public SortDirection? SortDirection { get; set; }

        [Option('I', "image-path", Required = false, HelpText = "Where to save images and how to name them.")]
        public string? ImagePath { get; set; }

        [Option('J', "json-path", Required = false, HelpText = "Where to save json files and how to name them.")]
        public string? JsonPath { get; set; }

        [Option('i', "skip-images", Required = false, HelpText = "Skip saving images.")]
        public bool? ShouldSkipImages { get; set; }

        [Option('j', "save-json", Required = false, HelpText = "Save json metadata files.")]
        public bool? ShouldSaveJson { get; set; }

        [Option('u', "update-json", HelpText = "Overwrite json metadata files with new data.")]
        public bool? ShouldUpdateJson { get; set; }

        [Option('b', "booru", HelpText = "What booru to download from. Multiple boorus can be given.")]
        public IEnumerable<string> Boorus { get; set; } = new List<string>();

        [Option('a', "api-key", HelpText = "The API key to use.")]
        public string? ApiKey { get; set; }


        public SearchConfig ToSearchConfig()
        {
            SearchConfig baseConfig;
            if (Preset is not null)
            {
                // TODO: Load the given preset
                baseConfig = new SearchConfig();
            }
            else
            {
                baseConfig = new SearchConfig();
            }

            // Build a new search config based on the default config (or selected preset)
            // Values are overridden if specified in the arguments
            return new SearchConfig()
            {
                Filter = Filter is not null ? Filter.Value : baseConfig.Filter,
                ImageLimit = Limit is not null ? Limit.Value : baseConfig.ImageLimit,
                ImagePathFormat = ImagePath is not null ? ImagePath : baseConfig.ImagePathFormat,
                JsonPathFormat = JsonPath is not null ? JsonPath : baseConfig.JsonPathFormat,
                Query = Query is not null ? Query : baseConfig.Query,
                ShouldSaveImages = ShouldSkipImages is not null ? !ShouldSkipImages.Value : baseConfig.ShouldSaveImages,
                ShouldSaveJson = ShouldSaveJson is not null ? ShouldSaveJson.Value : baseConfig.ShouldSaveJson,
                ShouldUpdateJson = ShouldUpdateJson is not null ? ShouldUpdateJson.Value : baseConfig.ShouldUpdateJson,
                SortDirection = SortDirection is not null ? SortDirection.Value : baseConfig.SortDirection,
                SortField = SortField is not null ? SortField.Value : baseConfig.SortField,
                Boorus = Boorus.Any() ? Boorus.ToList() : baseConfig.Boorus,
            };
        }
    }
}
