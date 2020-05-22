using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Models
{
    public class User : EntityBase
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public int Balance { get; set; }
        public string HashedPassword { get; set; } 
        public List<Role> Roles { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
