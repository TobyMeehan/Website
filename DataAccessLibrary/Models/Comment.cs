using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("comments")]
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
