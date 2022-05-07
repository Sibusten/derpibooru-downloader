using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class OembedModel
    {
        /// <summary>
        /// The comma-delimited names of the image authors.
        /// </summary>
        [JsonProperty("author_name")]
        public string? AuthorName { get; set; }

        /// <summary>
        /// The source URL of the image.
        /// </summary>
        [JsonProperty("author_url")]
        public string? AuthorUrl { get; set; }

        /// <summary>
        /// Always 7200.
        /// </summary>
        [JsonProperty("cache_age")]
        public int? CacheAge { get; set; }

        /// <summary>
        /// The number of comments made on the image.
        /// </summary>
        [JsonProperty("derpibooru_comments")]
        public int? DerpibooruComments { get; set; }

        /// <summary>
        /// The image's ID.
        /// </summary>
        [JsonProperty("derpibooru_id")]
        public int? DerpibooruId { get; set; }

        /// <summary>
        /// The image's number of upvotes minus the image's number of downvotes.
        /// </summary>
        [JsonProperty("derpibooru_score")]
        public int? DerpibooruScore { get; set; }

        /// <summary>
        /// The names of the image's tags.
        /// </summary>
        [JsonProperty("derpibooru_tags")]
        public List<string>? DerpibooruTags { get; set; }

        /// <summary>
        /// Always "Derpibooru".
        /// </summary>
        [JsonProperty("provider_name")]
        public string? ProviderName { get; set; }

        /// <summary>
        /// Always "https://derpibooru.org".
        /// </summary>
        [JsonProperty("provider_url")]
        public string? ProviderUrl { get; set; }

        /// <summary>
        /// The image's ID and associated tags, as would be given on the title of the image page.
        /// </summary>
        [JsonProperty("title")]
        public string? Title { get; set; }

        /// <summary>
        /// Always "photo".
        /// </summary>
        [JsonProperty("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Always "1.0".
        /// </summary>
        [JsonProperty("version")]
        public string? Version { get; set; }
    }
}
