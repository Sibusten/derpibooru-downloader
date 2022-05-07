using System;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class DnpEntryModel
    {
        [JsonProperty("conditions")]
        public string? Conditions { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("dnp_type")]
        public string? DnpType { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("reason")]
        public string? Reason { get; set; }
    }
}
