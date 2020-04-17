﻿using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Download
    {
        public string Id { get; set; }
        public string CreatorId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public List<string> Files { get; set; }
        //public string Version { get; set; }
        public DateTime? Updated { get; set; }
        public List<SimplifiedUser> Authors { get; set; }
        public DownloadVerification Verified { get; set; }
    }
}
