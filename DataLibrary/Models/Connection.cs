using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class Connection : EntityBase
    {
        public string AppId { get; set; }
        public Application Application { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
