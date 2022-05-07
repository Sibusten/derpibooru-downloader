using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class TagModel
    {
        /// <summary>
        /// The category class of this tag. One of "character", "content-fanmade", "content-official", "error", "oc", "origin", "rating", "species", "spoiler".
        /// </summary>
        [JsonProperty("category")]
        public string? Category { get; set; }

        /// <summary>
        /// The tag's ID.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// The name of the tag.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// The name of the tag in its namespace.
        /// </summary>
        [JsonProperty("name_in_namespace")]
        public string? NameInNamespace { get; set; }

        /// <summary>
        /// The namespace of the tag.
        /// </summary>
        [JsonProperty("namespace")]
        public string? Namespace { get; set; }
    }
}
