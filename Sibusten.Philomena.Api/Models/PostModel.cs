using System;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class PostModel
    {
        /// <summary>
        /// The post's author.
        /// </summary>
        [JsonProperty("author")]
        public string? Author { get; set; }

        /// <summary>
        /// The URL of the author's avatar. May be a link to the CDN path, or a data: URI.
        /// </summary>
        [JsonProperty("avatar")]
        public string? Avatar { get; set; }

        /// <summary>
        /// The post text.
        /// </summary>
        [JsonProperty("body")]
        public string? Body { get; set; }

        /// <summary>
        /// The creation time, in UTC, of the post.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// The edit reason for this post.
        /// </summary>
        [JsonProperty("edit_reason")]
        public string? EditReason { get; set; }

        /// <summary>
        /// The time, in UTC, this post was last edited at, or null if it was not edited.
        /// </summary>
        [JsonProperty("edited_at")]
        public DateTime? EditedAt { get; set; }

        /// <summary>
        /// The post's ID (used to identify it).
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// The time, in UTC, the post was last updated at.
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The ID of the user the post belongs to, if any.
        /// </summary>
        [JsonProperty("user_id")]
        public int? UserId { get; set; }
    }
}
