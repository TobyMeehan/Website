using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Models
{
    public class DownloadFile
    {
        public string Id { get; set; }
        public string DownloadId { get; set; }
        public string Filename { get; set; }
        public string RandomName { get; set; }
    }
}
