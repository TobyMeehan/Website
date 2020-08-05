using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models.Api
{
    public class DownloadResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }

        [JsonPropertyName("short_description")]
        public string ShortDescription { get; set; }

        [JsonPropertyName("long_description")]
        public string LongDescription { get; set; }
    }
}
