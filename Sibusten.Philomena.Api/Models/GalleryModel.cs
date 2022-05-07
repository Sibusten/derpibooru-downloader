using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class GalleryModel
    {
        /// <summary>
        /// The gallery's description.
        /// </summary>
        [JsonProperty("description")]
        public string? Description { get; set; }

        /// <summary>
        /// The gallery's ID.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// The gallery's spoiler warning.
        /// </summary>
        [JsonProperty("spoiler_warning")]
        public string? SpoilerWarning { get; set; }

        /// <summary>
        /// The ID of the cover image for the gallery.
        /// </summary>
        [JsonProperty("thumbnail_id")]
        public int? ThumbnailId { get; set; }

        /// <summary>
        /// The gallery's title.
        /// </summary>
        [JsonProperty("title")]
        public string? Title { get; set; }

        /// <summary>
        /// The name of the gallery's creator.
        /// </summary>
        [JsonProperty("user")]
        public string? User { get; set; }

        /// <summary>
        /// The ID of the gallery's creator.
        /// </summary>
        [JsonProperty("user_id")]
        public int? UserId { get; set; }
    }
}
