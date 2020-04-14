using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<Role> Roles { get; set; }
        public List<Transaction> Transactions { get; set; }
        public int Balance { get; set; }
    }
}
