using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Sibusten.Philomena.Client.Logging;
using Sibusten.Philomena.Client.Utilities;

namespace Sibusten.Philomena.Client.Images.Downloaders
{
    public class PhilomenaImageSvgFileDownloader : IPhilomenaImageDownloader
    {
        private ILogger _logger;

        private readonly GetFileForImageDelegate _getFileForImage;

        public PhilomenaImageSvgFileDownloader(GetFileForImageDelegate getFileForImage)
        {
            _logger = Logger.Factory.CreateLogger(GetType());

            _getFileForImage = getFileForImage;
        }

        public async Task Download(IPhilomenaImage downloadItem, CancellationToken cancellationToken = default, IProgress<PhilomenaImageDownloadProgressInfo>? progress = null)
        {
            if (!downloadItem.IsSvgImage)
            {
                _logger.LogDebug("Cannot download SVG since image {ImageId} is not an SVG image", downloadItem.Id);
                return;
            }

            try
            {
                string file = _getFileForImage(downloadItem);

                FileUtilities.CreateDirectoryForFile(file);

                // Create stream progress info
                IProgress<StreamProgressInfo> streamProgress = new SyncProgress<StreamProgressInfo>(streamProgress =>
                {
                    progress?.Report(new PhilomenaImageDownloadProgressInfo
                    {
                        Current = streamProgress.BytesRead,
                        Total = streamProgress.BytesTotal,
                        Action = $"Downloading image {downloadItem.Id} (SVG)"
                    });
                });

                // Get the download stream for the image
                using Stream downloadStream = await UrlUtilities.GetProgressWrappedDownloadStream(downloadItem.ShortSvgViewUrl, streamProgress, cancellationToken);

                _logger.LogDebug("Saving SVG image {ImageId} to {File}", downloadItem.Id, file);

                await FileUtilities.SafeFileWrite(file, async tempFile =>
                {
                    using FileStream tempFileStream = File.OpenWrite(tempFile);
                    await downloadStream.CopyToAsync(tempFileStream, cancellationToken);
                });
            }
            catch (Exception ex) when (ex is FlurlHttpException or IOException)
            {
                _logger.LogWarning(ex, "Failed to download SVG image {ImageId}", downloadItem.Id);
            }
        }
    }
}
