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
        /// Gets an image search with default options
        /// </summary>
        /// <param name="query">The search query</param>
        /// <returns>An image search</returns>
        IPhilomenaImageSearch GetImageSearch(string query);

        /// <summary>
        /// Gets an image search
        /// </summary>
        /// <param name="query">The search query</param>
        /// <param name="searchOptions">The search options</param>
        /// <returns>An image search</returns>
        IPhilomenaImageSearch GetImageSearch(string query, ImageSearchOptions searchOptions);

        /// <summary>
        /// Gets an image search
        /// </summary>
        /// <param name="query">The search query</param>
        /// <param name="buildOptions">Configures the search options builder</param>
        /// <returns>An image search</returns>
        IPhilomenaImageSearch GetImageSearch(string query, Func<PhilomenaImageSearchBuilder, PhilomenaImageSearchBuilder> buildOptions);

        /// <summary>
        /// Gets a tag by its ID
        /// </summary>
        /// <param name="tagId">The ID of the tag</param>
        /// <returns>The tag model</returns>
        Task<TagModel> GetTagById(int tagId);

        /// <summary>
        /// Gets a tag by its name
        /// </summary>
        /// <param name="tagName">The name of the tag</param>
        /// <returns>The tag model</returns>
        // TagModel GetTagByName(string tagName)
    }
}
