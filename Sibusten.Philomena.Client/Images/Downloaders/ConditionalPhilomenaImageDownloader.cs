using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sibusten.Philomena.Client.Logging;

namespace Sibusten.Philomena.Client.Images.Downloaders
{
    public delegate bool ShouldDownloadImageDelegate(IPhilomenaImage image);

    public class ConditionalImageDownloader : PhilomenaImageDownloader
    {
        private ILogger _logger;

        private readonly IPhilomenaImageDownloader _downloader;
        private readonly ShouldDownloadImageDelegate _shouldDownloadImage;

        public ConditionalImageDownloader(ShouldDownloadImageDelegate shouldDownloadImage, IPhilomenaImageDownloader downloader)
        {
            _logger = Logger.Factory.CreateLogger(GetType());

            _shouldDownloadImage = shouldDownloadImage;
            _downloader = downloader;
        }

        public override async Task Download(IPhilomenaImage downloadItem, CancellationToken cancellationToken = default, IProgress<PhilomenaImageDownloadProgressInfo>? progress = null)
        {
            bool shouldDownloadImage = _shouldDownloadImage(downloadItem);

            _logger.LogDebug("Condition for downloading image {ImageId}: {ConditionResult}", downloadItem.Id, shouldDownloadImage);

            if (shouldDownloadImage)
            {
                await _downloader.Download(downloadItem, cancellationToken, progress);
            }
        }
    }
}
