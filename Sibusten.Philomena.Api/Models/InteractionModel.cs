using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class InteractionModel
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("interaction_type")]
        public string? InteractionType { get; set; }

        [JsonProperty("value")]
        public string? Value { get; set; }

        [JsonProperty("user_id")]
        public int? UserId { get; set; }

        [JsonProperty("image_id")]
        public int? ImageId { get; set; }
    }
}
