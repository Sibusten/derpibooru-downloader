using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class ImageModel
    {
        /// <summary>
        /// The creation time, in UTC, of the image.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// The file extension of the image. One of "gif", "jpg", "jpeg", "png", "svg", "webm".
        /// </summary>
        [JsonProperty("format")]
        public string? Format { get; set; }
        /// <summary>
        /// The image's ID.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

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
        /// A mapping of representation names to their respective URLs. Contains the keys "full", "large", "medium", "small", "tall", "thumb", "thumb_small", "thumb_tiny".
        /// </summary>
        [JsonProperty("representations")]
        public RepresentationsModel Representations { get; set; } = default!;

        /// <summary>
        /// The SHA512 hash of this image after it has been processed.
        /// </summary>
        [JsonProperty("sha512_hash")]
        public string? Sha512Hash { get; set; }

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
        /// The image's uploader.
        /// </summary>
        [JsonProperty("uploader")]
        public string? Uploader { get; set; }

        /// <summary>
        /// The image's view URL, including tags.
        /// </summary>
        [JsonProperty("view_url")]
        public string? ViewUrl { get; set; }
    }

    public class RepresentationsModel
    {
        [JsonProperty("full")]
        public string Full { get; set; } = default!;
    }
}
