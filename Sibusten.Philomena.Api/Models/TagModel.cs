using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class TagModel
    {
        /// <summary>
        /// The slug of the tag this tag is aliased to, if any.
        /// </summary>
        [JsonProperty("aliased_tag")]
        public string? AliasedTag { get; set; }

        /// <summary>
        /// The slugs of the tags aliased to this tag.
        /// </summary>
        [JsonProperty("aliases")]
        public List<string>? Aliases { get; set; }

        /// <summary>
        /// The category class of this tag. One of "character", "content-fanmade", "content-official", "error", "oc", "origin", "rating", "species", "spoiler".
        /// </summary>
        [JsonProperty("category")]
        public string? Category { get; set; }

        /// <summary>
        /// The long description for the tag.
        /// </summary>
        [JsonProperty("description")]
        public string? Description { get; set; }

        /// <summary>
        /// An array of objects containing DNP entries claimed on the tag.
        /// </summary>
        [JsonProperty("dnp_entries")]
        public List<DnpEntryModel>? DnpEntries { get; set; }

        /// <summary>
        /// The tag's ID.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// The image count of the tag.
        /// </summary>
        [JsonProperty("images")]
        public int? Images { get; set; }

        /// <summary>
        /// The slugs of the tags this tag is implied by.
        /// </summary>
        [JsonProperty("implied_by_tags")]
        public List<string>? ImpliedByTags { get; set; }

        /// <summary>
        /// The slugs of the tags this tag implies.
        /// </summary>
        [JsonProperty("implied_tags")]
        public List<string>? ImpliedTags { get; set; }

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

        /// <summary>
        /// The short description for the tag.
        /// </summary>
        [JsonProperty("short_description")]
        public string? ShortDescription { get; set; }

        /// <summary>
        /// The slug for the tag.
        /// </summary>
        [JsonProperty("slug")]
        public string? Slug { get; set; }

        /// <summary>
        /// The spoiler image for the tag.
        /// </summary>
        [JsonProperty("spoiler_image_uri")]
        public string? SpoilerImageUri { get; set; }
    }

    public class TagResponseModel
    {
        [JsonProperty("tag")]
        public TagModel? Tag { get; set; }
    }
}
