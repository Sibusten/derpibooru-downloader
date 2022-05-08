using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sibusten.Philomena.Client.Logging;
using Sibusten.Philomena.Client.Utilities;

namespace Sibusten.Philomena.Client.Images.Downloaders
{
    public class PhilomenaImageMetadataFileDownloader : IPhilomenaImageDownloader
    {
        private ILogger _logger;

        private readonly GetFileForImageDelegate _getFileForImage;

        public PhilomenaImageMetadataFileDownloader(GetFileForImageDelegate getFileForImage)
        {
            _logger = Logger.Factory.CreateLogger(GetType());

            _getFileForImage = getFileForImage;
        }

        public async Task Download(IPhilomenaImage downloadItem, CancellationToken cancellationToken = default, IProgress<PhilomenaImageDownloadProgressInfo>? progress = null)
        {
            try
            {
                string file = _getFileForImage(downloadItem);

                FileUtilities.CreateDirectoryForFile(file);

                // Metadata is already downloaded, so just report 0 or 1 for progress
                void reportProgress(bool isFinished)
                {
                    progress?.Report(new PhilomenaImageDownloadProgressInfo
                    {
                        Current = isFinished ? 1 : 0,
                        Total = 1,
                        Action = $"Downloading image {downloadItem.Id} Metadata",
                    });
                }

                reportProgress(isFinished: false);

                _logger.LogDebug("Saving image {ImageId} metadata to {File}", downloadItem.Id, file);

                await FileUtilities.SafeFileWrite(file, async tempFile =>
                {
                    await File.WriteAllTextAsync(tempFile, downloadItem.RawMetadata, Encoding.UTF8, cancellationToken);
                });

                reportProgress(isFinished: true);
            }
            catch (IOException ex)
            {
                _logger.LogWarning(ex, "Failed to download image {ImageId} metadata", downloadItem.Id);
            }
        }
    }
}
