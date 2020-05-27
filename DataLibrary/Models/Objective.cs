using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("objectives")]
    public class Objective : IEntity
    {
        public string Id { get; set; }
        public string AppId { get; set; }
        public string Name { get; set; }
    }
}
