using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sibusten.Philomena.Downloader.Utility
{
    public static class UrlUtilities
    {
        private static string _domainNameRegex = @"https?:\/\/(.+?)(?:\/|$)";

        public static string? GetDomain(string url)
        {
            Match match = Regex.Match(url, _domainNameRegex);

            // Return the first capture group (domain name), or null if it doesn't exist
            return match.Groups.Cast<Group>().ElementAtOrDefault(1)?.Value;
        }

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
