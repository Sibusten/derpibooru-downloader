using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Sibusten.Philomena.Client;
using Sibusten.Philomena.Client.Extensions;
using Sibusten.Philomena.Client.Images;
using Sibusten.Philomena.Downloader.Reporters;
using Sibusten.Philomena.Downloader.Settings;
using Sibusten.Philomena.Downloader.Utility;

namespace Sibusten.Philomena.Downloader
{
    public class ImageDownloader
    {
        // TODO: Make configurable
        public const int MaxDownloadThreads = 8;

        private BooruConfig _booruConfig;
        private SearchConfig _searchConfig;

        public ImageDownloader(BooruConfig booruConfig, SearchConfig searchConfig)
        {
            _booruConfig = booruConfig;
            _searchConfig = searchConfig;
        }

        public async Task StartDownload(CancellationToken cancellation = default, IImageDownloadReporter? downloadReporter = null)
        {
            IPhilomenaClient client = new PhilomenaClient(_booruConfig.BaseUrl)
            {
                ApiKey = _booruConfig.ApiKey
            };

            await client
                .GetImageSearch(_searchConfig.Query, o => o
                    .WithSortField(_searchConfig.SortField)
                    .WithSortDirection(_searchConfig.SortDirection)
                    .If(_searchConfig.ImageLimit != SearchConfig.NoLimit, o => o
                        .WithMaxImages(_searchConfig.ImageLimit)
                    )
                    .If(_searchConfig.Filter != SearchConfig.NoFilter, o => o
                        .WithFilterId(_searchConfig.Filter)
                    )
                )
                .CreateParallelDownloader(MaxDownloadThreads, o => o
                    .WithConditionalDownloader(image => !HasImageBeenDownloaded(image), o => o
                        .WithImageFileDownloader(GetFileForImage)
                    )
                    .WithConditionalDownloader(image => _searchConfig.ShouldUpdateJson || !HasImageBeenDownloaded(image), o => o
                        .WithImageMetadataFileDownloader(GetFileForImageMetadata)
                    )
                )
                .BeginDownload(cancellation, downloadReporter?.SearchProgress, downloadReporter?.SearchDownloadProgress, downloadReporter?.IndividualDownloadProgresses);
        }

        private string GetFileForImage(IPhilomenaImage image) => GetFileForPathFormat(image, _searchConfig.ImagePathFormat);
        private string GetFileForImageMetadata(IPhilomenaImage image) => GetFileForPathFormat(image, _searchConfig.JsonPathFormat);

        private string GetFileForPathFormat(IPhilomenaImage image, string filePath)
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

            List<string> imageRatingTags = image.TagNames?.Where(t => ratingTags.Contains(t)).ToList() ?? new List<string>();

            string ratingString = imageRatingTags.Any() ? string.Join("+", imageRatingTags) : "no_rating";

            // TODO: Some fields are not filename safe. Sanitize them first
            Dictionary<string, string?> tagReplacements = new Dictionary<string, string?>
            {
                { "id", image.Id.ToString() },
                { "name", image.Name },
                { "original_name", image.OriginalName },
                { "uploader", image.Uploader },
                { "ext", image.Format?.ToLower() },  // Will be png or svg, depending on which version is being downloaded
                { "year", image.CreatedAt?.Year.ToString() },
                { "month", image.CreatedAt?.Month.ToString("00") },
                { "day", image.CreatedAt?.Day.ToString("00") },
                { "score", image.Score?.ToString() },
                { "upvotes", image.Upvotes?.ToString() },
                { "downvotes", image.Downvotes?.ToString() },
                { "faves", image.Faves?.ToString() },
                { "comments", image.CommentCount?.ToString() },
                { "width", image.Width?.ToString() },
                { "height", image.Height?.ToString() },
                { "aspect_ratio", image.AspectRatio?.ToString() },
                { "rating", ratingString },
                { "booru_url", UrlUtilities.GetDomain(_booruConfig.BaseUrl) },
                { "booru_name", _booruConfig.Name },
            };

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

        /// <summary>
        /// Determines if an image has been downloaded
        /// </summary>
        /// <param name="image">The image</param>
        /// <returns>True if the image has been downloaded</returns>
        private bool HasImageBeenDownloaded(IPhilomenaImage image)
        {
            // TODO
            return false;
        }
    }
}
