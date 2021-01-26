using System.Linq;
using System.Text.RegularExpressions;

namespace Sibusten.Philomena.Downloader.Utility
{
    public static class UrlUtilities
    {
        private static string _domainNameRegex = @"https?:\/\/(.+?)(?:\/|$)";

        public static string? GetDomain(string url) => Regex.Match(url, _domainNameRegex).Captures.FirstOrDefault()?.Value;
    }
}
