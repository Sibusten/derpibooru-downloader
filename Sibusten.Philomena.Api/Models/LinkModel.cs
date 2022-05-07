using System;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class LinkModel
    {
        /// <summary>
        /// The ID of the user who owns this link.
        /// </summary>
        [JsonProperty("user_id")]
        public int? UserId { get; set; }

        /// <summary>
        /// The creation time, in UTC, of this link.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// The state of this link.
        /// </summary>
        [JsonProperty("state")]
        public string? State { get; set; }

        /// <summary>
        /// The ID of an associated tag for this link. null if no tag linked.
        /// </summary>
        [JsonProperty("tag_id")]
        public int? TagId { get; set; }
    }
}
