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
        public User Creator => Authors[CreatorId];
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public EntityCollection<DownloadFile> Files { get; set; } = new EntityCollection<DownloadFile>();
        public string Version { get; set; }
        public DateTime? Updated { get; set; }
        public EntityCollection<User> Authors { get; set; } = new EntityCollection<User>();
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
