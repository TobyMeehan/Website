using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("objectives")]
    public class Objective : EntityBase
    {
        public string AppId { get; set; }
        public string Name { get; set; }
        public EntityCollection<Score> Scores { get; set; }
    }
}
