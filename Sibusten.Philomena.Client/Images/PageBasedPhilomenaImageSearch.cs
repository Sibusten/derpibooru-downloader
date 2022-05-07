using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.Logging;
using Sibusten.Philomena.Api;
using Sibusten.Philomena.Api.Models;
using Sibusten.Philomena.Client.Logging;
using Sibusten.Philomena.Client.Options;

namespace Sibusten.Philomena.Client.Images
{
    public class PageBasedPhilomenaImageSearch : IPhilomenaImageSearch
    {
        private ILogger _logger;

        private const int _perPage = 50;

        private PhilomenaApi _api;
        private string _query;
        private ImageSearchOptions _options;

        public PageBasedPhilomenaImageSearch(PhilomenaApi api, string query, ImageSearchOptions? options = null)
        {
            _logger = Logger.Factory.CreateLogger(GetType());

            _api = api;
            _query = query;
            _options = options ?? new ImageSearchOptions();
        }

        public async IAsyncEnumerable<IPhilomenaImage> BeginSearch([EnumeratorCancellation] CancellationToken cancellationToken = default, IProgress<PhilomenaImageSearchProgressInfo>? searchProgress = null)
        {
            // TODO: Optimize this process and make use of multiple threads
            // TODO: Enumerate using id.gt/id.lt when possible

            // Start on the first page
            int page = 1;

            // Track images processed
            int imagesProcessed = 0;

            // Set the random seed if needed
            int? _randomSeed = _options.SortOptions?.RandomSeed;
            if (_options.SortOptions?.SortField == SortField.Random && _options.SortOptions?.RandomSeed is null)
            {
                _randomSeed = _api.GetRandomSearchSeed();  // TODO: Expose this method in the API interface

                _logger.LogDebug("Search sort field is Random and a seed was not provided. Generated a new seed: {RandomSeed}", _randomSeed);
            }

            _logger.LogDebug("Beginning image search for '{Query}'", _query);

            // Enumerate images
            ImageSearchModel search;
            do
            {
                _logger.LogDebug("Downloading page {Page} of image search '{Query}'", page, _query);

                // Get the current page of images
                search = await _api.SearchImagesAsync(_query, page, _perPage, _options.SortOptions?.SortField, _options.SortOptions?.SortDirection, _options.FilterId, _options.ApiKey, _randomSeed, cancellationToken);

                if (search.Images is null)
                {
                    throw new InvalidOperationException("The search query did not provide a list of images");
                }

                if (search.Total is null)
                {
                    throw new InvalidOperationException("The search query did not provide a total image count");
                }

                // Determine how much metadata has been downloaded
                int metadataDownloaded = imagesProcessed + search.Images.Count;

                // Cap the total number of images downloaded at the limit
                int totalImagesToDownload = Math.Min(search.Total.Value, _options.MaxImages);

                // Avoid reporting more metadata downloaded than total images to download
                metadataDownloaded = Math.Min(metadataDownloaded, totalImagesToDownload);

                _logger.LogDebug("Downloaded metadata for {MetadataDownloaded}/{TotalImagesToDownload} images", metadataDownloaded, totalImagesToDownload);

                // Update search progress
                searchProgress?.Report(new()
                {
                    MetadataDownloaded = metadataDownloaded,
                    TotalImages = totalImagesToDownload
                });

                // Yield the images
                foreach (ImageModel imageModel in search.Images)
                {
                    IPhilomenaImage image = new PhilomenaImage(imageModel);

                    // Skip images that do not yet have generated thumbnails
                    if (!(image.ThumbnailsGenerated ?? false))
                    {
                        _logger.LogDebug("Skipping image {ImageId}: Thumbnails not generated", image.Id);

                        continue;
                    }

                    _logger.LogDebug("Processing image {ImageId}", image.Id);

                    yield return image;

                    imagesProcessed++;

                    _logger.LogDebug("Processed {ImagesProcessed}/{TotalImagesToProcess} images", imagesProcessed, totalImagesToDownload);

                    if (imagesProcessed >= _options.MaxImages)
                    {
                        _logger.LogDebug("Image search '{Query}' stopping due to reaching the max number of images ({MaxImages})", _query, _options.MaxImages);

                        yield break;
                    }
                }

                // Move to the next page
                page++;
            }
            while (search.Images.Any());  // Stop when there are no more images
        }
    }
}
