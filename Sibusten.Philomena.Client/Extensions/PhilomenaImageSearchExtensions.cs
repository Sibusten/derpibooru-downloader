using System;
using System.Collections.Generic;
using Sibusten.Philomena.Client.Fluent.Images;
using Sibusten.Philomena.Client.Images;
using Sibusten.Philomena.Client.Images.Downloaders;
using Sibusten.Philomena.Client.Options;

namespace Sibusten.Philomena.Client.Extensions
{
    public static class PhilomenaImageSearchExtensions
    {
        /// <summary>
        /// Creates a parallel downloader for a search query
        /// </summary>
        /// <param name="imageSearch">The image search to download</param>
        /// <param name="maxDownloadThreads">The max number of threads to use when downloading</param>
        /// <param name="imageDownloader">The downloader to use</param>
        /// <returns>A parallel downloader for the search query</returns>
        public static ParallelPhilomenaImageSearchDownloader CreateParallelDownloader(this IPhilomenaImageSearch imageSearch, int maxDownloadThreads, IPhilomenaImageDownloader imageDownloader)
        {
            return new ParallelPhilomenaImageSearchDownloader(imageSearch, imageDownloader, maxDownloadThreads);
        }

        /// <summary>
        /// Creates a parallel downloader for a search query
        /// </summary>
        /// <param name="imageSearch">The image search to download</param>
        /// <param name="maxDownloadThreads">The max number of threads to use when downloading</param>
        /// <param name="buildImageDownloader">Configures a sequential image downloader builder</param>
        /// <returns>A parallel downloader for the search query</returns>
        public static ParallelPhilomenaImageSearchDownloader CreateParallelDownloader(this IPhilomenaImageSearch imageSearch, int maxDownloadThreads, Func<SequentialPhilomenaImageDownloaderBuilder, SequentialPhilomenaImageDownloaderBuilder> buildImageDownloader)
        {
            SequentialPhilomenaImageDownloaderBuilder builder = new SequentialPhilomenaImageDownloaderBuilder();
            SequentialPhilomenaImageDownloader imageDownloader = buildImageDownloader(builder).Build();
            return new ParallelPhilomenaImageSearchDownloader(imageSearch, imageDownloader, maxDownloadThreads);
        }
    }
}
