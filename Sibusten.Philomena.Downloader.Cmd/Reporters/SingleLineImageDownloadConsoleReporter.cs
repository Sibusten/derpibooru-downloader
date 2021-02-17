using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sibusten.Philomena.Client;
using Sibusten.Philomena.Downloader.Reporters;

namespace Sibusten.Philomena.Downloader.Cmd.Reporters
{
    public class SingleLineImageDownloadConsoleReporter : IImageDownloadReporter
    {
        public string GetProgressText(ImageDownloadProgressInfo progress)
        {
            int width = Console.WindowWidth;

            double totalPercent = (double)progress.ImagesDownloaded / progress.TotalImages * 100;

            // Report overall progress
            // Ex: ' 45.6% 12(50)/100'
            string progressText = $"{totalPercent,5:##0.0}% {progress.ImagesDownloaded}({progress.MetadataDownloaded})/{progress.TotalImages}";

            // Report progress for each parallel download
            IEnumerable<string> parallelDownloadProgress = progress.Downloads.Select(p =>
            {
                const int idPadding = 7;
                const int percentPadding = 5;
                const int parallelDownloadProgressWidth = idPadding + 1 + percentPadding + 1;  // {ID}:{PERCENT}%

                // Use a blank space if no image is downloading
                if (p.ImageId == PhilomenaImageSearchQuery.NoImageDownloading)
                {
                    return new string(' ', parallelDownloadProgressWidth);
                }

                // Calculate percent, or use 0 if progress is unknown
                double downloadPercent = (p.BytesTotal is null or 0) ? 0 : (double)p.BytesDownloaded / p.BytesTotal.Value * 100;

                // Ex: '  12345: 45.6%'
                return $"{p.ImageId,idPadding}:{downloadPercent,percentPadding:##0.0}%";
            });
            string parallelDownloadProgressText = string.Join('|', parallelDownloadProgress);
            progressText += $" - {parallelDownloadProgressText}";

            // Truncate text if it is too long
            if (progressText.Length > width)
            {
                string ellipsis = "...";
                progressText = progressText.Substring(0, width - ellipsis.Length) + ellipsis;
            }

            // Pad the string to fill the whole console
            progressText = progressText.PadRight(width, ' ');

            return progressText;
        }

        public void ReportProgress(ImageDownloadProgressInfo progress)
        {
            string progressText = GetProgressText(progress);
            Console.Write("\r" + progressText);
        }
    }
}
