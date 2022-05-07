using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sibusten.Philomena.Client.Logging;

namespace Sibusten.Philomena.Client.Images.Downloaders
{
    public class PhilomenaImageMetadataFileDownloader : PhilomenaImageDownloader
    {
        private ILogger _logger;

        private const string _tempExtension = "tmp";

        private readonly GetFileForImageDelegate _getFileForImage;

        public PhilomenaImageMetadataFileDownloader(GetFileForImageDelegate getFileForImage)
        {
            _logger = Logger.Factory.CreateLogger(GetType());

            _getFileForImage = getFileForImage;
        }

        public override async Task Download(IPhilomenaImage downloadItem, CancellationToken cancellationToken = default, IProgress<PhilomenaImageDownloadProgressInfo>? progress = null)
        {
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

                // Write to a temp file first
                string tempFile = file + "." + _tempExtension;
                await File.WriteAllTextAsync(tempFile, downloadItem.RawMetadata, Encoding.UTF8, cancellationToken);

                // Move the temp file to the destination file
                File.Move(tempFile, file, overwrite: true);

                reportProgress(isFinished: true);
            }
            catch (IOException ex)
            {
                _logger.LogWarning(ex, "Failed to download image {ImageId} metadata", downloadItem.Id);
            }
        }
    }
}
