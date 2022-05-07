using System;
using System.Threading.Tasks;
using Sibusten.Philomena.Api.Models;
using Sibusten.Philomena.Client.Fluent.Images;
using Sibusten.Philomena.Client.Images;
using Sibusten.Philomena.Client.Options;

namespace Sibusten.Philomena.Client
{
    public interface IPhilomenaClient
    {
        /// <summary>
        /// Gets an image search
        /// </summary>
        /// <param name="query">The search query</param>
        /// <param name="buildOptions">Configures the search options builder</param>
        /// <returns>An image search</returns>
        IPhilomenaImageSearch GetImageSearch(string query, Func<PhilomenaImageSearchBuilder, PhilomenaImageSearchBuilder> buildOptions);
    }
}
