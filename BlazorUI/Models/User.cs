using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorUI.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<Role> Roles { get; set; }
        public bool IsVerified => Roles.Any(role => role.Name == "Verified");
        public List<Transaction> Transactions { get; set; }
        public int Balance { get; set; }
    }
}
