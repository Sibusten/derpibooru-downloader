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
        int? FileSize { get; }
        string? SourceUrl { get; }
        bool? IsSpoilered { get; }
        int? TagCount { get; }
        bool? ThumbnailsGenerated { get; }
        DateTime? UpdatedAt { get; }
        string? Uploader { get; }
        int? UploaderId { get; }
        int? Upvotes { get; }
        string? ViewUrl { get; }
        string? SvgViewUrl { get; }
        string? ShortViewUrl { get; }
        string? ShortSvgViewUrl { get; }
        bool? Processed { get; }
        string? MimeType { get; }
        bool? IsAnimated { get; }
        double? AspectRatio { get; }
        int? CommentCount { get; }
        DateTime? CreatedAt { get; }
        string? DeletionReason { get; }
        string? Description { get; }
        int? Downvotes { get; }
        int? Width { get; }
        int? DuplicateOf { get; }
        int? Faves { get; }
        DateTime? FirstSeenAt { get; }
        int? Height { get; }
        bool? IsHiddenFromUsers { get; }
        double? Duration { get; }
        double? WilsonScore { get; }
    }
}
