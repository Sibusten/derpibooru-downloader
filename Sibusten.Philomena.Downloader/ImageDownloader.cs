using System;
using System.Collections.Generic;
using System.IO;
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

        private Uri _booruBaseUri;
        private SearchConfig _searchConfig;

        public ImageDownloader(Uri booruBaseUri, SearchConfig searchConfig)
        {
            _booruBaseUri = booruBaseUri;
            _searchConfig = searchConfig;
        }

        public async Task StartDownload(CancellationToken cancellation = default, IImageDownloadReporter? downloadReporter = null)
        {
            IPhilomenaClient client = new PhilomenaClient(_booruBaseUri.ToString());

            await client
                .GetImageSearch(_searchConfig.Query, o => o
                    .If(_searchConfig.ImageLimit != SearchConfig.NoLimit, o => o
                        .WithMaxImages(_searchConfig.ImageLimit)
                    )
                    .If(_searchConfig.Filter != SearchConfig.NoFilter, o => o
                        .WithFilterId(_searchConfig.Filter)
                    )
                )
                .CreateParallelDownloader(MaxDownloadThreads, o => o
                    // If image saving is enabled
                    .If(_searchConfig.ShouldSaveImages, o => o
                        // Only download images that have not been downloaded already
                        .WithConditionalDownloader(image => !HasImageBeenDownloaded(image), o => o
                            // Svg images require special download logic
                            .WithConditionalDownloader(image => image.IsSvgImage, o => o
                                // Download raster versions of the SVG image if set to do so
                                .If(_searchConfig.SvgMode is SvgMode.RasterOnly or SvgMode.Both, o => o
                                    .WithImageFileDownloader(GetFileForImage)
                                )
                                // Download SVG versions of the SVG image if set to do so
                                .If(_searchConfig.SvgMode is SvgMode.SvgOnly or SvgMode.Both, o => o
                                    .WithImageSvgFileDownloader(GetFileForSvgImage)
                                )
                            )
                            // Non-svg images are downloaded normally
                            .WithConditionalDownloader(image => !image.IsSvgImage, o => o
                                .WithImageFileDownloader(GetFileForImage)
                            )
                        )
                    )
                    // If JSON saving is enabled
                    .If(_searchConfig.ShouldSaveJson, o => o
                        // Only save JSON if the image has not been downloaded already, or if set to always update
                        .WithConditionalDownloader(image => _searchConfig.ShouldUpdateJson || !HasImageBeenDownloaded(image), o => o
                            .WithImageMetadataFileDownloader(GetFileForImageMetadata)
                        )
                    )
                )
                .BeginDownload(cancellation, downloadReporter?.SearchProgress, downloadReporter?.SearchDownloadProgress, downloadReporter?.IndividualDownloadProgresses);
        }

        private string GetFileForImage(IPhilomenaImage image) => GetFileForPathFormat(image, _searchConfig.ImagePathFormat);
        private string GetFileForSvgImage(IPhilomenaImage image) => GetFileForPathFormat(image, _searchConfig.ImagePathFormat, isSvgImage: true);
        private string GetFileForImageMetadata(IPhilomenaImage image) => GetFileForPathFormat(image, _searchConfig.JsonPathFormat);

        private string GetFileForPathFormat(IPhilomenaImage image, string filePath, bool isSvgImage = false)
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
                { "ext", isSvgImage ? "svg" : image.Format?.ToLower() },  // Override image format for SVG images
                { "year", image.CreatedAt?.Year.ToString() },
                { "month", image.CreatedAt?.Month.ToString("00") },
                { "day", image.CreatedAt?.Day.ToString("00") },
                { "rating", ratingString },
            };

            // Sanitize replacement tag values
            foreach ((string key, string? tag) in tagReplacements)
            {
                if (tag is not null)
                {
                    tagReplacements[key] = SanitizeFilename(tag);
                }
            }

            // First, replace all instances of "#" tags
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
        /// Sanitize a string for use in a filename
        /// </summary>
        /// <param name="filename">The string to sanitize</param>
        /// <returns>The sanitized string</returns>
        private string SanitizeFilename(string filename)
        {
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(invalidChar, '_');
            }

            return filename;
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
