using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class Transaction : EntityBase
    {
        public string UserId { get; set; }
        public string AppId { get; set; }
        public Application Application { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public DateTime Sent { get; set; }
    }
}
