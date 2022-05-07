using System;
using System.Collections.Generic;
using System.Threading;
using Sibusten.Philomena.Api;
using Sibusten.Philomena.Client.Images;
using Sibusten.Philomena.Client.Options;

namespace Sibusten.Philomena.Client.Fluent.Images
{
    public class PhilomenaImageSearchBuilder
    {
        private ImageSearchOptions _options { get; set; }
        private PhilomenaApi _api { get; }
        private string _query { get; }

        public PhilomenaImageSearchBuilder(PhilomenaApi api, string query)
        {
            _options = new();
            _api = api;
            _query = query;
        }

        /// <summary>
        /// Builds the search
        /// </summary>
        public IPhilomenaImageSearch Build()
        {
            // TODO: Allow changing what type of search to use (page-based, id-based, parallel)
            return new PageBasedPhilomenaImageSearch(_api, _query, _options);
        }

        /// <summary>
        /// The API Key to use when querying
        /// </summary>
        /// <value></value>
        public PhilomenaImageSearchBuilder WithApiKey(string apiKey)
        {
            _options = _options with
            {
                ApiKey = apiKey
            };
            return this;
        }

        /// <summary>
        /// The filter for the query
        /// </summary>
        public PhilomenaImageSearchBuilder WithFilterId(int filterId)
        {
            _options = _options with
            {
                FilterId = filterId
            };
            return this;
        }
    }
}
