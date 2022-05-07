using System;
using System.Collections.Generic;
using System.Threading;

namespace Sibusten.Philomena.Client.Images
{
    public record PhilomenaImageSearchProgressInfo
    {
        public int MetadataDownloaded { get; init; }
        public int TotalImages { get; init; }
    }

    public interface IPhilomenaImageSearch
    {
        IAsyncEnumerable<IPhilomenaImage> BeginSearch(CancellationToken cancellationToken = default, IProgress<PhilomenaImageSearchProgressInfo>? searchProgress = null);
    }
}
