using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("conversations")]
    public class Conversation : EntityBase
    {
        public string Name { get; set; }

        public EntityCollection<User> Users { get; set; } = new EntityCollection<User>();
    }
}
