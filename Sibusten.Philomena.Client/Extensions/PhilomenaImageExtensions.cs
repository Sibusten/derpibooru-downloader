using System;
using System.Threading;
using System.Threading.Tasks;
using Sibusten.Philomena.Client.Images;
using Sibusten.Philomena.Client.Images.Downloaders;

namespace Sibusten.Philomena.Client.Extensions
{
    public static class PhilomenaImageExtensions
    {
        public static async Task DownloadToFile(this IPhilomenaImage image, string filename, CancellationToken cancellationToken = default, IProgress<PhilomenaImageDownloadProgressInfo>? progress = null)
        {
            PhilomenaImageFileDownloader downloader = new PhilomenaImageFileDownloader(image => filename);
            await downloader.Download(image, cancellationToken, progress);
        }
    }
}
