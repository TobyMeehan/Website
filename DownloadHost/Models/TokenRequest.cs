using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadHost.Models
{
    public class TokenRequest
    {
        public string Secret { get; set; }
        public string DownloadId { get; set; }
        public string Filename { get; set; }
        public int Partitions { get; set; }

    }
}
