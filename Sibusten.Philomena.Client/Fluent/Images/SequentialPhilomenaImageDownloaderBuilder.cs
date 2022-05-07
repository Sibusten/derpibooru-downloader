using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sibusten.Philomena.Client.Images;
using Sibusten.Philomena.Client.Images.Downloaders;

namespace Sibusten.Philomena.Client.Fluent.Images
{
    public class SequentialPhilomenaImageDownloaderBuilder
    {
        private List<IPhilomenaImageDownloader> _downloaders;

        public SequentialPhilomenaImageDownloaderBuilder() : this(downloaders: Enumerable.Empty<IPhilomenaImageDownloader>()) { }

        private SequentialPhilomenaImageDownloaderBuilder(IEnumerable<IPhilomenaImageDownloader> downloaders)
        {
            _downloaders = downloaders.ToList();
        }

        /// <summary>
        /// Builds the downloader
        /// </summary>
        /// <returns>The downloader</returns>
        public SequentialPhilomenaImageDownloader Build()
        {
            return new SequentialPhilomenaImageDownloader(_downloaders);
        }

        /// <summary>
        /// Adds an image file downloader
        /// </summary>
        /// <param name="getFileForImage">A delegate to get the file for an image</param>
        public SequentialPhilomenaImageDownloaderBuilder WithImageFileDownloader(GetFileForImageDelegate getFileForImage)
        {
            return new
            (
                _downloaders.Append
                (
                    new PhilomenaImageFileDownloader(getFileForImage)
                )
            );
        }

        /// <summary>
        /// Adds an image SVG file downloader
        /// </summary>
        /// <param name="getFileForImage">A delegate to get the file for an SVG image</param>
        public SequentialPhilomenaImageDownloaderBuilder WithImageSvgFileDownloader(GetFileForImageDelegate getFileForImage)
        {
            return new
            (
                _downloaders.Append
                (
                    new PhilomenaImageSvgFileDownloader(getFileForImage)
                )
            );
        }

        /// <summary>
        /// Adds an image metadata file downloader
        /// </summary>
        /// <param name="getFileForImage">A delegate to get the file for an image</param>
        public SequentialPhilomenaImageDownloaderBuilder WithImageMetadataFileDownloader(GetFileForImageDelegate getFileForImage)
        {
            return new
            (
                _downloaders.Append
                (
                    new PhilomenaImageMetadataFileDownloader(getFileForImage)
                )
            );
        }

        /// <summary>
        /// Adds a conditional downloader
        /// </summary>
        /// <param name="shouldDownloadImage">A delegate that returns true if an image should be downloaded</param>
        /// <param name="buildInnerDownloader">Configures the inner downloader builder</param>
        public SequentialPhilomenaImageDownloaderBuilder WithConditionalDownloader(ShouldDownloadImageDelegate shouldDownloadImage, Func<SequentialPhilomenaImageDownloaderBuilder, SequentialPhilomenaImageDownloaderBuilder> buildInnerDownloader)
        {
            // Build the inner downloader
            SequentialPhilomenaImageDownloaderBuilder builder = new SequentialPhilomenaImageDownloaderBuilder();
            SequentialPhilomenaImageDownloader innerDownloader = buildInnerDownloader(builder).Build();

            // Wrap the downloader in a conditional downloader
            var conditionalDownloader = new ConditionalImageDownloader(shouldDownloadImage, innerDownloader);

            return new
            (
                _downloaders.Append(conditionalDownloader)
            );
        }
    }
}
