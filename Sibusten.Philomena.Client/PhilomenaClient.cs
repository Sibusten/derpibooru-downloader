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

        public IPhilomenaImageSearch GetImageSearch(string query, Func<PhilomenaImageSearchBuilder, PhilomenaImageSearchBuilder> buildOptions)
        {
            PhilomenaImageSearchBuilder builder = new PhilomenaImageSearchBuilder(_api, query);
            return buildOptions(builder).Build();
        }
    }
}
