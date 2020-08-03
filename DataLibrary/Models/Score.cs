using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("scoreboard")]
    public class Score : EntityBase
    {
        public string ObjectiveId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int Value { get; set; }
    }
}
