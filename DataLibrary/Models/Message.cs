using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class Message : EntityBase
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
    }
}
