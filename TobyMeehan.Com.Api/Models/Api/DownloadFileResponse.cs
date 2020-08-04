using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models.Api
{
    public class DownloadFileResponse
    {
        public string Id { get; set; }
        public string Filename { get; set; }
        public string Url { get; set; }
    }
}
