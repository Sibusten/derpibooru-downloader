using Flurl;
using Flurl.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sibusten.Philomena.Api.Models;
using System;
using System.Threading;

namespace Sibusten.Philomena.Api
{
    public class PhilomenaApi
    {
        private const string _filterIdParam = "filter_id";
        private const string _apiKeyParam = "key";
        private const string _pageParam = "page";
        private const string _perPageParam = "per_page";
        private const string _queryParam = "q";
        private const string _sortDirectionParam = "sd";
        private const string _sortFieldParam = "sf";

        private const string _userAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";

        private string _baseUrl;
        private IFlurlRequest _apiRequest => _baseUrl.AppendPathSegment("api/v1/json").WithHeader("User-Agent", _userAgent);

        private readonly Random randomSeed = new Random();

        private string GetSortDirectionParamValue(SortDirection sortDirection)
        {
            return sortDirection switch
            {
                SortDirection.Descending => "desc",
                SortDirection.Ascending => "asc",

                _ => throw new ArgumentOutOfRangeException(nameof(sortDirection))
            };
        }

        private string GetSortFieldParamValue(SortField sortField, int? randomSeed)
        {
            string sortFieldParamValue = sortField switch
            {
                SortField.ImageId => "id",
                SortField.LastModificationDate => "updated_at",
                SortField.InitialPostDate => "first_seen_at",
                SortField.AspectRatio => "aspect_ratio",
                SortField.FaveCount => "faves",
                SortField.Upvotes => "upvotes",
                SortField.Downvotes => "downvotes",
                SortField.Score => "score",
                SortField.WilsonScore => "wilson_score",
                SortField.Relevance => "_score",
                SortField.Width => "width",
                SortField.Height => "height",
                SortField.Comments => "comment_count",
                SortField.TagCount => "tag_count",
                SortField.Pixels => "pixels",
                SortField.FileSize => "size",
                SortField.Duration => "duration",
                SortField.Random => "random",

                _ => throw new ArgumentOutOfRangeException(nameof(sortField))
            };

            // Add seed to random searches if provided.
            if (sortField == SortField.Random && randomSeed is not null)
            {
                // Colon followed by the seed value
                sortFieldParamValue += $":{randomSeed}";
            }

            return sortFieldParamValue;
        }

        /// <summary>
        /// Generates a random seed for use in random image searches. This seed should be reused when requesting multiple pages to prevent duplicates.
        /// </summary>
        /// <returns>A random seed</returns>
        public int GetRandomSearchSeed()
        {
            lock (randomSeed)
            {
                return randomSeed.Next();
            }
        }

        public PhilomenaApi(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<ImageSearchModel> SearchImagesAsync(string query, int? page = null, int? perPage = null, SortField? sortField = null, SortDirection? sortDirection = null, int? filterId = null, string? apiKey = null, int? randomSeed = null, CancellationToken cancellationToken = default)
        {
            string? sortFieldParamValue = (sortField is null) ? null : GetSortFieldParamValue(sortField.Value, randomSeed);
            string? sortDirectionParamValue = (sortDirection is null) ? null : GetSortDirectionParamValue(sortDirection.Value);

            return await _apiRequest
                .AppendPathSegment("search/images")
                .SetQueryParam(_queryParam, query)
                .SetQueryParam(_pageParam, page)
                .SetQueryParam(_perPageParam, perPage)
                .SetQueryParam(_sortFieldParam, sortFieldParamValue)
                .SetQueryParam(_sortDirectionParam, sortDirectionParamValue)
                .SetQueryParam(_filterIdParam, filterId)
                .SetQueryParam(_apiKeyParam, apiKey)
                .GetJsonAsync<ImageSearchModel>(cancellationToken);
        }

        public async Task<TagSearchModel> SearchTagsAsync(string query, int? page = null, int? perPage = null, CancellationToken cancellationToken = default)
        {
            return await _apiRequest
                .AppendPathSegment("search/tags")
                .SetQueryParam(_queryParam, query)
                .SetQueryParam(_pageParam, page)
                .SetQueryParam(_perPageParam, perPage)
                .GetJsonAsync<TagSearchModel>(cancellationToken);
        }
    }
}
