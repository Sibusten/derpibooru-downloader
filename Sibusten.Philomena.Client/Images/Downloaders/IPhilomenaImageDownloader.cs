using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sibusten.Philomena.Client.Images.Downloaders
{
    public interface IPhilomenaImageDownloader
    {
        Task Download(IPhilomenaImage image, CancellationToken cancellationToken = default, IProgress<PhilomenaImageDownloadProgressInfo>? progress = null);
    }
}
