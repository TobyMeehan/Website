using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models.OAuth
{
    public class AuthCodeTokenRequest
    {
        [JsonPropertyName("grant_type")]
        [FromForm(Name = "grant_type")]
        public string GrantType { get; set; }

        [JsonPropertyName("code")]
        [FromForm(Name = "code")]
        public string Code { get; set; }

        [JsonPropertyName("redirect_uri")]
        [FromForm(Name = "redirect_uri")]
        public string RedirectUri { get; set; }

        [JsonPropertyName("client_id")]
        [FromForm(Name = "client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("client_secret")]
        [FromForm(Name = "client_secret")]
        public string ClientSecret { get; set; }

        [JsonPropertyName("code_verifier")]
        [FromForm(Name = "code_verifier")]
        public string CodeVerifier { get; set; }
    }
}
