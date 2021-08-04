using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Extensions
{
    public static class DownloadExtensions
    {
        public static string UpdatedString(this Download download)
        {
            TimeSpan? timeSpan = DateTime.Now - download.Updated;

            return $"{timeSpan.Value.Humanize()} ago";
        }
    }
}
