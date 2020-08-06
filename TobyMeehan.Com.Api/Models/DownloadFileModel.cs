using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models
{
    public class DownloadFileModel : EntityModel
    {
        public string DownloadId { get; set; }
        public string Filename { get; set; }
        public string Url { get; set; }
    }
}
