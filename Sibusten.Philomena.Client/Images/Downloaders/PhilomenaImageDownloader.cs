using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Sibusten.Philomena.Client.Logging;
using Sibusten.Philomena.Client.Utilities;

namespace Sibusten.Philomena.Client.Images.Downloaders
{
    public abstract class PhilomenaImageDownloader : IPhilomenaImageDownloader
    {
        private ILogger _logger;

        public PhilomenaImageDownloader()
        {
            _logger = Logger.Factory.CreateLogger(GetType());
        }

        public abstract Task Download(IPhilomenaImage downloadItem, CancellationToken cancellationToken = default, IProgress<PhilomenaImageDownloadProgressInfo>? progress = null);

        /// <summary>
        /// Gets a stream for downloading an image
        /// </summary>
        /// <param name="image">The image to download</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <param name="progress">The progress of the image download</param>
        /// <exception cref="FlurlHttpException">Thrown when the image fails to download</exception>
        /// <returns>A stream for downloading an image</returns>
        protected async Task<Stream> GetDownloadStream(IPhilomenaImage image, CancellationToken cancellationToken, IProgress<StreamProgressInfo>? progress, bool isSvgVersion = false)
        {
            string? imageUrl = isSvgVersion ? image.ShortSvgViewUrl : image.ShortViewUrl;
            IFlurlResponse response = await imageUrl.GetAsync(cancellationToken, HttpCompletionOption.ResponseHeadersRead);

            // Attempt to read the length of the stream from the header
            long? length = null;
            if (response.Headers.TryGetFirst("Content-Length", out string lengthString))
            {
                if (long.TryParse(lengthString, out long parsedLength))
                {
                    length = parsedLength;
                }
            }

            // Open the image stream
            Stream downloadStream = await response.GetStreamAsync();

            _logger.LogDebug("Opened download stream for image {ImageId} with size {DownloadSize}: {DownloadUrl}", image.Id, length, image.ShortViewUrl);

            // Create progress stream wrapper for reporting download progress
            return new StreamProgressReporter(downloadStream, progress, length);
        }
    }
}
