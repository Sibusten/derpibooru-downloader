using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sibusten.Philomena.Api;
using Sibusten.Philomena.Api.Models;
using Sibusten.Philomena.Client.Fluent.Images;
using Sibusten.Philomena.Client.Images;
using Sibusten.Philomena.Client.Logging;
using Sibusten.Philomena.Client.Options;

namespace Sibusten.Philomena.Client
{
    public class PhilomenaClient : IPhilomenaClient
    {
        private ILogger _logger;

        private PhilomenaApi _api;

        public string? ApiKey { get; set; } = null;

        public PhilomenaClient(string baseUrl)
        {
            _logger = Logger.Factory.CreateLogger(GetType());

            _api = new PhilomenaApi(baseUrl);
        }

        public IPhilomenaImageSearch GetImageSearch(string query)
        {
            return new PageBasedPhilomenaImageSearch(_api, query);
        }

        public IPhilomenaImageSearch GetImageSearch(string query, ImageSearchOptions searchOptions)
        {
            return new PageBasedPhilomenaImageSearch(_api, query, searchOptions);
        }

        public IPhilomenaImageSearch GetImageSearch(string query, Func<PhilomenaImageSearchBuilder, PhilomenaImageSearchBuilder> buildOptions)
        {
            PhilomenaImageSearchBuilder builder = new PhilomenaImageSearchBuilder(_api, query);
            return buildOptions(builder).Build();
        }

        public async Task<TagModel> GetTagById(int tagId)
        {
            string tagQuery = $"id:{tagId}";

            _logger.LogDebug("Searching for tags: '{Query}'", tagQuery);

            TagSearchModel tagSearch = await _api.SearchTagsAsync(tagQuery, page: 1, perPage: 1);

            if (tagSearch.Tags is null)
            {
                throw new InvalidOperationException("The search query did not provide a list of tags");
            }

            if (!tagSearch.Tags.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(tagId), tagId, "A tag with this ID was not found");
            }

            return tagSearch.Tags.First();
        }
    }
}
