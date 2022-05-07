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
    public class PhilomenaImageSvgFileDownloader : PhilomenaImageDownloader
    {
        private ILogger _logger;

        private const string _tempExtension = "tmp";

        private readonly GetFileForImageDelegate _getFileForImage;

        public PhilomenaImageSvgFileDownloader(GetFileForImageDelegate getFileForImage)
        {
            _logger = Logger.Factory.CreateLogger(GetType());

            _getFileForImage = getFileForImage;
        }

        public override async Task Download(IPhilomenaImage downloadItem, CancellationToken cancellationToken = default, IProgress<PhilomenaImageDownloadProgressInfo>? progress = null)
        {
            if (!downloadItem.IsSvgImage)
            {
                _logger.LogDebug("Cannot download SVG since image {ImageId} is not an SVG image", downloadItem.Id);
                return;
            }

            try
            {
                string file = _getFileForImage(downloadItem);

                // Create directory for image download
                string? imageDirectory = Path.GetDirectoryName(file);
                if (imageDirectory is null)
                {
                    throw new DirectoryNotFoundException($"The file does not have a parent directory: {file}");
                }
                Directory.CreateDirectory(imageDirectory);

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
                using Stream downloadStream = await GetDownloadStream(downloadItem, cancellationToken, streamProgress, isSvgVersion: true);

                _logger.LogDebug("Saving SVG image {ImageId} to {File}", downloadItem.Id, file);

                // Write to a temp file first
                string tempFile = file + "." + _tempExtension;
                using (FileStream tempFileStream = File.OpenWrite(tempFile))
                {
                    await downloadStream.CopyToAsync(tempFileStream, cancellationToken);
                }

                // Move the temp file to the destination file
                File.Move(tempFile, file, overwrite: true);
            }
            catch (Exception ex) when (ex is FlurlHttpException or IOException)
            {
                _logger.LogWarning(ex, "Failed to download SVG image {ImageId}", downloadItem.Id);
            }
        }
    }
}
