using System;
using System.Collections.Generic;
using Sibusten.Philomena.Client;
using Sibusten.Philomena.Client.Images;
using Sibusten.Philomena.Client.Images.Downloaders;
using Sibusten.Philomena.Client.Utilities;
using Sibusten.Philomena.Downloader.Reporters;

namespace Sibusten.Philomena.Downloader.Cmd.Reporters
{
    public class SimpleConsoleReporter : IImageDownloadReporter
    {
        /// <summary>
        /// Search progress is not supported by this reporter
        /// </summary>
        public IProgress<PhilomenaImageSearchProgressInfo>? SearchProgress { get; } = null;

        public IProgress<PhilomenaImageSearchDownloadProgressInfo> SearchDownloadProgress { get; }

        /// <summary>
        /// Individual download progress is not supported by this reporter
        /// </summary>
        public IReadOnlyCollection<IProgress<PhilomenaImageDownloadProgressInfo>>? IndividualDownloadProgresses { get; } = null;

        public SimpleConsoleReporter()
        {
            SearchDownloadProgress = new SyncProgress<PhilomenaImageSearchDownloadProgressInfo>(OnSearchDownloadProgressReported);
        }

        public void OnSearchDownloadProgressReported(PhilomenaImageSearchDownloadProgressInfo progress)
        {
            double downloadPercent = (double)progress.ImagesDownloaded / progress.ImagesTotal;
            Console.WriteLine($"{downloadPercent:P} {progress.ImagesDownloaded}/{progress.ImagesTotal}");
        }
    }
}
