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

                _logger.LogDebug("Saving image {ImageId} metadata to {File}", downloadItem.Id, file);

                await FileUtilities.SafeFileWrite(file, async tempFile =>
                {
                    await File.WriteAllTextAsync(tempFile, downloadItem.RawMetadata, Encoding.UTF8, cancellationToken);
                });
            }
            catch (IOException ex)
            {
                _logger.LogWarning(ex, "Failed to download image {ImageId} metadata", downloadItem.Id);
            }
        }
    }
}
