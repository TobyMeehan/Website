using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("comments")]
    public class Comment : MessageBase
    {
        public string EntityId { get; set; }
    }
}
