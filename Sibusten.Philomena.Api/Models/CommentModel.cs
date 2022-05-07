using System;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class CommentModel
    {
        /// <summary>
        /// The comment's author.
        /// </summary>
        [JsonProperty("author")]
        public string? Author { get; set; }

        /// <summary>
        /// The URL of the author's avatar. May be a link to the CDN path, or a data: URI.
        /// </summary>
        [JsonProperty("avatar")]
        public string? Avatar { get; set; }

        /// <summary>
        /// The comment text.
        /// </summary>
        [JsonProperty("body")]
        public string? Body { get; set; }

        /// <summary>
        /// The creation time, in UTC, of the comment.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// The edit reason for this comment, or null if none provided.
        /// </summary>
        [JsonProperty("edit_reason")]
        public string? EditReason { get; set; }

        /// <summary>
        /// The time, in UTC, this comment was last edited at, or null if it was not edited.
        /// </summary>
        [JsonProperty("edited_at")]
        public DateTime? EditedAt { get; set; }

        /// <summary>
        /// The comment's ID.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// The ID of the image the comment belongs to.
        /// </summary>
        [JsonProperty("image_id")]
        public int? ImageId { get; set; }

        /// <summary>
        /// The time, in UTC, the comment was last updated at.
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The ID of the user the comment belongs to, if any.
        /// </summary>
        [JsonProperty("user_id")]
        public int? UserId { get; set; }
    }
}
