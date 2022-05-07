using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sibusten.Philomena.Client.Images.Downloaders
{
    public class SequentialPhilomenaImageDownloader : IPhilomenaImageDownloader
    {
        public IReadOnlyList<IPhilomenaImageDownloader> Downloaders { get; }

        public SequentialPhilomenaImageDownloader(IEnumerable<IPhilomenaImageDownloader> downloaders)
        {
            Downloaders = downloaders.ToList();
        }

        public async Task Download(IPhilomenaImage image, CancellationToken cancellationToken = default, IProgress<PhilomenaImageDownloadProgressInfo>? progress = null)
        {
            foreach (IPhilomenaImageDownloader downloader in Downloaders)
            {
                await downloader.Download(image, cancellationToken, progress);
            }
        }
    }
}
