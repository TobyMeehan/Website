using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("downloadauthors")]
    public class DownloadAuthor
    {
        public string DownloadId { get; set; }
        public string UserId { get; set; }

    }
}
