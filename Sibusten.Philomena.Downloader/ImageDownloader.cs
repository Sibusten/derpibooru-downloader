using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Sibusten.Philomena.Client;
using Sibusten.Philomena.Client.Images;
using Sibusten.Philomena.Client.Images.Downloaders;
using Sibusten.Philomena.Downloader.Reporters;
using Sibusten.Philomena.Downloader.Settings;
using Sibusten.Philomena.Downloader.Utility;

namespace Sibusten.Philomena.Downloader
{
    public class ImageDownloader : IPhilomenaImageDownloader
    {
        // TODO: Make configurable
        public const int MaxDownloadThreads = 8;

        private SearchConfig _searchConfig;

        public ImageDownloader(SearchConfig searchConfig)
        {
            _searchConfig = searchConfig;
        }

        public async Task Download(IPhilomenaImage image, CancellationToken cancellationToken = default, IProgress<PhilomenaImageDownloadProgressInfo>? progress = null)
        {
            if (HasImageBeenDownloaded(image))
            {
                return;
            }

            bool downloadImage;
            bool downloadSvg;

            if (image.IsSvgImage)
            {
                switch (_searchConfig.SvgMode)
                {
                    case SvgMode.RasterOnly:
                        downloadImage = true;
                        downloadSvg = false;
                        break;

                    case SvgMode.SvgOnly:
                        downloadImage = false;
                        downloadSvg = true;
                        break;

                    case SvgMode.Both:
                    default:
                        downloadImage = true;
                        downloadSvg = true;
                        break;
                }
            }
            else
            {
                downloadImage = true;
                downloadSvg = false;
            }

            if (downloadImage)
            {
                await new PhilomenaImageFileDownloader(GetFileForImage).Download(image, cancellationToken, progress);
            }

            if (downloadSvg)
            {
                await new PhilomenaImageSvgFileDownloader(GetFileForSvgImage).Download(image, cancellationToken, progress);
            }

            if (_searchConfig.JsonPathFormat is not null)
            {
                await new PhilomenaImageMetadataFileDownloader(GetFileForImageMetadata).Download(image, cancellationToken, progress);
            }
        }

        private string GetFileForImage(IPhilomenaImage image) => GetFileForPathFormat(image, _searchConfig.ImagePathFormat);
        private string GetFileForSvgImage(IPhilomenaImage image) => GetFileForPathFormat(image, _searchConfig.ImagePathFormat, isSvgImage: true);
        private string GetFileForImageMetadata(IPhilomenaImage image) => GetFileForPathFormat(image, _searchConfig.JsonPathFormat!);

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
