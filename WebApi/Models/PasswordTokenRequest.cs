using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class PasswordTokenRequest
    {
        public string grant_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string client_id { get; set; }

    }
}
