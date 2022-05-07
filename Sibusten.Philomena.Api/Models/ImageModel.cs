using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class ImageModel
    {
        /// <summary>
        /// Whether the image is animated.
        /// </summary>
        [JsonProperty("animated")]
        public bool? IsAnimated { get; set; }

        /// <summary>
        /// The image's width divided by its height.
        /// </summary>
        [JsonProperty("aspect_ratio")]
        public double? AspectRatio { get; set; }

        /// <summary>
        /// The number of comments made on the image.
        /// </summary>
        [JsonProperty("comment_count")]
        public int? CommentCount { get; set; }

        /// <summary>
        /// The creation time, in UTC, of the image.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// The hide reason for the image, or null if none provided. This will only have a value on images which are deleted for a rule violation.
        /// </summary>
        [JsonProperty("deletion_reason")]
        public string? DeletionReason { get; set; }

        /// <summary>
        /// The image's description.
        /// </summary>
        [JsonProperty("description")]
        public string? Description { get; set; }

        /// <summary>
        /// The number of downvotes the image has.
        /// </summary>
        [JsonProperty("downvotes")]
        public int? Downvotes { get; set; }

        /// <summary>
        /// The ID of the target image, or null if none provided. This will only have a value on images which are merged into another image.
        /// </summary>
        [JsonProperty("duplicate_of")]
        public int? DuplicateOf { get; set; }

        /// <summary>
        /// The number of seconds the image lasts, if animated, otherwise .04.
        /// </summary>
        [JsonProperty("duration")]
        public double? Duration { get; set; }

        /// <summary>
        /// The number of faves the image has.
        /// </summary>
        [JsonProperty("faves")]
        public int? Faves { get; set; }

        /// <summary>
        /// The time, in UTC, the image was first seen (before any duplicate merging).
        /// </summary>
        [JsonProperty("first_seen_at")]
        public DateTime? FirstSeenAt { get; set; }

        /// <summary>
        /// The file extension of the image. One of "gif", "jpg", "jpeg", "png", "svg", "webm".
        /// </summary>
        [JsonProperty("format")]
        public string? Format { get; set; }

        /// <summary>
        /// The image's height, in pixels.
        /// </summary>
        [JsonProperty("height")]
        public int? Height { get; set; }

        /// <summary>
        /// Whether the image is hidden. An image is hidden if it is merged or deleted for a rule violation.
        /// </summary>
        [JsonProperty("hidden_from_users")]
        public bool? IsHiddenFromUsers { get; set; }

        /// <summary>
        /// The image's ID.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Optional object of internal image intensity data for deduplication purposes. May be null if intensities have not yet been generated.
        /// </summary>
        [JsonProperty("intensities")]
        public IntensitiesModel? Intensities { get; set; }

        /// <summary>
        /// The MIME type of this image. One of "image/gif", "image/jpeg", "image/png", "image/svg+xml", "video/webm".
        /// </summary>
        [JsonProperty("mime_type")]
        public string? MimeType { get; set; }

        /// <summary>
        /// The filename that the image was uploaded with.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// The SHA512 hash of the image as it was originally uploaded.
        /// </summary>
        [JsonProperty("orig_sha512_hash")]
        public string? OrigSha512Hash { get; set; }

        /// <summary>
        /// Whether the image has finished optimization.
        /// </summary>
        [JsonProperty("processed")]
        public bool? Processed { get; set; }

        /// <summary>
        /// A mapping of representation names to their respective URLs. Contains the keys "full", "large", "medium", "small", "tall", "thumb", "thumb_small", "thumb_tiny".
        /// </summary>
        [JsonProperty("representations")]
        public RepresentationsModel? Representations { get; set; }

        /// <summary>
        /// The image's number of upvotes minus the image's number of downvotes.
        /// </summary>
        [JsonProperty("score")]
        public int? Score { get; set; }

        /// <summary>
        /// The SHA512 hash of this image after it has been processed.
        /// </summary>
        [JsonProperty("sha512_hash")]
        public string? Sha512Hash { get; set; }

        /// <summary>
        /// The number of bytes the image's file contains.
        /// </summary>
        [JsonProperty("size")]
        public int? Size { get; set; }

        /// <summary>
        /// The current source URL of the image.
        /// </summary>
        [JsonProperty("source_url")]
        public string? SourceUrl { get; set; }

        /// <summary>
        /// Whether the image is hit by the current filter.
        /// </summary>
        [JsonProperty("spoilered")]
        public bool? IsSpoilered { get; set; }

        /// <summary>
        /// The number of tags present on the image.
        /// </summary>
        [JsonProperty("tag_count")]
        public int? TagCount { get; set; }

        /// <summary>
        /// A list of tag IDs the image contains.
        /// </summary>
        [JsonProperty("tag_ids")]
        public List<int>? TagIds { get; set; }

        /// <summary>
        /// A list of tag names the image contains.
        /// </summary>
        [JsonProperty("tags")]
        public List<string>? Tags { get; set; }

        /// <summary>
        /// Whether the image has finished thumbnail generation. Do not attempt to load images from view_url or representations if this is false.
        /// </summary>
        [JsonProperty("thumbnails_generated")]
        public bool? ThumbnailsGenerated { get; set; }

        /// <summary>
        /// The time, in UTC, the image was last updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The image's uploader.
        /// </summary>
        [JsonProperty("uploader")]
        public string? Uploader { get; set; }

        /// <summary>
        /// The ID of the image's uploader. null if uploaded anonymously.
        /// </summary>
        [JsonProperty("uploader_id")]
        public int? UploaderId { get; set; }

        /// <summary>
        /// The image's number of upvotes.
        /// </summary>
        [JsonProperty("upvotes")]
        public int? Upvotes { get; set; }

        /// <summary>
        /// The image's view URL, including tags.
        /// </summary>
        [JsonProperty("view_url")]
        public string? ViewUrl { get; set; }

        /// <summary>
        /// The image's width, in pixels.
        /// </summary>
        [JsonProperty("width")]
        public int? Width { get; set; }

        /// <summary>
        /// The lower bound of the Wilson score interval for the image, based on its upvotes and downvotes, given a z-score corresponding to a confidence of 99.5%.
        /// </summary>
        [JsonProperty("wilson_score")]
        public double? WilsonScore { get; set; }
    }

    public class IntensitiesModel
    {
        [JsonProperty("ne")]
        public double? NorthEast { get; set; }
        [JsonProperty("nw")]
        public double? NorthWest { get; set; }
        [JsonProperty("se")]
        public double? SouthEast { get; set; }
        [JsonProperty("sw")]
        public double? SouthWest { get; set; }
    }

    public class RepresentationsModel
    {
        [JsonProperty("full")]
        public string? Full { get; set; }
        [JsonProperty("large")]
        public string? Large { get; set; }
        [JsonProperty("medium")]
        public string? Medium { get; set; }
        [JsonProperty("small")]
        public string? Small { get; set; }
        [JsonProperty("tall")]
        public string? Tall { get; set; }
        [JsonProperty("thumb")]
        public string? Thumb { get; set; }
        [JsonProperty("thumb_small")]
        public string? ThumbSmall { get; set; }
        [JsonProperty("thumb_tiny")]
        public string? ThumbTiny { get; set; }
    }

    public class ImageResponseModel
    {
        [JsonProperty("image")]
        public ImageModel? Image { get; set; }

        [JsonProperty("interactions")]
        public List<InteractionModel>? Interactions { get; set; }
    }
}
