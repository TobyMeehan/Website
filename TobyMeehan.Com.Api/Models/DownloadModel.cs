using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Api.Models
{
    public class DownloadModel : EntityModel
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public IList<UserModel> Authors { get; set; }
        public IList<DownloadFileModel> Files { get; set; }
        public Version Version { get; set; }
    }
}
