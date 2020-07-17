using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("comments")]
    public class Comment : EntityBase
    {
        public string EntityId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
    }
}
