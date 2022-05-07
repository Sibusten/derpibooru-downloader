using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sibusten.Philomena.Api.Models
{
    public class ImageSearchModel
    {
        [JsonProperty("images")]
        public List<ImageModel>? Images { get; set; }

        [JsonProperty("total")]
        public int? Total { get; set; }
    }
}
