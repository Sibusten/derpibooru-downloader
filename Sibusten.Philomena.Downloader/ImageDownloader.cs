using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Sibusten.Philomena.Client;
using Sibusten.Philomena.Downloader.Settings;
using Sibusten.Philomena.Downloader.Utility;

namespace Sibusten.Philomena.Downloader
{
    public class ImageDownloader
    {
        // TODO: Make configurable
        private const int _maxDownloadThreads = 8;

        private BooruConfig _booruConfig;
        private SearchConfig _searchConfig;
        private IPhilomenaImageSearchQuery _search;

        public ImageDownloader(BooruConfig booruConfig, SearchConfig searchConfig)
        {
            _booruConfig = booruConfig;
            _searchConfig = searchConfig;

            IPhilomenaClient client = new PhilomenaClient(booruConfig.BaseUrl)
            {
                ApiKey = booruConfig.ApiKey
            };

            _search = client.Search(searchConfig.Query)
                .Limit(searchConfig.ImageLimit)
                .SortBy(searchConfig.SortField, searchConfig.SortDirection)
                .WithFilter(searchConfig.Filter)
                .WithMaxDownloadThreads(_maxDownloadThreads);
        }

        public async Task StartDownload(CancellationToken cancellation = default, IProgress<ImageDownloadProgressInfo>? progress = null)
        {
            await _search.DownloadAllToFilesAsync(GetFileForImage, SkipDownloadedImages, cancellation, progress);
        }

        private string GetFileForImage(IPhilomenaImage image)
        {
            List<string> ratingTags = new List<string>
            {
                "safe",
                "suggestive",
                "questionable",
                "explicit",
                "semi-grimdark",
                "grimdark",
                "grotesque",
            };

            List<string> imageRatingTags = image.Model.Tags?.Where(t => ratingTags.Contains(t)).ToList() ?? new List<string>();

            string ratingString = imageRatingTags.Any() ? string.Join("+", imageRatingTags) : "no_rating";

            // TODO: Some fields are not filename safe. Sanitize them first
            Dictionary<string, string?> tagReplacements = new Dictionary<string, string?>
            {
                { "id", image.Id.ToString() },
                { "name", image.Name },
                { "original_name", image.OriginalName },
                { "uploader", image.Model.Uploader },
                { "ext", image.Model.Format?.ToLower() },  // Will be png or svg, depending on which version is being downloaded
                { "year", image.Model.CreatedAt?.Year.ToString() },
                { "month", image.Model.CreatedAt?.Month.ToString("00") },
                { "day", image.Model.CreatedAt?.Day.ToString("00") },
                { "score", image.Model.Score?.ToString() },
                { "upvotes", image.Model.Upvotes?.ToString() },
                { "downvotes", image.Model.Downvotes?.ToString() },
                { "faves", image.Model.Faves?.ToString() },
                { "comments", image.Model.CommentCount?.ToString() },
                { "width", image.Model.Width?.ToString() },
                { "height", image.Model.Height?.ToString() },
                { "aspect_ratio", image.Model.AspectRatio?.ToString() },
                { "rating", ratingString },
                { "booru_url", UrlUtilities.GetDomain(_booruConfig.BaseUrl) },
                { "booru_name", _booruConfig.Name },
            };

            // Begin with the path format
            string filePath = _searchConfig.ImagePathFormat;

            // First, replace all instances of '#" tags
            // Ex: With {###}
            //  1234 => 1200
            //   234 =>  200
            //   100 =>  100
            //    45 =>    0
            filePath = Regex.Replace(filePath, @"{(#+)}", match =>
            {
                // The number of '#' determines the length of the partition
                int partitionLength = match.Groups[0].Value.Length;

                // Calculate the number to divide by
                int partitionDivisionValue = (int)Math.Pow(10, partitionLength - 1);

                // Use integer division to remove the remainder
                int partitionedNumber = (image.Id / partitionDivisionValue) * partitionDivisionValue;

                return partitionedNumber.ToString();
            });

            // Replace all other tags
            foreach ((string tag, string? replacement) in tagReplacements)
            {
                string tagWithBraces = "{" + tag + "}";

                filePath = filePath.Replace(tagWithBraces, replacement);
            }

            return filePath;
        }

        private bool SkipDownloadedImages(IPhilomenaImage image)
        {
            // TODO
            return true;
        }
    }
}
