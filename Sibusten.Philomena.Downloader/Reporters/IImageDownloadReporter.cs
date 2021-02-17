using Sibusten.Philomena.Client;

namespace Sibusten.Philomena.Downloader.Reporters
{
    public interface IImageDownloadReporter
    {
        void ReportProgress(ImageDownloadProgressInfo progress);
    }
}
