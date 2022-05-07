using System;

namespace Sibusten.Philomena.Downloader.Utility
{
    public static class UrlUtilities
    {
        public static Uri GetWellFormedWebUri(string rawUri)
        {
            // If the URL is valid, use it
            if (Uri.TryCreate(rawUri, UriKind.Absolute, out Uri? uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return uri;
            }

            // If the URL was not valid, try adding the https scheme
            string valueWithHttps = $"https://{rawUri}";
            if (Uri.TryCreate(valueWithHttps, UriKind.Absolute, out uri))
            {
                return uri;
            }

            throw new FormatException($"Could not parse URL '{rawUri}'");
        }
    }
}
