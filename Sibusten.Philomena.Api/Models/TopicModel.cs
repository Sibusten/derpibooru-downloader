using System;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class TopicModel
    {
        /// <summary>
        /// The topic's slug (used to identify it).
        /// </summary>
        [JsonProperty("slug")]
        public string? Slug { get; set; }

        /// <summary>
        /// The topic's title.
        /// </summary>
        [JsonProperty("title")]
        public string? Title { get; set; }

        /// <summary>
        /// The amount of posts in the topic.
        /// </summary>
        [JsonProperty("post_count")]
        public int? PostCount { get; set; }

        /// <summary>
        /// The amount of views the topic has received.
        /// </summary>
        [JsonProperty("view_count")]
        public int? ViewCount { get; set; }

        /// <summary>
        /// Whether the topic is sticky.
        /// </summary>
        [JsonProperty("sticky")]
        public bool? IsSticky { get; set; }

        /// <summary>
        /// The time, in UTC, when the last reply was made.
        /// </summary>
        [JsonProperty("last_replied_to_at")]
        public DateTime? LastRepliedToAt { get; set; }

        /// <summary>
        /// Whether the topic is locked.
        /// </summary>
        [JsonProperty("locked")]
        public bool? IsLocked { get; set; }

        /// <summary>
        /// The ID of the user who made the topic. null if posted anonymously.
        /// </summary>
        [JsonProperty("user_id")]
        public int? UserId { get; set; }

        /// <summary>
        /// The name of the user who made the topic.
        /// </summary>
        [JsonProperty("author")]
        public string? Author { get; set; }
    }
}
