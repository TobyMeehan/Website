using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class AuthorizationCode
    {
        public string Id { get; set; }
        public Connection Connection { get; set; }
        public string Code { get; set; }
        public DateTime Expiry { get; set; }
        public bool IsValid => Expiry >= DateTime.Now;
    }
}
