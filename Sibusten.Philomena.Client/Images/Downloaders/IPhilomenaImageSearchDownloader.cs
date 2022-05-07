using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sibusten.Philomena.Client.Images.Downloaders
{
    public record PhilomenaImageSearchDownloadProgressInfo
    {
        public int ImagesDownloaded { get; init; }
        public int ImagesTotal { get; init; }
    }

    public interface IPhilomenaImageSearchDownloader
    {
        /// <summary>
        /// Begins the image search download
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <param name="searchProgress">The progress of the image search</param>
        /// <param name="searchDownloadProgress">The progress of the image search download</param>
        /// <param name="individualDownloadProgresses">The progress of individual images being downloaded. There must be one progress reporter given for each download slot</param>
        Task BeginDownload(
            CancellationToken cancellationToken = default,
            IProgress<PhilomenaImageSearchProgressInfo>? searchProgress = null,
            IProgress<PhilomenaImageSearchDownloadProgressInfo>? searchDownloadProgress = null,
            IReadOnlyCollection<IProgress<PhilomenaImageDownloadProgressInfo>>? individualDownloadProgresses = null);
    }
}
