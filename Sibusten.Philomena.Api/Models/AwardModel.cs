using System;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class AwardModel
    {
        /// <summary>
        /// The URL of this award.
        /// </summary>
        [JsonProperty("image_url")]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// The title of this award.
        /// </summary>
        [JsonProperty("title")]
        public string? Title { get; set; }

        /// <summary>
        /// The ID of the badge this award is derived from.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// The label of this award.
        /// </summary>
        [JsonProperty("label")]
        public string? Label { get; set; }

        /// <summary>
        /// The time, in UTC, when this award was given.
        /// </summary>
        [JsonProperty("awarded_on")]
        public DateTime? AwardedOn { get; set; }
    }
}
