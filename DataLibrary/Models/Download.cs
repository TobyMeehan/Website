using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("downloads")]
    public class Download : EntityBase
    {
        public string CreatorId { get; set; }
        public User Creator => Authors.Find(user => user.Id == CreatorId);
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public List<DownloadFile> Files { get; set; }
        public string Version { get; set; }
        public DateTime? Updated { get; set; }
        public List<User> Authors { get; set; }
        public DownloadVerification Verified { get; set; }
        public virtual int VerifiedId
        {
            get
            {
                return (int)Verified;
            }
            set
            {
                Verified = (DownloadVerification)value;
            }
        }
    }
}
