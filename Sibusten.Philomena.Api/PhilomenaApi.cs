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

        private const string _userAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";

        private string _baseUrl;
        private IFlurlRequest _apiRequest => _baseUrl.AppendPathSegment("api/v1/json").WithHeader("User-Agent", _userAgent);

        public PhilomenaApi(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<ImageSearchModel> SearchImagesAsync(string query, int? page = null, int? perPage = null, int? filterId = null, string? apiKey = null, CancellationToken cancellationToken = default)
        {
            return await _apiRequest
                .AppendPathSegment("search/images")
                .SetQueryParam(_queryParam, query)
                .SetQueryParam(_pageParam, page)
                .SetQueryParam(_perPageParam, perPage)
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
