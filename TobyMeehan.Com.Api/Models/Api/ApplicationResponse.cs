using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models.Api
{
    public class ApplicationResponse
    {
        public string Name { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        public string Description { get; set; }

        [JsonPropertyName("icon_url")]
        public string IconUrl { get; set; }
    }
}
