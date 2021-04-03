using System;
using System.Collections.Generic;
using ShellProgressBar;
using Sibusten.Philomena.Client.Images;
using Sibusten.Philomena.Client.Images.Downloaders;
using Sibusten.Philomena.Client.Utilities;
using Sibusten.Philomena.Downloader.Reporters;

namespace Sibusten.Philomena.Downloader.Cmd.Reporters
{
    public class AdvancedConsoleReporter : IImageDownloadReporter
    {
        private const int _progressBarResolution = 10000;

        private readonly string _downloadMessage;
        private readonly ProgressBar _progressBar;
        private readonly List<ChildProgressBar> _individualDownloadProgressBars = new List<ChildProgressBar>();

        public IProgress<PhilomenaImageSearchProgressInfo> SearchProgress { get; }

        public IProgress<PhilomenaImageSearchDownloadProgressInfo> SearchDownloadProgress { get; }

        public IReadOnlyCollection<IProgress<PhilomenaImageDownloadProgressInfo>> IndividualDownloadProgresses { get; }

        public AdvancedConsoleReporter(int maxConcurrentDownloads, string downloadMessage)
        {
            _downloadMessage = downloadMessage;

            // Create progress bars
            _progressBar = new ProgressBar(_progressBarResolution, downloadMessage);
            for (int i = 0; i < maxConcurrentDownloads; i++)
            {
                _individualDownloadProgressBars.Add(_progressBar.Spawn(_progressBarResolution, ""));
            }

            // Set up progress report handlers
            SearchProgress = new SyncProgress<PhilomenaImageSearchProgressInfo>(OnSearchProgressReported);
            SearchDownloadProgress = new SyncProgress<PhilomenaImageSearchDownloadProgressInfo>(OnSearchDownloadProgressReported);
            List<IProgress<PhilomenaImageDownloadProgressInfo>> individualDownloadProgresses = new List<IProgress<PhilomenaImageDownloadProgressInfo>>();
            for (int i = 0; i < maxConcurrentDownloads; i++)
            {
                // Thread index must be copied to a local so it is not modified in closure
                int threadIndex = i;
                individualDownloadProgresses.Add
                (
                    new SyncProgress<PhilomenaImageDownloadProgressInfo>
                    (
                        // Pass the thread index to the progress report handler
                        progress => OnIndividualSearchDownloadProgressReported(threadIndex, progress)
                    )
                );
            }
            IndividualDownloadProgresses = individualDownloadProgresses;
        }

        private int GetProgressBarTicks(long progress, long total)
        {
            decimal percent = (decimal)progress / total;
            return (int)(_progressBarResolution * percent);
        }

        private void OnSearchProgressReported(PhilomenaImageSearchProgressInfo progress)
        {

        }

        private void OnSearchDownloadProgressReported(PhilomenaImageSearchDownloadProgressInfo progress)
        {
            string message = $"{progress.ImagesDownloaded}/{progress.ImagesTotal} - {_downloadMessage}";
            _progressBar.Tick(GetProgressBarTicks(progress.ImagesDownloaded, progress.ImagesTotal), message);
        }

        private void OnIndividualSearchDownloadProgressReported(int threadIndex, PhilomenaImageDownloadProgressInfo progress)
        {
            if (progress.Total is null)
            {
                // No progress can be reported
                return;
            }

            string message = $"Downloading {progress.Action}";
            _individualDownloadProgressBars[threadIndex].Tick(GetProgressBarTicks(progress.Current, progress.Total.Value), message);
        }

        public void Dispose()
        {
            _progressBar.Dispose();
        }
    }
}
