﻿using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Com.Data.Collections;

namespace TobyMeehan.Com.Data.Models
{
    public class Download : EntityBase
    {
        public string Title { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public DownloadVisibility Visibility { get; set; }

        public IEntityCollection<DownloadFile> Files { get; set; } = new EntityCollection<DownloadFile>();

        public Version Version { get; set; }
        public string VersionString
        {
            get
            {
                return Version.ToString();
            }
            set
            {
                Version = Version.Parse(value);
            }
        }

        public DateTime? Updated { get; set; }

        public IEntityCollection<User> Authors { get; set; } = new EntityCollection<User>();

        public DownloadVerification Verified { get; set; }
    }
}
