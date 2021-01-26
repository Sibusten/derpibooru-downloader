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
    }
}
