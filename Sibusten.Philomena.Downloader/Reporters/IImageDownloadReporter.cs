using System;
using System.Collections.Generic;
using Sibusten.Philomena.Client.Images;
using Sibusten.Philomena.Client.Images.Downloaders;

namespace Sibusten.Philomena.Downloader.Reporters
{
    public interface IImageDownloadReporter
    {
        /// <summary>
        /// A reporter for the progress of the image search
        /// </summary>
        /// <value>The search progress reporter, or null if not supported</value>
        IProgress<PhilomenaImageSearchProgressInfo>? SearchProgress { get; }

        /// <summary>
        /// A reporter for the progress of the image search download
        /// </summary>
        /// <value>The search download progress reporter, or null if not supported</value>
        IProgress<PhilomenaImageSearchDownloadProgressInfo>? SearchDownloadProgress { get; }

        /// <summary>
        /// A reporter for the progress of individual downloads in the parallel download
        /// </summary>
        /// <value>The individual download progress reporters, or null if not supported</value>
        IReadOnlyCollection<IProgress<PhilomenaImageDownloadProgressInfo>>? IndividualDownloadProgresses { get; }
    }
}
