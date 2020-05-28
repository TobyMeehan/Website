using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Models
{
    public class UsernameViewModel
    {
        public UsernameViewModel() { }

        public UsernameViewModel(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
    }
}
