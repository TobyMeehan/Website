using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("transactions")]
    public class Transaction : EntityBase
    {
        public string UserId { get; set; }
        public string Sender { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
    }
}
