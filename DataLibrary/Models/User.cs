using System;
using System.Collections.Generic;
using System.Text;
using TobyMeehan.Sql.QueryBuilder;

namespace TobyMeehan.Com.Data.Models
{
    [SqlName("users")]
    public class User : IEntity
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Balance { get; set; }
        public string HashedPassword { get; set; } 
        public List<Role> Roles { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
