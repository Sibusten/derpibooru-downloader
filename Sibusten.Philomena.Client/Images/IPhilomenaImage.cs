using System;
using System.Collections.Generic;

namespace Sibusten.Philomena.Client.Images
{
    public interface IPhilomenaImage
    {
        /// <summary>
        /// True if this is an SVG image (one which has both an SVG download and a raster download)
        /// </summary>
        bool IsSvgImage { get; }

        /// <summary>
        /// Raw metadata received from the API
        /// </summary>
        string RawMetadata { get; }

        int Id { get; }
        string? Name { get; }
        string? OriginalName { get; }
        string? Format { get; }
        string? Hash { get; }
        string? OriginalHash { get; }
        List<string> TagNames { get; }
        List<int> TagIds { get; }
        int? Score { get; }
        bool? ThumbnailsGenerated { get; }
        string? Uploader { get; }
        int? Upvotes { get; }
        string? ShortViewUrl { get; }
        string? ShortSvgViewUrl { get; }
        double? AspectRatio { get; }
        int? CommentCount { get; }
        DateTime? CreatedAt { get; }
        int? Downvotes { get; }
        int? Width { get; }
        int? Faves { get; }
        int? Height { get; }
    }
}
