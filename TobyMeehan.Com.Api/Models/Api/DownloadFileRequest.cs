using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models.Api
{
    [ModelBinder(typeof(JsonFormFileModelBinder), Name = "json")]
    public class DownloadFileRequest
    {
        [JsonPropertyName("download_id")]
        public string DownloadId { get; set; }
        public string Filename { get; set; }
        public IFormFile File { get; set; }
    }
}
