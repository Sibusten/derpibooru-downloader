using static Dasync.Collections.ParallelForEachExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Sibusten.Philomena.Client.Options;
using System.Collections.Concurrent;
using Flurl.Http;
using Sibusten.Philomena.Client.Logging;
using Microsoft.Extensions.Logging;
using Sibusten.Philomena.Client.Utilities;

namespace Sibusten.Philomena.Client.Images.Downloaders
{
    public class ParallelPhilomenaImageSearchDownloader : IPhilomenaImageSearchDownloader
    {
        private ILogger _logger;

        private readonly IPhilomenaImageSearch _imageSearch;
        private readonly IPhilomenaImageDownloader _imageDownloader;
        private readonly int _maxDownloadThreads;

        public ParallelPhilomenaImageSearchDownloader(IPhilomenaImageSearch imageSearch, IPhilomenaImageDownloader imageDownloader, int maxDownloadThreads)
        {
            _logger = Logger.Factory.CreateLogger(GetType());

            _imageSearch = imageSearch;
            _imageDownloader = imageDownloader;
            _maxDownloadThreads = maxDownloadThreads;
        }

        public async Task BeginDownload(
            CancellationToken cancellationToken = default,
            IProgress<PhilomenaImageSearchProgressInfo>? searchProgress = null,
            IProgress<PhilomenaImageSearchDownloadProgressInfo>? searchDownloadProgress = null,
            IReadOnlyCollection<IProgress<PhilomenaImageDownloadProgressInfo>>? individualDownloadProgresses = null)
        {
            ConcurrentBag<IProgress<PhilomenaImageDownloadProgressInfo>>? availableProgress = null;

            if (individualDownloadProgresses is not null)
            {
                // Ensure enough progress entries are provided
                if (individualDownloadProgresses.Count != _maxDownloadThreads)
                {
                    throw new ArgumentException($"Expected {_maxDownloadThreads} progress entries, but {individualDownloadProgresses.Count} were provided.", nameof(individualDownloadProgresses));
                }

                // Copy progress entries to a thread safe structure
                availableProgress = new ConcurrentBag<IProgress<PhilomenaImageDownloadProgressInfo>>(individualDownloadProgresses);
            }

            // Track images downloaded
            int imagesDownloaded = 0;

            // The total images to download is unknown until the image search reports it
            int totalImages = -1;

            // An object to lock on for reporting the search download progress
            object searchDownloadProgressLock = new object();

            // Wrap the image search progress to track the total number of images to download
            IProgress<PhilomenaImageSearchProgressInfo> wrappedSearchProgress = new SyncProgress<PhilomenaImageSearchProgressInfo>(progress =>
            {
                totalImages = progress.TotalImages;
                searchProgress?.Report(progress);
            });

            // Download the images using as many threads as configured
            IAsyncEnumerable<IPhilomenaImage> imagesToDownload = _imageSearch.BeginSearch(cancellationToken, wrappedSearchProgress);
            await imagesToDownload.ParallelForEachAsync
            (
                async (image) =>
                {
                    _logger.LogDebug("Downloading image {ImageId}", image.Id);

                    // Take a progress slot for this image
                    IProgress<PhilomenaImageDownloadProgressInfo>? imageProgress = null;
                    availableProgress?.TryTake(out imageProgress);

                    // Download the image
                    await _imageDownloader.Download(image, cancellationToken, imageProgress);

                    // Report search download progress
                    lock (searchDownloadProgressLock)
                    {
                        imagesDownloaded++;
                        searchDownloadProgress?.Report(new()
                        {
                            ImagesDownloaded = imagesDownloaded,
                            ImagesTotal = totalImages
                        });
                    }

                    // Make individual download progress available if one was taken
                    if (imageProgress is not null)
                    {
                        availableProgress!.Add(imageProgress);
                    }
                },
                maxDegreeOfParallelism: _maxDownloadThreads,
                cancellationToken: cancellationToken
            );
        }
    }
}
