using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class CloudFile
    {
        public CloudFile(string downloadLink, string mediaLink)
        {
            DownloadLink = downloadLink;
            MediaLink = mediaLink;
        }

        public string DownloadLink { get; set; }
        public string MediaLink { get; set; }
    }
}
