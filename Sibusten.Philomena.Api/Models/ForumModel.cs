using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class ForumModel
    {
        /// <summary>
        /// The forum's name.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// The forum's short name (used to identify it).
        /// </summary>
        [JsonProperty("short_name")]
        public string? ShortName { get; set; }

        /// <summary>
        /// The forum's description.
        /// </summary>
        [JsonProperty("description")]
        public string? Description { get; set; }

        /// <summary>
        /// The amount of topics in the forum.
        /// </summary>
        [JsonProperty("topic_count")]
        public int? TopicCount { get; set; }

        /// <summary>
        /// The amount of posts in the forum.
        /// </summary>
        [JsonProperty("post_count")]
        public int? PostCount { get; set; }
    }
}
