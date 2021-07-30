using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Com.Data.Collections;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("objectives")]
    public class Objective : EntityBase
    {
        public string AppId { get; set; }
        public string Name { get; set; }
        public IEntityCollection<Score> Scores { get; set; }
    }
}
