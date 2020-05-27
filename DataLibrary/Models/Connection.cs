using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("connections")]
    public class Connection : IEntity
    {
        public string Id { get; set; }
        public string AppId { get; set; }
        public Application Application { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
