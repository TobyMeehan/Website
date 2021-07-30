using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class Comment : EntityBase
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
        public DateTime Sent { get; set; }
        public DateTime? Edited { get; set; }
        public string EntityId { get; set; }
    }
}
