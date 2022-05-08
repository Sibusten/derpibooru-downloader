using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;

namespace Sibusten.Philomena.Client.Utilities
{
    public static class UrlUtilities
    {
        public static async Task<Stream> GetProgressWrappedDownloadStream(string downloadUrl, IProgress<StreamProgressInfo> progress, CancellationToken cancellationToken)
        {
            IFlurlResponse response = await downloadUrl.GetAsync(cancellationToken, HttpCompletionOption.ResponseHeadersRead);

            // Attempt to read the length of the stream from the header
            long? length = null;
            if (response.Headers.TryGetFirst("Content-Length", out string lengthString))
            {
                if (long.TryParse(lengthString, out long parsedLength))
                {
                    length = parsedLength;
                }
            }

            // Open the image stream
            Stream downloadStream = await response.GetStreamAsync();

            // Create progress stream wrapper for reporting download progress
            return new StreamProgressReporter(downloadStream, progress, length);
        }
    }
}
