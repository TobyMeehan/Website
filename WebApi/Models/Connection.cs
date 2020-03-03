using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Connection
    {
        public Application Application { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
        public string AuthorizationCode { get; set; }
    }
}
